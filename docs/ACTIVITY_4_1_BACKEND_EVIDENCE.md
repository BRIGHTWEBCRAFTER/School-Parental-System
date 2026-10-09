# Activity 4.1 — Database and Backend Evidence

## Project

Bright Web Crafters — School-Parental System

## Shared Backend

The Flutter mobile application and ASP.NET web application use one shared Supabase PostgreSQL backend.

Supabase project reference:

`jdperpigajvtywmhvecb`

## Database Implementation

Implemented application tables:

- schools
- users
- parents
- teachers
- learners
- parent_learners
- classes
- enrollments
- subjects
- teaching_assignments
- attendance
- grades
- announcements
- announcement_recipients
- events
- notifications
- admin_registration_codes

Supabase Auth provides authentication through `auth.users`.

`public.users.id` references `auth.users.id`.

## Security

Row Level Security is enabled on the application tables.

Role-based access was configured for:

- Super Admin
- School Admin
- Teacher
- Parent

Learner is not an authentication role.

Passwords are not stored in public database tables.

Secret/service-role keys are not stored in the application repository.

## Backend Tests Completed

### Super Admin

Verified:

- can view both test schools
- can view learners across schools
- can view application users across schools

### School Admin

Verified:

- can view own school
- can view own-school learner
- can view same-school users
- cannot insert learner records into another school

### Teacher

Verified:

- can view assigned learner
- can view assigned class
- can view teaching assignment
- can insert attendance for assigned learner
- can insert grade for assigned learner

### Parent

Verified:

- can view linked learner
- can view linked learner attendance
- can view linked learner grades
- can view linked learner class
- cannot insert attendance
- cannot insert grades
- can mark own announcement recipient record as read
- can acknowledge an announcement
- can mark own notification as read

## Integration Test Records

Backend testing includes:

- attendance record for a test learner
- grade record for a test learner
- announcement targeted to a test parent
- announcement recipient record
- notification record
- parent read/acknowledgement update

## Edge Functions

Deployed and source-controlled:

- `create-admin-code`
- `invite-school-user`

## Dummy Data

Safe dummy/test data is included in `supabase/seed.sql`.

Real stakeholder-school data is excluded from the seed.

## Evidence Screenshots To Keep

For the project portfolio, keep screenshots showing:

- Supabase project dashboard
- database tables
- table relationships / foreign keys
- RLS enabled
- representative RLS policies
- authentication users
- deployed Edge Functions
- migration list with Local and Remote timestamps matching
- successful attendance test
- successful grade test
- blocked cross-school School Admin operation
- blocked Parent attendance/grade operation
- successful Parent announcement acknowledgement
- successful Parent notification read update
- GitHub branch and committed backend files

Do not include passwords, database credentials, API secret keys, access tokens, or one-time administrator registration codes in screenshots.
