# School-Parental System — Supabase Backend

This folder contains the shared Supabase backend used by both the Flutter mobile application and the ASP.NET web application for the Bright Web Crafters School-Parental System.

## Group Supabase Project

- Project name: BRIGHTWEBCRAFTER's Project
- Project reference: jdperpigajvtywmhvecb
- Region: West Europe (London)
- Database: PostgreSQL
- Authentication: Supabase Auth
- Application access control: PostgreSQL Row Level Security (RLS)

No passwords, secret/service-role keys, database passwords, access tokens, or one-time registration codes must be committed to GitHub.

## Application Roles

The system supports four authenticated roles:

- Super Admin
- School Admin
- Teacher
- Parent

A Learner is a managed school record and is not an authentication/login role.

## Database

The shared database contains the following application tables:

1. schools
2. users
3. parents
4. teachers
5. learners
6. parent_learners
7. classes
8. enrollments
9. subjects
10. teaching_assignments
11. attendance
12. grades
13. announcements
14. announcement_recipients
15. events
16. notifications
17. admin_registration_codes

The `public.users.id` field links application profiles to `auth.users.id`.

UUID primary keys are used throughout the implemented Supabase database.

## Migration History

The `migrations/` directory contains the migration history fetched from the linked group Supabase project.

Current migration versions:

- 20261008232038 — rebuild_school_parental_core_schema
- 20261008232104 — add_school_parental_rls_helpers
- 20261008232135 — add_school_parental_rls_policies
- 20261008232147 — add_admin_registration_and_protection
- 20261008232249 — lock_down_existing_rls_auto_enable_function
- 20261008233813 — rename_student_terminology_to_learner
- 20261008235647 — enable_pg_net_for_bootstrap

Do not edit already-applied migration files. Future database changes should be added as new migrations.

## Edge Functions

The `functions/` directory contains the source code downloaded from the deployed group Supabase project.

### create-admin-code

Allows an authenticated active Super Admin to create hashed, time-limited, one-time registration codes for administrator registration.

### invite-school-user

Allows an authenticated active Super Admin or School Admin to provision Teacher or Parent accounts through Supabase Auth.

Teachers require an email invitation.

Parents may use email or phone authentication. Phone/SMS authentication requires an SMS provider to be configured in Supabase before it can be tested end-to-end.

## Authentication Design

Credentials are managed only by Supabase Auth.

There are no password fields in the public application tables.

Client applications must use a Supabase publishable key with RLS enabled. Secret keys and service-role credentials must never be embedded in Flutter, browser code, source control, screenshots, or documentation.

## Row Level Security

RLS is enabled across the application tables.

Access is restricted according to the authenticated user's role and school.

Examples of the implemented rules include:

- Super Admin can access data across schools.
- School Admin access is restricted to their school.
- Teachers can access assigned classes and learners.
- Teachers can capture attendance and grades for assigned learners/classes.
- Parents can view information linked to their own learners.
- Parents cannot create grades or attendance records.
- Cross-school access is blocked.

The system uses helper functions in the private schema to support RLS checks while keeping authorization logic centralized.

## Seed Data

`seed.sql` contains safe development data for backend recreation and testing.

It intentionally excludes:

- passwords
- Supabase Auth UUIDs
- API secrets
- access tokens
- one-time registration codes
- real stakeholder-school records

Authentication users must be created or invited separately through Supabase Auth.

## Local CLI Workflow

Login:

```bash
npx supabase login
npx supabase link --project-ref jdperpigajvtywmhvecb
npx supabase migration list
npx supabase migration fetch
npx supabase functions list
npx supabase functions download create-admin-code
npx supabase functions download invite-school-user

```

Do not run destructive commands against the linked group project.

In particular, do not use `db reset --linked`.

## Current Backend Verification

Backend verification completed with test records for the four authenticated roles.

Verified behaviour includes:

- Super Admin cross-school visibility
- School Admin same-school visibility
- cross-school School Admin insertion blocked
- Teacher assigned-learner access
- Teacher attendance insertion
- Teacher grade insertion
- Parent linked-learner visibility
- Parent attendance creation blocked
- Parent grade creation blocked
- Parent announcement read/acknowledgement update
- Parent notification read update

This backend is shared by the Flutter mobile app and ASP.NET web app.
