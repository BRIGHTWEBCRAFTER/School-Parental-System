
create or replace function public.search_school_parents(
  p_query text,
  p_limit integer default 8
)
returns table (
  parent_id uuid,
  guardian_number varchar,
  full_name varchar,
  phone varchar,
  email varchar
)
language sql
stable
security invoker
set search_path = public, private, pg_temp
as $$
  with params as (
    select
      lower(btrim(coalesce(p_query, ''))) as q,
      greatest(1, least(coalesce(p_limit, 8), 10)) as lim
  )
  select
    p.id as parent_id,
    p.guardian_number,
    u.full_name,
    u.phone,
    u.email
  from public.parents p
  join public.users u
    on u.id = p.user_id
  cross join params x
  where private.current_user_role() = 'School Admin'
    and u.school_id = private.current_school_id()
    and p.is_active = true
    and u.is_active = true
    and length(x.q) >= 2
    and (
      lower(p.guardian_number) like '%' || x.q || '%'
      or lower(u.full_name) like '%' || x.q || '%'
      or lower(coalesce(u.phone, '')) like '%' || x.q || '%'
      or lower(coalesce(u.email, '')) like '%' || x.q || '%'
    )
  order by
    case
      when lower(p.guardian_number) = x.q then 0
      when lower(p.guardian_number) like x.q || '%' then 1
      when lower(u.full_name) like x.q || '%' then 2
      else 3
    end,
    u.full_name,
    p.guardian_number
  limit (select lim from params);
$$;

revoke execute on function public.search_school_parents(text, integer)
from public, anon;

grant execute on function public.search_school_parents(text, integer)
to authenticated;

comment on function public.search_school_parents(text, integer) is
  'Searches active parents for the signed-in School Admin school only, returning at most 10 matches.';
;
