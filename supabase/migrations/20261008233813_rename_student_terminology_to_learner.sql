
-- Use "Learner" terminology consistently throughout the application database.

alter table public.students rename to learners;
alter table public.parent_students rename to parent_learners;

alter table public.parent_learners rename column student_id to learner_id;
alter table public.enrollments rename column student_id to learner_id;
alter table public.attendance rename column student_id to learner_id;
alter table public.grades rename column student_id to learner_id;

-- Rename application constraints for clarity.
alter table public.learners
  rename constraint uq_students_school_admission to uq_learners_school_admission;

alter table public.parent_learners
  rename constraint uq_parent_students_link to uq_parent_learners_link;

alter table public.enrollments
  rename constraint uq_enrollments_student_class_year to uq_enrollments_learner_class_year;

alter table public.attendance
  rename constraint uq_attendance_student_date_class to uq_attendance_learner_date_class;

-- Rename automatically-created FK constraints where they still have old terminology.
do $$
begin
  if exists (select 1 from pg_constraint where conname='students_school_id_fkey' and conrelid='public.learners'::regclass) then
    alter table public.learners rename constraint students_school_id_fkey to learners_school_id_fkey;
  end if;
  if exists (select 1 from pg_constraint where conname='parent_students_parent_id_fkey' and conrelid='public.parent_learners'::regclass) then
    alter table public.parent_learners rename constraint parent_students_parent_id_fkey to parent_learners_parent_id_fkey;
  end if;
  if exists (select 1 from pg_constraint where conname='parent_students_student_id_fkey' and conrelid='public.parent_learners'::regclass) then
    alter table public.parent_learners rename constraint parent_students_student_id_fkey to parent_learners_learner_id_fkey;
  end if;
  if exists (select 1 from pg_constraint where conname='enrollments_student_id_fkey' and conrelid='public.enrollments'::regclass) then
    alter table public.enrollments rename constraint enrollments_student_id_fkey to enrollments_learner_id_fkey;
  end if;
  if exists (select 1 from pg_constraint where conname='attendance_student_id_fkey' and conrelid='public.attendance'::regclass) then
    alter table public.attendance rename constraint attendance_student_id_fkey to attendance_learner_id_fkey;
  end if;
  if exists (select 1 from pg_constraint where conname='grades_student_id_fkey' and conrelid='public.grades'::regclass) then
    alter table public.grades rename constraint grades_student_id_fkey to grades_learner_id_fkey;
  end if;
end
$$;

-- Rename custom indexes.
alter index if exists public.idx_students_school_id rename to idx_learners_school_id;
alter index if exists public.idx_parent_students_parent_id rename to idx_parent_learners_parent_id;
alter index if exists public.idx_parent_students_student_id rename to idx_parent_learners_learner_id;
alter index if exists public.uq_parent_students_one_primary rename to uq_parent_learners_one_primary;
alter index if exists public.idx_enrollments_student_id rename to idx_enrollments_learner_id;
alter index if exists public.idx_attendance_student_id rename to idx_attendance_learner_id;
alter index if exists public.idx_grades_student_id rename to idx_grades_learner_id;

alter trigger trg_students_updated_at on public.learners
  rename to trg_learners_updated_at;
alter trigger trg_parent_students_updated_at on public.parent_learners
  rename to trg_parent_learners_updated_at;

-- New learner-named RLS helper functions.
create or replace function private.learner_school(target_learner_id uuid)
returns uuid
language sql
stable
security definer
set search_path = ''
as $$
  select l.school_id
  from public.learners l
  where l.id = target_learner_id
  limit 1
$$;

create or replace function private.parent_linked_learner(target_learner_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_learners pl
    where pl.parent_id = private.current_parent_id()
      and pl.learner_id = target_learner_id
      and pl.is_active = true
  )
$$;

create or replace function private.learner_enrolled_in_class(
  target_learner_id uuid,
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
    where e.learner_id = target_learner_id
      and e.class_id = target_class_id
      and e.is_active = true
  )
$$;

create or replace function private.teacher_can_access_learner(target_learner_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.enrollments e
    where e.learner_id = target_learner_id
      and e.is_active = true
      and private.teacher_assigned_to_class(e.class_id)
  )
$$;

create or replace function private.parent_has_learner_in_grade(target_grade text)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_learners pl
    join public.learners l on l.id = pl.learner_id
    where pl.parent_id = private.current_parent_id()
      and pl.is_active = true
      and l.is_active = true
      and l.grade = target_grade
  )
$$;

create or replace function private.parent_has_learner_in_class(target_class_id uuid)
returns boolean
language sql
stable
security definer
set search_path = ''
as $$
  select exists (
    select 1
    from public.parent_learners pl
    join public.enrollments e on e.learner_id = pl.learner_id
    where pl.parent_id = private.current_parent_id()
      and pl.is_active = true
      and e.class_id = target_class_id
      and e.is_active = true
  )
$$;

revoke execute on all functions in schema private from public, anon;
grant execute on all functions in schema private to authenticated;

-- Replace policies that referenced student terminology/functions.
drop policy if exists "students_select" on public.learners;
drop policy if exists "students_insert_admin" on public.learners;
drop policy if exists "students_update_admin" on public.learners;

create policy "learners_select"
on public.learners for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_can_access_learner(id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_linked_learner(id)
  )
);

