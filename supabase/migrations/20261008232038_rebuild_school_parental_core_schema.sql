
drop table if exists public.profiles cascade;
drop table if exists public.schools cascade;

create table public.schools (
  id uuid primary key default gen_random_uuid(),
  name varchar(150) not null,
  address varchar(255) not null,
  phone varchar(20),
  email varchar(150),
  logo_url varchar(500),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_schools_email unique (email)
);

create table public.users (
  id uuid primary key references auth.users(id),
  email varchar(150),
  full_name varchar(150) not null,
  role varchar(20) not null,
  school_id uuid references public.schools(id),
  phone varchar(20),
  profile_picture varchar(500),
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_users_email unique (email),
  constraint uq_users_phone unique (phone),
  constraint ck_users_role check (role in ('Super Admin','School Admin','Teacher','Parent')),
  constraint ck_users_contact check (email is not null or phone is not null),
  constraint ck_users_school_role check (
    (role = 'Super Admin' and school_id is null)
    or
    (role <> 'Super Admin' and school_id is not null)
  )
);

create table public.parents (
  id uuid primary key default gen_random_uuid(),
  guardian_number varchar(30) not null unique,
  user_id uuid not null unique references public.users(id),
  gender varchar(20),
  alternative_phone varchar(20),
  address varchar(255),
  contact_last_verified date,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now()
);

create table public.teachers (
  id uuid primary key default gen_random_uuid(),
  employee_id varchar(30) not null,
  user_id uuid not null unique references public.users(id),
  school_id uuid not null references public.schools(id),
  date_of_birth date,
  gender varchar(20),
  specialization varchar(100),
  joining_date date,
  alternative_phone varchar(20),
  address varchar(255),
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_teachers_school_employee unique (school_id, employee_id)
);

create table public.students (
  id uuid primary key default gen_random_uuid(),
  admission_number varchar(30) not null,
  full_name varchar(150) not null,
  date_of_birth date,
  gender varchar(20),
  school_id uuid not null references public.schools(id),
  grade varchar(20) not null,
  address varchar(255),
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_students_school_admission unique (school_id, admission_number)
);

create table public.parent_students (
  id uuid primary key default gen_random_uuid(),
  parent_id uuid not null references public.parents(id) on delete cascade,
  student_id uuid not null references public.students(id) on delete cascade,
  relationship varchar(30) not null,
  is_primary_guardian boolean not null default false,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_parent_students_link unique (parent_id, student_id)
);

create table public.classes (
  id uuid primary key default gen_random_uuid(),
  name varchar(50) not null,
  code varchar(20) not null,
  school_id uuid not null references public.schools(id),
  grade varchar(20) not null,
  section varchar(10),
  class_teacher_id uuid references public.teachers(id) on delete set null,
  room_number varchar(20),
  capacity integer,
  academic_year integer not null,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_classes_school_code_year unique (school_id, code, academic_year),
  constraint uq_classes_teacher_year unique (class_teacher_id, academic_year),
  constraint ck_classes_capacity check (capacity is null or capacity > 0)
);

create table public.enrollments (
  id uuid primary key default gen_random_uuid(),
  student_id uuid not null references public.students(id) on delete cascade,
  class_id uuid not null references public.classes(id) on delete cascade,
  enrollment_date date not null,
  academic_year integer not null,
  end_date date,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_enrollments_student_class_year unique (student_id, class_id, academic_year),
  constraint ck_enrollments_dates check (end_date is null or end_date >= enrollment_date)
);

create table public.subjects (
  id uuid primary key default gen_random_uuid(),
  school_id uuid not null references public.schools(id),
  name varchar(100) not null,
  code varchar(20) not null,
  description varchar(255),
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_subjects_school_code unique (school_id, code)
);

create table public.teaching_assignments (
  id uuid primary key default gen_random_uuid(),
  teacher_id uuid not null references public.teachers(id) on delete cascade,
  class_id uuid not null references public.classes(id) on delete cascade,
  subject_id uuid not null references public.subjects(id) on delete cascade,
  assigned_at date not null,
  is_active boolean not null default true,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_teaching_assignment unique (teacher_id, class_id, subject_id)
);

create table public.attendance (
  id uuid primary key default gen_random_uuid(),
  student_id uuid not null references public.students(id) on delete cascade,
  class_id uuid not null references public.classes(id) on delete cascade,
  attendance_date date not null,
  status varchar(20) not null,
  check_in_time time,
  remarks varchar(255),
  marked_by uuid not null references public.teachers(id),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint uq_attendance_student_date_class unique (student_id, class_id, attendance_date),
  constraint ck_attendance_status check (status in ('Present','Absent'))
);

