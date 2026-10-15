
create table if not exists private.school_reference_counters (
  school_id uuid not null references public.schools(id) on delete cascade,
  reference_type text not null
    check (reference_type in ('teacher', 'guardian')),
  last_number integer not null default 0
    check (last_number >= 0),
  updated_at timestamptz not null default now(),
  primary key (school_id, reference_type)
);

revoke all on private.school_reference_counters
from public, anon, authenticated;

insert into private.school_reference_counters (
  school_id,
  reference_type,
  last_number
)
select
  s.id,
  'teacher',
  coalesce(
    max(
      case
        when t.employee_id ~ (
          '^' || s.admission_prefix || '-T-[0-9]+$'
        )
        then substring(t.employee_id from '([0-9]+)$')::integer
      end
    ),
    0
  )
from public.schools s
left join public.teachers t
  on t.school_id = s.id
where s.admission_prefix is not null
group by s.id
on conflict (school_id, reference_type)
do update set
  last_number = greatest(
    private.school_reference_counters.last_number,
    excluded.last_number
  ),
  updated_at = now();

insert into private.school_reference_counters (
  school_id,
  reference_type,
  last_number
)
select
  s.id,
  'guardian',
  coalesce(
    max(
      case
        when p.guardian_number ~ (
          '^' || s.admission_prefix || '-G-[0-9]+$'
        )
        then substring(p.guardian_number from '([0-9]+)$')::integer
      end
    ),
    0
  )
from public.schools s
left join public.users u
  on u.school_id = s.id
left join public.parents p
  on p.user_id = u.id
where s.admission_prefix is not null
group by s.id
on conflict (school_id, reference_type)
do update set
  last_number = greatest(
    private.school_reference_counters.last_number,
    excluded.last_number
  ),
  updated_at = now();

create or replace function private.next_school_reference_number(
  target_school_id uuid,
  target_reference_type text
)
returns text
language plpgsql
security definer
set search_path = ''
as $$
declare
  v_prefix text;
  v_next integer;
  v_marker text;
begin
  if target_reference_type not in ('teacher', 'guardian') then
    raise exception 'Unsupported school reference type';
  end if;

  select s.admission_prefix
    into v_prefix
  from public.schools s
  where s.id = target_school_id;

  if v_prefix is null or v_prefix = '' then
    raise exception 'School admission prefix is not configured';
  end if;

  insert into private.school_reference_counters (
    school_id,
    reference_type,
    last_number,
    updated_at
  )
  values (
    target_school_id,
    target_reference_type,
    1,
    now()
  )
  on conflict (school_id, reference_type)
  do update set
    last_number =
      private.school_reference_counters.last_number + 1,
    updated_at = now()
  returning last_number into v_next;

  v_marker :=
    case target_reference_type
      when 'teacher' then 'T'
      when 'guardian' then 'G'
    end;

  return
    upper(v_prefix)
    || '-'
    || v_marker
    || '-'
    || lpad(v_next::text, 3, '0');
end;
$$;

revoke all on function private.next_school_reference_number(uuid, text)
from public, anon, authenticated;

create or replace function public.assign_teacher_employee_id()
returns trigger
language plpgsql
security definer
set search_path = ''
as $$
begin
  new.employee_id :=
    private.next_school_reference_number(
      new.school_id,
      'teacher'
    );

  return new;
end;
$$;

revoke all on function public.assign_teacher_employee_id()
from public, anon, authenticated;

drop trigger if exists trg_assign_teacher_employee_id
on public.teachers;

create trigger trg_assign_teacher_employee_id
before insert on public.teachers
for each row
execute function public.assign_teacher_employee_id();

create or replace function public.assign_parent_guardian_number()
returns trigger
language plpgsql
security definer
set search_path = ''
as $$
declare
  v_school_id uuid;
begin
  select u.school_id
    into v_school_id
  from public.users u
  where u.id = new.user_id;

  if v_school_id is null then
    raise exception 'Parent user must belong to a school';
  end if;

  new.guardian_number :=
    private.next_school_reference_number(
      v_school_id,
      'guardian'
    );

  return new;
end;
$$;

revoke all on function public.assign_parent_guardian_number()
from public, anon, authenticated;

drop trigger if exists trg_assign_parent_guardian_number
on public.parents;

create trigger trg_assign_parent_guardian_number
before insert on public.parents
for each row
execute function public.assign_parent_guardian_number();
;
