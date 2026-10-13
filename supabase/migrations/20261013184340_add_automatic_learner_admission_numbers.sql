
-- Automatic learner admission numbers.
-- Format: PREFIX-YEAR-SEQUENCE, e.g. GF-2026-002.

create schema if not exists private;

alter table public.schools
  add column if not exists admission_prefix varchar(12);

alter table public.schools
  drop constraint if exists schools_admission_prefix_format_check;

alter table public.schools
  add constraint schools_admission_prefix_format_check
  check (
    admission_prefix is null
    or (
      admission_prefix = upper(admission_prefix)
      and admission_prefix ~ '^[A-Z0-9]+$'
    )
  );

create unique index if not exists uq_schools_admission_prefix
  on public.schools (admission_prefix)
  where admission_prefix is not null;

-- Use names rather than generated UUIDs so this migration remains portable.
update public.schools
set admission_prefix = 'GF'
where name = 'Goldfield High School'
  and admission_prefix is distinct from 'GF';

update public.schools
set admission_prefix = 'SD'
where name = 'St Dominic''s College High School'
  and admission_prefix is distinct from 'SD';

create table if not exists private.learner_admission_counters (
  school_id uuid not null
    references public.schools(id)
    on delete cascade,
  admission_year integer not null,
  last_number integer not null default 0,
  primary key (school_id, admission_year),
  constraint learner_admission_counters_last_number_check
    check (last_number >= 0)
);

revoke all on table private.learner_admission_counters
  from anon, authenticated;

-- Seed counters from existing admission numbers that already follow
-- each school's configured prefix-year-sequence format.
insert into private.learner_admission_counters (
  school_id,
  admission_year,
  last_number
)
select
  l.school_id,
  split_part(l.admission_number, '-', 2)::integer as admission_year,
  max(split_part(l.admission_number, '-', 3)::integer) as last_number
from public.learners l
join public.schools s
  on s.id = l.school_id
where s.admission_prefix is not null
  and l.admission_number ~ '^[A-Z0-9]+-[0-9]{4}-[0-9]+$'
  and split_part(l.admission_number, '-', 1) = s.admission_prefix
group by
  l.school_id,
  split_part(l.admission_number, '-', 2)::integer
on conflict (school_id, admission_year)
do update
set last_number = greatest(
  private.learner_admission_counters.last_number,
  excluded.last_number
);

create or replace function public.assign_learner_admission_number()
returns trigger
language plpgsql
security definer
set search_path = pg_catalog, public, private
as $$
declare
  v_prefix text;
  v_year integer;
  v_next_number integer;
begin
  select s.admission_prefix
    into v_prefix
  from public.schools s
  where s.id = new.school_id;

  if v_prefix is null or btrim(v_prefix) = '' then
    raise exception 'Admission prefix has not been configured for this school.';
  end if;

  v_year := extract(
    year from timezone('Africa/Johannesburg', current_timestamp)
  )::integer;

  insert into private.learner_admission_counters (
    school_id,
    admission_year,
    last_number
  )
  values (
    new.school_id,
    v_year,
    1
  )
  on conflict (school_id, admission_year)
  do update
  set last_number =
    private.learner_admission_counters.last_number + 1
  returning last_number
    into v_next_number;

  new.admission_number :=
    v_prefix
    || '-'
    || v_year::text
    || '-'
    || lpad(v_next_number::text, 3, '0');

  return new;
end;
$$;

drop trigger if exists trigger_assign_learner_admission_number
  on public.learners;

create trigger trigger_assign_learner_admission_number
before insert on public.learners
for each row
execute function public.assign_learner_admission_number();

comment on column public.schools.admission_prefix is
  'Short uppercase prefix used to generate learner admission numbers, e.g. GF or SD.';

comment on function public.assign_learner_admission_number() is
  'Generates a concurrency-safe learner admission number before insert.';

-- public.learners already has UNIQUE (school_id, admission_number)
-- via uq_learners_school_admission, so no duplicate index is added.
;