create table public.grades (
  id uuid primary key default gen_random_uuid(),
  student_id uuid not null references public.students(id) on delete cascade,
  subject_id uuid not null references public.subjects(id),
  class_id uuid not null references public.classes(id),
  teacher_id uuid not null references public.teachers(id),
  assessment_name varchar(100) not null,
  assessment_type varchar(50) not null,
  score numeric(5,2) not null,
  grade_letter varchar(5),
  academic_term varchar(20) not null,
  academic_year integer not null,
  remarks varchar(255),
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint ck_grades_score check (score >= 0 and score <= 100)
);

create table public.announcements (
  id uuid primary key default gen_random_uuid(),
  school_id uuid not null references public.schools(id),
  created_by uuid not null references public.users(id),
  title varchar(150) not null,
  content text not null,
  priority varchar(20) not null default 'Normal',
  target_type varchar(30) not null,
  target_role varchar(20),
  target_grade varchar(20),
  target_class_id uuid references public.classes(id) on delete set null,
  acknowledgement_required boolean not null default false,
  expires_at timestamptz,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint ck_announcements_priority check (priority in ('Normal','Important','Urgent')),
  constraint ck_announcements_target_type check (
    target_type in ('Whole School','Role','Grade','Class','Specific Users')
  )
);

create table public.announcement_recipients (
  id uuid primary key default gen_random_uuid(),
  announcement_id uuid not null references public.announcements(id) on delete cascade,
  user_id uuid not null references public.users(id) on delete cascade,
  is_read boolean not null default false,
  read_at timestamptz,
  is_acknowledged boolean not null default false,
  acknowledged_at timestamptz,
  created_at timestamptz not null default now(),
  constraint uq_announcement_recipient unique (announcement_id, user_id)
);

create table public.events (
  id uuid primary key default gen_random_uuid(),
  school_id uuid not null references public.schools(id),
  created_by uuid not null references public.users(id),
  event_name varchar(150) not null,
  description varchar(500),
  event_date date not null,
  start_time time,
  end_time time,
  venue varchar(150),
  target_type varchar(30) not null,
  target_role varchar(20),
  target_grade varchar(20),
  target_class_id uuid references public.classes(id) on delete set null,
  created_at timestamptz not null default now(),
  updated_at timestamptz not null default now(),
  constraint ck_events_target_type check (
    target_type in ('Whole School','Role','Grade','Class')
  ),
  constraint ck_events_times check (
    end_time is null or start_time is null or end_time >= start_time
  )
);

create table public.notifications (
  id uuid primary key default gen_random_uuid(),
  user_id uuid not null references public.users(id) on delete cascade,
  announcement_id uuid references public.announcements(id) on delete cascade,
  event_id uuid references public.events(id) on delete cascade,
  grade_id uuid references public.grades(id) on delete cascade,
  attendance_id uuid references public.attendance(id) on delete cascade,
  title varchar(150) not null,
  message varchar(500) not null,
  notification_type varchar(30) not null,
  is_read boolean not null default false,
  read_at timestamptz,
  created_at timestamptz not null default now(),
  constraint ck_notifications_type check (
    notification_type in ('Announcement','Event','Grade','Attendance','System')
  ),
  constraint ck_notifications_single_source check (
    num_nonnulls(announcement_id, event_id, grade_id, attendance_id) <= 1
  )
);

create table public.admin_registration_codes (
  id uuid primary key default gen_random_uuid(),
  code_hash text not null unique,
  admin_role varchar(20) not null,
  school_id uuid references public.schools(id),
  expires_at timestamptz,
  is_active boolean not null default true,
  used_at timestamptz,
  used_by uuid references public.users(id),
  created_by uuid references public.users(id),
  created_at timestamptz not null default now(),
  constraint ck_admin_reg_codes_role check (
    admin_role in ('Super Admin','School Admin')
  ),
  constraint ck_admin_reg_codes_school_role check (
    (admin_role='Super Admin' and school_id is null)
    or
    (admin_role='School Admin' and school_id is not null)
  )
);

create unique index uq_parent_students_one_primary
on public.parent_students(student_id)
where is_primary_guardian = true and is_active = true;

