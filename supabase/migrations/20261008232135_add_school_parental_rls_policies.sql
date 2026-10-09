
revoke all on all tables in schema public from anon;

grant select, update on public.schools to authenticated;
grant select on public.users to authenticated;
grant select, insert, update on public.parents to authenticated;
grant select, insert, update on public.teachers to authenticated;
grant select, insert, update on public.students to authenticated;
grant select, insert, update on public.parent_students to authenticated;
grant select, insert, update on public.classes to authenticated;
grant select, insert, update on public.enrollments to authenticated;
grant select, insert, update on public.subjects to authenticated;
grant select, insert, update on public.teaching_assignments to authenticated;
grant select, insert, update on public.attendance to authenticated;
grant select, insert, update on public.grades to authenticated;
grant select, insert, update on public.announcements to authenticated;
grant select, update on public.announcement_recipients to authenticated;
grant select, insert, update on public.events to authenticated;
grant select, update on public.notifications to authenticated;
revoke all on public.admin_registration_codes from anon, authenticated;

create policy "schools_select"
on public.schools for select to authenticated
using (private.is_super_admin() or id = private.current_school_id());

create policy "schools_update_super_admin"
on public.schools for update to authenticated
using (private.is_super_admin())
with check (private.is_super_admin());

create policy "users_select"
on public.users for select to authenticated
using (
  id = (select auth.uid())
  or private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
);

create policy "parents_select"
on public.parents for select to authenticated
using (
  private.is_super_admin()
  or user_id = (select auth.uid())
  or (
    private.current_user_role() = 'School Admin'
    and private.parent_school(id) = private.current_school_id()
  )
);

create policy "parents_insert_admin"
on public.parents for insert to authenticated
with check (
  private.can_manage_school(private.user_school(user_id))
  and private.user_role(user_id) = 'Parent'
);

create policy "parents_update_admin"
on public.parents for update to authenticated
using (private.can_manage_school(private.parent_school(id)))
with check (
  private.can_manage_school(private.user_school(user_id))
  and private.user_role(user_id) = 'Parent'
);

create policy "teachers_select"
on public.teachers for select to authenticated
using (
  private.is_super_admin()
  or user_id = (select auth.uid())
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
);

create policy "teachers_insert_admin"
on public.teachers for insert to authenticated
with check (
  private.can_manage_school(school_id)
  and private.user_school(user_id) = school_id
  and private.user_role(user_id) = 'Teacher'
);

create policy "teachers_update_admin"
on public.teachers for update to authenticated
using (private.can_manage_school(school_id))
with check (
  private.can_manage_school(school_id)
  and private.user_school(user_id) = school_id
  and private.user_role(user_id) = 'Teacher'
);

create policy "students_select"
on public.students for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
  or (
    private.current_user_role() = 'Teacher'
    and private.teacher_can_access_student(id)
  )
  or (
    private.current_user_role() = 'Parent'
    and private.parent_linked_student(id)
  )
);

create policy "students_insert_admin"
on public.students for insert to authenticated
with check (private.can_manage_school(school_id));

create policy "students_update_admin"
on public.students for update to authenticated
using (private.can_manage_school(school_id))
with check (private.can_manage_school(school_id));

create policy "parent_students_select"
on public.parent_students for select to authenticated
using (
  private.is_super_admin()
  or parent_id = private.current_parent_id()
  or (
    private.current_user_role() = 'School Admin'
    and private.student_school(student_id) = private.current_school_id()
  )
);

create policy "parent_students_insert_admin"
on public.parent_students for insert to authenticated
with check (
  private.can_manage_school(private.student_school(student_id))
  and private.parent_school(parent_id) = private.student_school(student_id)
);

create policy "parent_students_update_admin"
on public.parent_students for update to authenticated
using (private.can_manage_school(private.student_school(student_id)))
with check (
  private.can_manage_school(private.student_school(student_id))
  and private.parent_school(parent_id) = private.student_school(student_id)
);

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
    and private.parent_has_student_in_class(id)
  )
);

create policy "classes_insert_admin"
on public.classes for insert to authenticated
with check (
  private.can_manage_school(school_id)
  and (class_teacher_id is null or private.teacher_school(class_teacher_id) = school_id)
);

create policy "classes_update_admin"
on public.classes for update to authenticated
using (private.can_manage_school(school_id))
with check (
  private.can_manage_school(school_id)
  and (class_teacher_id is null or private.teacher_school(class_teacher_id) = school_id)
);

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
    and private.parent_linked_student(student_id)
  )
);

create policy "enrollments_insert_admin"
on public.enrollments for insert to authenticated
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.student_school(student_id) = private.class_school(class_id)
);

create policy "enrollments_update_admin"
on public.enrollments for update to authenticated
using (private.can_manage_school(private.class_school(class_id)))
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.student_school(student_id) = private.class_school(class_id)
);

create policy "subjects_select"
on public.subjects for select to authenticated
using (private.is_super_admin() or school_id = private.current_school_id());

