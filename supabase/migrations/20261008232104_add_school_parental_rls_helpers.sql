
create schema if not exists private;

create or replace function private.current_user_role()
returns text
language sql
stable
security definer
set search_path = ''
as $$
  select u.role
  from public.users u
  where u.id = (select auth.uid())
    and u.is_active = true
  limit 1
$$;

create or replace function private.current_school_id()
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select u.school_id
  from public.users u
  where u.id = (select auth.uid())
    and u.is_active = true
  limit 1
$$;

create or replace function private.current_parent_id()
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select p.id
  from public.parents p
  join public.users u on u.id = p.user_id
  where p.user_id = (select auth.uid())
    and p.is_active = true
    and u.is_active = true
    and u.role = 'Parent'
  limit 1
$$;

create or replace function private.current_teacher_id()
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select t.id
  from public.teachers t
  join public.users u on u.id = t.user_id
  where t.user_id = (select auth.uid())
    and t.is_active = true
    and u.is_active = true
    and u.role = 'Teacher'
  limit 1
$$;

create or replace function private.is_super_admin()
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select coalesce(private.current_user_role() = 'Super Admin', false)
$$;

create or replace function private.can_manage_school(target_school_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select private.is_super_admin()
     or (
       private.current_user_role() = 'School Admin'
       and private.current_school_id() = target_school_id
     )
$$;

create or replace function private.user_school(target_user_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select u.school_id from public.users u where u.id = target_user_id limit 1
$$;

create or replace function private.user_role(target_user_id uuid)
returns text
language sql
stable
security definer
set search_path = ''
as $$
  select u.role from public.users u where u.id = target_user_id limit 1
$$;

create or replace function private.parent_school(target_parent_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select u.school_id
  from public.parents p
  join public.users u on u.id = p.user_id
  where p.id = target_parent_id
  limit 1
$$;

create or replace function private.teacher_school(target_teacher_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select t.school_id from public.teachers t where t.id = target_teacher_id limit 1
$$;

create or replace function private.student_school(target_student_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select s.school_id from public.students s where s.id = target_student_id limit 1
$$;

create or replace function private.class_school(target_class_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select c.school_id from public.classes c where c.id = target_class_id limit 1
$$;

create or replace function private.subject_school(target_subject_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select s.school_id from public.subjects s where s.id = target_subject_id limit 1
$$;

create or replace function private.parent_linked_student(target_student_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_students ps
    where ps.parent_id = private.current_parent_id()
      and ps.student_id = target_student_id
      and ps.is_active = true
  )
$$;

create or replace function private.teacher_assigned_to_class(target_class_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.classes c
    where c.id = target_class_id
      and c.is_active = true
      and c.class_teacher_id = private.current_teacher_id()
  )
  or exists (
    select 1
    from public.teaching_assignments ta
    where ta.class_id = target_class_id
      and ta.teacher_id = private.current_teacher_id()
      and ta.is_active = true
  )
$$;

create or replace function private.teacher_assigned_to_subject_class(
  target_class_id uuid,
  target_subject_id uuid
)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.teaching_assignments ta
    where ta.class_id = target_class_id
      and ta.subject_id = target_subject_id
      and ta.teacher_id = private.current_teacher_id()
      and ta.is_active = true
  )
$$;

create or replace function private.student_enrolled_in_class(
  target_student_id uuid,
  target_class_id uuid
)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.enrollments e
    where e.student_id = target_student_id
      and e.class_id = target_class_id
      and e.is_active = true
  )
$$;

create or replace function private.teacher_can_access_student(target_student_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.enrollments e
    where e.student_id = target_student_id
      and e.is_active = true
      and private.teacher_assigned_to_class(e.class_id)
  )
$$;

create or replace function private.parent_has_student_in_grade(target_grade text)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_students ps
    join public.students s on s.id = ps.student_id
    where ps.parent_id = private.current_parent_id()
      and ps.is_active = true
      and s.is_active = true
      and s.grade = target_grade
  )
$$;

create or replace function private.parent_has_student_in_class(target_class_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_students ps
    join public.enrollments e on e.student_id = ps.student_id
    where ps.parent_id = private.current_parent_id()
      and ps.is_active = true
      and e.class_id = target_class_id
      and e.is_active = true
  )
$$;

create or replace function private.is_announcement_recipient(target_announcement_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.announcement_recipients ar
    where ar.announcement_id = target_announcement_id
      and ar.user_id = (select auth.uid())
  )
$$;

revoke all on schema private from public, anon;
grant usage on schema private to authenticated;
revoke execute on all functions in schema private from public, anon;
grant execute on all functions in schema private to authenticated;
;