create policy "learners_insert_admin"
on public.learners for insert to authenticated
with check (private.can_manage_school(school_id));

create policy "learners_update_admin"
on public.learners for update to authenticated
using (private.can_manage_school(school_id))
with check (private.can_manage_school(school_id));

drop policy if exists "parent_students_select" on public.parent_learners;
drop policy if exists "parent_students_insert_admin" on public.parent_learners;
drop policy if exists "parent_students_update_admin" on public.parent_learners;

create policy "parent_learners_select"
on public.parent_learners for select to authenticated
using (
  private.is_super_admin()
  or parent_id = private.current_parent_id()
  or (
    private.current_user_role() = 'School Admin'
    and private.learner_school(learner_id) = private.current_school_id()
  )
);

create policy "parent_learners_insert_admin"
on public.parent_learners for insert to authenticated
with check (
  private.can_manage_school(private.learner_school(learner_id))
  and private.parent_school(parent_id) = private.learner_school(learner_id)
);

create policy "parent_learners_update_admin"
on public.parent_learners for update to authenticated
using (private.can_manage_school(private.learner_school(learner_id)))
with check (
  private.can_manage_school(private.learner_school(learner_id))
  and private.parent_school(parent_id) = private.learner_school(learner_id)
);

drop policy if exists "classes_select" on public.classes;
create policy "classes_select"
on public.classes for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_assigned_to_class(id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_has_learner_in_class(id)
  )
);

drop policy if exists "enrollments_select" on public.enrollments;
drop policy if exists "enrollments_insert_admin" on public.enrollments;
drop policy if exists "enrollments_update_admin" on public.enrollments;

create policy "enrollments_select"
on public.enrollments for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and private.class_school(class_id) = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_assigned_to_class(class_id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_linked_learner(learner_id)
  )
);

create policy "enrollments_insert_admin"
on public.enrollments for insert to authenticated
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.learner_school(learner_id) = private.class_school(class_id)
);

create policy "enrollments_update_admin"
on public.enrollments for update to authenticated
using (private.can_manage_school(private.class_school(class_id)))
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.learner_school(learner_id) = private.class_school(class_id)
);

drop policy if exists "attendance_select" on public.attendance;
drop policy if exists "attendance_insert_teacher" on public.attendance;
drop policy if exists "attendance_update_teacher" on public.attendance;

create policy "attendance_select"
on public.attendance for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and private.class_school(class_id) = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_assigned_to_class(class_id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_linked_learner(learner_id)
  )
);

create policy "attendance_insert_teacher"
on public.attendance for insert to authenticated
with check (
  private.current_user_role() = 'Teacher'
  and marked_by = private.current_teacher_id()
  and private.teacher_assigned_to_class(class_id)
  and private.learner_enrolled_in_class(learner_id, class_id)
);

create policy "attendance_update_teacher"
on public.attendance for update to authenticated
using (
  private.current_user_role() = 'Teacher'
  and marked_by = private.current_teacher_id()
  and private.teacher_assigned_to_class(class_id)
  and created_at >= now() - interval '7 days'
)
with check (
  private.current_user_role() = 'Teacher'
  and marked_by = private.current_teacher_id()
  and private.teacher_assigned_to_class(class_id)
  and private.learner_enrolled_in_class(learner_id, class_id)
);

drop policy if exists "grades_select" on public.grades;
drop policy if exists "grades_insert_teacher" on public.grades;
drop policy if exists "grades_update_teacher" on public.grades;

create policy "grades_select"
on public.grades for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and private.class_school(class_id) = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_assigned_to_subject_class(class_id, subject_id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_linked_learner(learner_id)
  )
);

create policy "grades_insert_teacher"
on public.grades for insert to authenticated
with check (
  private.current_user_role() = 'Teacher'
  and teacher_id = private.current_teacher_id()
  and private.teacher_assigned_to_subject_class(class_id, subject_id)
  and private.learner_enrolled_in_class(learner_id, class_id)
);

create policy "grades_update_teacher"
on public.grades for update to authenticated
using (
  private.current_user_role() = 'Teacher'
  and teacher_id = private.current_teacher_id()
  and private.teacher_assigned_to_subject_class(class_id, subject_id)
)
with check (
  private.current_user_role() = 'Teacher'
  and teacher_id = private.current_teacher_id()
  and private.teacher_assigned_to_subject_class(class_id, subject_id)
  and private.learner_enrolled_in_class(learner_id, class_id)
);

drop policy if exists "events_select" on public.events;
create policy "events_select"
on public.events for select to authenticated
using (
  private.is_super_admin()
  or created_by = (select auth.uid())
  or (
    private.current_user_role() in ('School Admin','Teacher')
    and school_id = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Parent'
    and school_id = private.current_school_id()
    and (
      target_type = 'Whole School'
      or (target_type = 'Role' and target_role = 'Parent')
      or (target_type = 'Grade' and private.parent_has_learner_in_grade(target_grade))
      or (target_type = 'Class' and private.parent_has_learner_in_class(target_class_id))
    )
  )
);

-- Remove obsolete helper functions after policies have been switched.
drop function if exists private.student_school(uuid);
drop function if exists private.parent_linked_student(uuid);
drop function if exists private.student_enrolled_in_class(uuid, uuid);
drop function if exists private.teacher_can_access_student(uuid);
drop function if exists private.parent_has_student_in_grade(text);
drop function if exists private.parent_has_student_in_class(uuid);
;