create policy "subjects_insert_admin"
on public.subjects for insert to authenticated
with check (private.can_manage_school(school_id));

create policy "subjects_update_admin"
on public.subjects for update to authenticated
using (private.can_manage_school(school_id))
with check (private.can_manage_school(school_id));

create policy "teaching_assignments_select"
on public.teaching_assignments for select to authenticated
using (
  private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and private.class_school(class_id) = private.current_school_id()
  )
  or teacher_id = private.current_teacher_id()
);

create policy "teaching_assignments_insert_admin"
on public.teaching_assignments for insert to authenticated
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.teacher_school(teacher_id) = private.class_school(class_id)
  and private.subject_school(subject_id) = private.class_school(class_id)
);

create policy "teaching_assignments_update_admin"
on public.teaching_assignments for update to authenticated
using (private.can_manage_school(private.class_school(class_id)))
with check (
  private.can_manage_school(private.class_school(class_id))
  and private.teacher_school(teacher_id) = private.class_school(class_id)
  and private.subject_school(subject_id) = private.class_school(class_id)
);

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
    and private.parent_linked_student(student_id)
  )
);

create policy "attendance_insert_teacher"
on public.attendance for insert to authenticated
with check (
  private.current_user_role() = 'Teacher'
  and marked_by = private.current_teacher_id()
  and private.teacher_assigned_to_class(class_id)
  and private.student_enrolled_in_class(student_id, class_id)
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
  and private.student_enrolled_in_class(student_id, class_id)
);

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
    and private.parent_linked_student(student_id)
  )
);

create policy "grades_insert_teacher"
on public.grades for insert to authenticated
with check (
  private.current_user_role() = 'Teacher'
  and teacher_id = private.current_teacher_id()
  and private.teacher_assigned_to_subject_class(class_id, subject_id)
  and private.student_enrolled_in_class(student_id, class_id)
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
  and private.student_enrolled_in_class(student_id, class_id)
);

create policy "announcements_select"
on public.announcements for select to authenticated
using (
  private.is_super_admin()
  or created_by = (select auth.uid())
  or (
    private.current_user_role() = 'School Admin'
    and school_id = private.current_school_id()
  )
  or private.is_announcement_recipient(id)
);

create policy "announcements_insert"
on public.announcements for insert to authenticated
with check (
  created_by = (select auth.uid())
  and (
    private.can_manage_school(school_id)
    or (
      private.current_user_role() = 'Teacher'
      and school_id = private.current_school_id()
      and target_type = 'Class'
      and target_class_id is not null
      and private.teacher_assigned_to_class(target_class_id)
    )
  )
);

create policy "announcements_update"
on public.announcements for update to authenticated
using (
  private.can_manage_school(school_id)
  or (created_by = (select auth.uid()) and private.current_user_role() = 'Teacher')
)
with check (
  created_by = (select auth.uid())
  and (
    private.can_manage_school(school_id)
    or (
      private.current_user_role() = 'Teacher'
      and school_id = private.current_school_id()
      and target_type = 'Class'
      and target_class_id is not null
      and private.teacher_assigned_to_class(target_class_id)
    )
  )
);

create policy "announcement_recipients_select"
on public.announcement_recipients for select to authenticated
using (
  user_id = (select auth.uid())
  or private.is_super_admin()
  or (
    private.current_user_role() = 'School Admin'
    and private.user_school(user_id) = private.current_school_id()
  )
  or exists (
    select 1 from public.announcements a
    where a.id = announcement_id
      and a.created_by = (select auth.uid())
  )
);

create policy "announcement_recipients_update_own"
on public.announcement_recipients for update to authenticated
using (user_id = (select auth.uid()))
with check (user_id = (select auth.uid()));

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
      or (target_type = 'Grade' and private.parent_has_student_in_grade(target_grade))
      or (target_type = 'Class' and private.parent_has_student_in_class(target_class_id))
    )
  )
);

create policy "events_insert"
on public.events for insert to authenticated
with check (
  created_by = (select auth.uid())
  and (
    private.can_manage_school(school_id)
    or (
      private.current_user_role() = 'Teacher'
      and school_id = private.current_school_id()
      and target_type = 'Class'
      and target_class_id is not null
      and private.teacher_assigned_to_class(target_class_id)
    )
  )
);

create policy "events_update"
on public.events for update to authenticated
using (
  private.can_manage_school(school_id)
  or (created_by = (select auth.uid()) and private.current_user_role() = 'Teacher')
)
with check (
  created_by = (select auth.uid())
  and (
    private.can_manage_school(school_id)
    or (
      private.current_user_role() = 'Teacher'
      and school_id = private.current_school_id()
      and target_type = 'Class'
      and target_class_id is not null
      and private.teacher_assigned_to_class(target_class_id)
    )
  )
);

create policy "notifications_select"
on public.notifications for select to authenticated
using (user_id = (select auth.uid()) or private.is_super_admin());

create policy "notifications_update_own"
on public.notifications for update to authenticated
using (user_id = (select auth.uid()))
with check (user_id = (select auth.uid()));
;