create index idx_users_school_id on public.users(school_id);
create index idx_parents_user_id on public.parents(user_id);
create index idx_teachers_user_id on public.teachers(user_id);
create index idx_teachers_school_id on public.teachers(school_id);
create index idx_students_school_id on public.students(school_id);
create index idx_parent_students_parent_id on public.parent_students(parent_id);
create index idx_parent_students_student_id on public.parent_students(student_id);
create index idx_classes_school_id on public.classes(school_id);
create index idx_classes_teacher_id on public.classes(class_teacher_id);
create index idx_enrollments_student_id on public.enrollments(student_id);
create index idx_enrollments_class_id on public.enrollments(class_id);
create index idx_subjects_school_id on public.subjects(school_id);
create index idx_teaching_assignments_teacher_id on public.teaching_assignments(teacher_id);
create index idx_teaching_assignments_class_id on public.teaching_assignments(class_id);
create index idx_teaching_assignments_subject_id on public.teaching_assignments(subject_id);
create index idx_attendance_student_id on public.attendance(student_id);
create index idx_attendance_class_id on public.attendance(class_id);
create index idx_attendance_marked_by on public.attendance(marked_by);
create index idx_grades_student_id on public.grades(student_id);
create index idx_grades_subject_id on public.grades(subject_id);
create index idx_grades_class_id on public.grades(class_id);
create index idx_grades_teacher_id on public.grades(teacher_id);
create index idx_announcements_school_id on public.announcements(school_id);
create index idx_announcements_created_by on public.announcements(created_by);
create index idx_announcements_target_class_id on public.announcements(target_class_id);
create index idx_announcement_recipients_announcement_id on public.announcement_recipients(announcement_id);
create index idx_announcement_recipients_user_id on public.announcement_recipients(user_id);
create index idx_events_school_id on public.events(school_id);
create index idx_events_created_by on public.events(created_by);
create index idx_events_target_class_id on public.events(target_class_id);
create index idx_notifications_user_id on public.notifications(user_id);
create index idx_notifications_announcement_id on public.notifications(announcement_id);
create index idx_notifications_event_id on public.notifications(event_id);
create index idx_notifications_grade_id on public.notifications(grade_id);
create index idx_notifications_attendance_id on public.notifications(attendance_id);
create index idx_admin_registration_codes_school_id on public.admin_registration_codes(school_id);
create index idx_admin_registration_codes_used_by on public.admin_registration_codes(used_by);
create index idx_admin_registration_codes_created_by on public.admin_registration_codes(created_by);

create or replace function public.set_updated_at()
returns trigger
language plpgsql
set search_path = public
as $$
begin
  new.updated_at = now();
  return new;
end;
$$;

create trigger trg_schools_updated_at before update on public.schools
for each row execute function public.set_updated_at();
create trigger trg_users_updated_at before update on public.users
for each row execute function public.set_updated_at();
create trigger trg_parents_updated_at before update on public.parents
for each row execute function public.set_updated_at();
create trigger trg_teachers_updated_at before update on public.teachers
for each row execute function public.set_updated_at();
create trigger trg_students_updated_at before update on public.students
for each row execute function public.set_updated_at();
create trigger trg_parent_students_updated_at before update on public.parent_students
for each row execute function public.set_updated_at();
create trigger trg_classes_updated_at before update on public.classes
for each row execute function public.set_updated_at();
create trigger trg_enrollments_updated_at before update on public.enrollments
for each row execute function public.set_updated_at();
create trigger trg_subjects_updated_at before update on public.subjects
for each row execute function public.set_updated_at();
create trigger trg_teaching_assignments_updated_at before update on public.teaching_assignments
for each row execute function public.set_updated_at();
create trigger trg_attendance_updated_at before update on public.attendance
for each row execute function public.set_updated_at();
create trigger trg_grades_updated_at before update on public.grades
for each row execute function public.set_updated_at();
create trigger trg_announcements_updated_at before update on public.announcements
for each row execute function public.set_updated_at();
create trigger trg_events_updated_at before update on public.events
for each row execute function public.set_updated_at();

alter table public.schools enable row level security;
alter table public.users enable row level security;
alter table public.parents enable row level security;
alter table public.teachers enable row level security;
alter table public.students enable row level security;
alter table public.parent_students enable row level security;
alter table public.classes enable row level security;
alter table public.enrollments enable row level security;
alter table public.subjects enable row level security;
alter table public.teaching_assignments enable row level security;
alter table public.attendance enable row level security;
alter table public.grades enable row level security;
alter table public.announcements enable row level security;
alter table public.announcement_recipients enable row level security;
alter table public.events enable row level security;
alter table public.notifications enable row level security;
alter table public.admin_registration_codes enable row level security;
;
