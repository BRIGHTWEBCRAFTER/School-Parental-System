-- Bright Web Crafters safe development seed.
-- No passwords, secret keys, Auth UUIDs, or real stakeholder records are included.
-- Reitzpark, Lenakeng and Thabong are intentionally excluded.

insert into public.schools (name, address, email)
values
  ('Goldfield High School', 'Test address, Welkom, Free State', 'goldfield.test@brightwebcrafters.invalid'),
  ('St Dominic''s College High School', 'Test address, Welkom, Free State', 'stdominics.test@brightwebcrafters.invalid')
on conflict (email) do update
set name = excluded.name,
    address = excluded.address;

with goldfield as (
  select id from public.schools
  where email = 'goldfield.test@brightwebcrafters.invalid'
  limit 1
)
insert into public.learners (
  admission_number, full_name, date_of_birth, school_id, grade, address, is_active
)
select 'GF-2026-001', 'Sandile Motone', date '2010-06-15', id,
       'Grade 10', 'Welkom, Free State', true
from goldfield
on conflict (school_id, admission_number) do update
set full_name = excluded.full_name,
    date_of_birth = excluded.date_of_birth,
    grade = excluded.grade,
    address = excluded.address,
    is_active = true;

with st_dominics as (
  select id from public.schools
  where email = 'stdominics.test@brightwebcrafters.invalid'
  limit 1
)
insert into public.learners (
  admission_number, full_name, date_of_birth, school_id, grade, address, is_active
)
select 'SD-2026-001', 'Lerato Mokoena', date '2011-03-20', id,
       'Grade 9', 'Welkom, Free State', true
from st_dominics
on conflict (school_id, admission_number) do update
set full_name = excluded.full_name,
    date_of_birth = excluded.date_of_birth,
    grade = excluded.grade,
    address = excluded.address,
    is_active = true;

with goldfield as (
  select id from public.schools
  where email = 'goldfield.test@brightwebcrafters.invalid'
  limit 1
)
insert into public.classes (
  name, code, school_id, grade, section, room_number,
  capacity, academic_year, is_active
)
select 'Grade 10 A', 'GF-10A', id, 'Grade 10', 'A', 'A10',
       35, 2026, true
from goldfield
on conflict (school_id, code, academic_year) do update
set name = excluded.name,
    grade = excluded.grade,
    section = excluded.section,
    room_number = excluded.room_number,
    capacity = excluded.capacity,
    is_active = true;

with goldfield as (
  select id from public.schools
  where email = 'goldfield.test@brightwebcrafters.invalid'
  limit 1
)
insert into public.subjects (
  school_id, name, code, description, is_active
)
select id, 'Mathematics', 'GF-MATH',
       'Test subject for backend verification', true
from goldfield
on conflict (school_id, code) do update
set name = excluded.name,
    description = excluded.description,
    is_active = true;

insert into public.enrollments (
  learner_id, class_id, enrollment_date, academic_year, is_active
)
select l.id, c.id, date '2026-01-15', 2026, true
from public.learners l
join public.schools s on s.id = l.school_id
join public.classes c on c.school_id = s.id
where s.email = 'goldfield.test@brightwebcrafters.invalid'
  and l.admission_number = 'GF-2026-001'
  and c.code = 'GF-10A'
  and c.academic_year = 2026
on conflict (learner_id, class_id, academic_year) do update
set is_active = true,
    end_date = null;

-- Auth-linked users are deliberately NOT seeded here.
-- Create/invite Super Admin, School Admin, Teacher and Parent through Supabase Auth.
-- Their UUIDs are environment-specific and should not be hard-coded into Git.
