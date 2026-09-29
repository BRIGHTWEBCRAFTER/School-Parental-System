# School-Parental System

![Status](https://img.shields.io/badge/Status-Phase%201%20Complete-brightgreen)
![Phase](https://img.shields.io/badge/Phase-1%20Requirements-blue)
![Module](https://img.shields.io/badge/Module-ITC327W-orange)
![Year](https://img.shields.io/badge/Year-2026-purple)

---

## PROJECT OVERVIEW

The School-Parental System is a centralized, web and mobile-based platform designed to streamline school management and improve communication between schools and parents.

### Problem Statement

Most schools currently lack a centralized, accessible, and easy-to-use system for tracking and managing student records, attendance, grades, and parent communication. Student-related data is scattered across paper registers, spreadsheets, email records, and manual filing systems, making it difficult to maintain accuracy, track student progress, or plan academic activities effectively.

Manual tracking of attendance, grades, and parent communication is:

- Inefficient
- Error-prone
- Lacks real-time insights
- Leads to poor decision-making
- Slower communication
- Difficult to intervene before it is too late

### Who Is Affected

| Stakeholder | Problem |
|-------------|---------|
| School Administrators | Spend excessive time on manual administrative tasks |
| Teachers | Burdened with record-keeping instead of focusing on teaching |
| Parents | Lack real-time visibility into their children's academic progress |
| Students | Experience delays in receiving feedback; at risk of dropout or suspension without parent knowledge |

### Our Solution

A centralized platform that provides:

- Real-time student information access
- Efficient attendance and grade management
- Direct parent-school communication
- Early intervention for at-risk students
- Multi-school support for Super Admins

---

## GROUP INFORMATION

| Field | Details |
|-------|---------|
| Group Name | Bright Web Crafters |
| Module | ITC327W |
| Institution | Central University of Technology (CUT) |
| Phase | Phase 1 - Requirements Analysis |
| Year | 2026 |
| Repository | https://github.com/BRIGHTWEBCRAFTER/School-Parental-System |

### Team Members

| Name | Student Number | Role |
|------|----------------|------|
| [Name 1] | [Number] | Project Manager |
| [Name 2] | [Number] | Backend Developer |
| [Name 3] | [Number] | Frontend Developer |
| [Name 4] | [Number] | UI/UX Designer |
| [Name 5] | [Number] | Database Administrator |
| [Name 6] | [Number] | Quality Assurance |

---

## PROJECT AIM

The aim of the School-Parental System is to design and develop a centralized web and mobile platform that improves the management of student information, attendance, grades, and communication between school administrators, teachers, and parents.

The system provides secure role-based access to information while improving operational efficiency, data accuracy, and stakeholder communication.

---

## PROJECT OBJECTIVES

1. Centralize student, teacher, attendance, and grade information within a single system
2. Enable School Administrators to manage students, teachers, and classes digitally
3. Allow teachers to record attendance and student grades electronically
4. Provide parents with access to their children's attendance, grades, and school communication
5. Improve communication between schools and parents through announcements, notifications, and events
6. Implement secure role-based access control for Super Admins, School Admins, Teachers, and Parents
7. Support multi-school management through a centralized platform
8. Integrate Flutter, ASP.NET Core, and Supabase into a single solution
9. Improve the accuracy, accessibility, and availability of school information

---

## PROJECT SCOPE

### What the System Enables

| Role | Capabilities |
|------|--------------|
| Super Admin | Manage multiple schools from a single dashboard |
| School Admin | Manage students, teachers, classes, attendance, and grades for their school |
| Teacher | Mark attendance, record grades, and view assigned classes |
| Parent | View children's attendance, grades, and school announcements |

### Deliverables

- Software Requirements Specification (SRS)
- System design documentation
- UML diagrams
- Entity Relationship Diagram (ERD)
- Flutter mobile application prototype
- ASP.NET Core Web API
- Supabase PostgreSQL Database
- Authentication and Role-Based Access Control Implementation
- Testing Documentation
- Final Project Presentation

### Acceptance Criteria

The prototype will be considered acceptable when:

- Users can successfully authenticate and access the system
- Role-Based Access Control functions correctly
- School Administrators can manage students, teachers, and classes
- Teachers can record attendance and grades
- Parents can view authorized student information
- Communication features function correctly
- Flutter and ASP.NET Core successfully integrate with Supabase
- System security requirements are satisfied

### Exclusions

The initial prototype will NOT include:

- School fee payment processing
- Payroll management
- Library management
- Transport tracking
- Online classroom functionality
- Integration with external government education systems
- Full production deployment

### Constraints

- Must be completed within the academic semester
- Development must use Flutter, ASP.NET Core, and Supabase
- Developed by a student team with limited resources
- Internet connectivity is required for system operation
- Academic prototype rather than a production system

### Assumptions

- Stakeholders will provide the information required for analysis and design
- Users will have access to internet-enabled devices
- Schools maintain accurate student and staff records
- Parents are linked only to their own children
- Users possess basic computer or smartphone literacy

---

## STAKEHOLDER PROFILE

### Stakeholders Identified

| # | Stakeholder | Role |
|---|-------------|------|
| 1 | Participating Schools | Primary stakeholder and source of requirements |
| 2 | Super Admin | Manages multiple schools and system-wide information |
| 3 | School Admin | Manages students, teachers, classes, and school information |
| 4 | Teacher | Records attendance, grades, and manages assigned classes |
| 5 | Parent | Views children's information and school communication |
| 6 | Learners | Beneficiaries of improved school administration and communication |
| 7 | Development Team | Responsible for analysis, design, development, and testing |

### Stakeholder Engagement

Interviews and requirements gathering were conducted with administrators and educators from:

- Reitzpark Primary School (Welkom, Free State)
- Lenakeng Technical School (Welkom, Free State)
- Thabong Primary School (Welkom, Free State)

### Key Findings from Stakeholder Interviews

| Problem Identified | Evidence from Stakeholders |
|--------------------|---------------------------|
| Manual and paper-based record keeping | Paper registers, SA-SAMS, manual filing |
| Learners not delivering documents to parents | Letters do not reach parents |
| Difficulty confirming communication received | "We don't know if parents received the circular" |
| Parents not responding to messages | Wrong numbers, changed contact details |
| Outdated parent contact information | "Parents change numbers and do not inform the school" |
| Duplication and difficulty locating information | Information scattered across systems |
| Dependence on multiple communication methods | WhatsApp, Facebook, letters, phone calls |

---

## CURRENT PROCESS

Schools currently manage student information through a combination of:

- Paper registers
- Spreadsheets
- Email communication
- Manual filing systems
- WhatsApp groups
- Facebook pages
- SA-SAMS (South African School Administration and Management System)

### Problems with Current Process

| Issue | Impact |
|-------|--------|
| Separate systems for different tasks | Duplication of work |
| Manual data capture | Errors and delays |
| Delayed access to information | Poor decision-making |
| Limited parent visibility | Parents unaware of issues until too late |
| Difficulty confirming communication | No proof of receipt |
| Outdated contact information | Communication failures |

---

## SYSTEM OVERVIEW

### Product Perspective

The School-Parental System is an integrated platform designed to allow educational institutions to manage student records, attendance, grades, and parent communication efficiently. The system supports multiple schools from a single platform with role-based access control.

### Product Functions

- User Registration and Authentication (Email/Password)
- Role-Based Access Control (Super Admin, School Admin, Teacher, Parent)
- Multi-School Management (Super Admin only)
- Student Profile Management
- Teacher Management
- Class Management
- Attendance Tracking
- Grade Management
- Communication Management
- Announcements
- Notifications
- Events
- Read/Unread Tracking
- Parent Acknowledgement
- School Selector (Super Admin only)
- Report Generation

### System Environment

| Component | Technology |
|-----------|------------|
| Web API Backend | ASP.NET Core 8 |
| Mobile Application | Flutter (Android and iOS) |
| Database | Supabase PostgreSQL with Row Level Security |
| Authentication | Supabase Auth |
| Storage | Supabase Storage (profile pictures, documents) |

---

## BUSINESS RULES

### User Authentication

- Only registered and verified users can access the system
- Authentication is handled through Supabase Auth with email/password
- JWT tokens are used for session management

### Role-Based Access Control

| Role | Permissions |
|------|-------------|
| Super Admin | Can view, add, edit, and delete any school's data |
| School Admin | Can view, add, edit, and delete data for their school only |
| Teacher | Can view and manage their assigned classes only |
| Parent | Can view only their children's information |

### School Context Rules

- All data must be filtered based on the selected school context
- Super Admin must select a school to view data
- School Admin, Teacher, and Parent are automatically assigned to their school
- School selector is only visible to Super Admin

### Data Validation Rules

- All student records must include: full name, grade, and school
- All teachers must be assigned to a school
- Attendance status must be one of: Present, Absent, Late
- Grades must be between 0 and 100
- Announcements must have a title and content

### Attendance Rules

- Attendance can only be marked by teachers for their assigned classes
- Attendance records cannot be changed after 7 days
- Parents can only view attendance for their children

### Grade Rules

- Grades can only be recorded by teachers for their assigned classes
- Parents can only view grades for their children
- Grade history must be maintained

### Communication Rules

- Communication must be associated with a school
- Users shall only receive communication intended for their school and role
- Announcements may target the whole school, specific roles, grades, classes, or specific users
- Announcements may have a priority of Normal, Important, or Urgent
- Important or Urgent announcements may require Parent Acknowledgement
- Read status and acknowledgement status shall be stored separately
- A parent shall not acknowledge the same announcement more than once

---

## FUNCTIONAL REQUIREMENTS

### Super Admin Interface

| # | Requirement | Priority |
|---|-------------|----------|
| SA1 | Super Admin can register and login using their credentials | High |
| SA2 | Super Admin can view all schools in the system | High |
| SA3 | Super Admin can add new schools | High |
| SA4 | Super Admin can edit school details | High |
| SA5 | Super Admin can delete schools | High |
| SA6 | Super Admin can view all users across all schools | High |
| SA7 | Super Admin can view reports and dashboards | High |
| SA8 | Super Admin can select a school to view specific data | High |
| SA9 | Super Admin can create announcements for permitted schools | Medium |
| SA10 | Super Admin can create and manage events | Medium |
| SA11 | Super Admin can view communication statistics | Medium |

### School Admin Interface

| # | Requirement | Priority |
|---|-------------|----------|
| SA12 | School Admin can register and login | High |
| SA13 | School Admin can view only their school's data | High |
| SA14 | School Admin can add students to their school | High |
| SA15 | School Admin can edit student details | High |
| SA16 | School Admin can add teachers to their school | High |
| SA17 | School Admin can edit teacher details | High |
| SA18 | School Admin can create classes | High |
| SA19 | School Admin can assign students to classes | High |
| SA20 | School Admin can view attendance reports | Medium |
| SA21 | School Admin can view grade reports | Medium |
| SA22 | School Admin can create and manage announcements | High |
| SA23 | School Admin can create and manage school events | Medium |
| SA24 | School Admin can require Parent Acknowledgement | Medium |
| SA25 | School Admin can view read/unread statistics | Medium |

### Teacher Interface

| # | Requirement | Priority |
|---|-------------|----------|
| T1 | Teacher can register and login | High |
| T2 | Teacher can view only their school's data | High |
| T3 | Teacher can view their assigned classes | High |
| T4 | Teacher can view students in their classes | High |
| T5 | Teacher can mark attendance for their classes | High |
| T6 | Teacher can view attendance records | High |
| T7 | Teacher can record grades for students | High |
| T8 | Teacher can edit grades | High |
| T9 | Teacher can view grade reports for their classes | Medium |
| T10 | Teacher can create announcements for assigned classes | Medium |
| T11 | Teacher can view communication related to assigned classes | Medium |
| T12 | Teacher can create class-related events where permitted | Medium |

### Parent Interface

| # | Requirement | Priority |
|---|-------------|----------|
| P1 | Parent can register and login | High |
| P2 | Parent can view only their school's data | High |
| P3 | Parent can view only their children's information | High |
| P4 | Parent can view their children's attendance | High |
| P5 | Parent can view their children's grades | High |
| P6 | Parent can view school announcements | High |
| P7 | Parent can view their children's class information | Medium |
| P8 | Parent can receive and view notifications | High |
| P9 | Parent can view upcoming events | Medium |
| P10 | Parent can identify read and unread communication | High |
| P11 | Parent can acknowledge announcements | High |

---

## NON-FUNCTIONAL REQUIREMENTS

### Performance Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR1 | Page load times shall be under 3 seconds | High |
| NFR2 | The system shall support at least 100 concurrent users | High |
| NFR3 | Data synchronization shall occur within 5 seconds | High |
| NFR4 | API response time shall be under 500ms | Medium |

### Security Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR5 | All data transmission shall use HTTPS | High |
| NFR6 | Passwords shall be encrypted using hashing algorithms | High |
| NFR7 | JWT tokens shall expire after 24 hours | High |
| NFR8 | Row Level Security (RLS) shall be enforced at database level | High |
| NFR9 | User input shall be validated to prevent SQL injection | High |

### Usability Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR10 | The interface shall follow Material Design guidelines | Medium |
| NFR11 | The system shall be responsive across different screen sizes | Medium |
| NFR12 | Error messages shall be clear and user-friendly | Medium |
| NFR13 | The system shall provide loading indicators during operations | Medium |

### Reliability Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR14 | The system shall have 99% uptime | High |
| NFR15 | The system shall handle errors gracefully | High |
| NFR16 | The system shall maintain data integrity | High |

### Maintainability Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR17 | Code shall follow industry best practices | Medium |
| NFR18 | Code shall be properly documented | Medium |
| NFR19 | The system shall use version control (GitHub) | High |

### Communication Requirements

| # | Requirement | Priority |
|---|-------------|----------|
| NFR20 | Communication shall only be accessible to authorized users | Medium |
| NFR21 | Unread communication shall be visually distinguished | Medium |
| NFR22 | Important and Urgent announcements shall be visually distinguished | Medium |
| NFR23 | The system shall provide clear feedback for Parent Acknowledgement | High |

---

## DATA REQUIREMENTS

### Data Entities

| # | Entity | Key Fields |
|---|--------|------------|
| 1 | Schools | id, name, address, phone, email, logo_url, school_code, created_at |
| 2 | Users | id, email, full_name, role, school_id, phone, is_active, created_at |
| 3 | Students | id, admission_number, full_name, date_of_birth, gender, school_id, grade, parent_id, address |
| 4 | Teachers | id, employee_id, user_id, school_id, specialization, joining_date |
| 5 | Classes | id, name, code, school_id, grade, section, teacher_id, room_number, capacity |
| 6 | Attendance | id, student_id, class_id, date, status, remarks, marked_by |
| 7 | Grades | id, student_id, subject, assessment_name, assessment_type, score, grade_letter |
| 8 | Announcements | id, school_id, created_by, title, content, priority, target_type, acknowledgement_required |
| 9 | Announcement_Recipients | id, announcement_id, user_id, is_read, read_at, is_acknowledged, acknowledged_at |
| 10 | Notifications | id, user_id, title, message, notification_type, is_read, read_at |
| 11 | Events | id, school_id, created_by, event_name, description, event_date, venue |

### Storage Requirements

- User profile pictures - Supabase Storage
- Student documents - Supabase Storage
- Database - Supabase PostgreSQL
- Regular backups - Maintained by Supabase

---

## EXTERNAL REQUIREMENTS

### User Interfaces

- Responsive web interface for modern browsers
- Mobile app interface developed using Flutter
- Role-specific dashboards
- Admin interface with data visualizations
- Usability and accessibility best practices

### Software Interfaces

- Mobile app and web interface communicate with RESTful API
- Backend API provides endpoints for authentication and data operations
- Backend interacts with Supabase PostgreSQL for storage
- Supabase Auth for authentication

### Communication Interfaces

- HTTPS for all client-backend communication
- JWT tokens for session management
- HTTP POST/GET for file uploads
- Real-time synchronization with backend

---

## RISK ANALYSIS

### 1. Data Breach / Unauthorized Access

| Aspect | Details |
|--------|---------|
| Mitigation | Supabase RLS policies, JWT token validation, HTTPS encryption |
| Contingency | Revoke keys, force password resets, incident response |

### 2. Multi-School Data Leakage

| Aspect | Details |
|--------|---------|
| Mitigation | Strict RLS policies enforcing school_id filtering |
| Contingency | Audit logs review, patch security rules |

### 3. Incorrect Role Enforcement

| Aspect | Details |
|--------|---------|
| Mitigation | Automated RBAC tests, server-side validation |
| Contingency | Rollback, patch rules, re-audit permissions |

### 4. Integration Failures

| Aspect | Details |
|--------|---------|
| Mitigation | Retry logic, test environments, error handling |
| Contingency | Manual data entry fallback, rollback |

### 5. Network Dependency

| Aspect | Details |
|--------|---------|
| Mitigation | Offline caching, status indicators |
| Contingency | Notify users of sync status |

### 6. Browser or Device Compatibility

| Aspect | Details |
|--------|---------|
| Mitigation | Cross-platform testing |
| Contingency | Quick patches or workarounds |

### 7. Outdated Parent Contact Information

| Aspect | Details |
|--------|---------|
| Mitigation | Allow staff to update contact info, regular verification |
| Contingency | Verify with learner, use alternative methods |

### 8. Parents Not Reading Important Communication

| Aspect | Details |
|--------|---------|
| Mitigation | Notifications, Read/Unread Tracking, Parent Acknowledgement |
| Contingency | Follow up via phone calls or printed notices |

---

## FEASIBILITY STUDY

### Technical Feasibility

Decision: Feasible with changes

Required technologies (Flutter, ASP.NET Core, Supabase) can support the prototype. Security and integration are complex but manageable.

### Operational Feasibility

Decision: Feasible with changes

Solves confirmed school problems. Keep alternative communication methods for parents with limited access.

### Economic Feasibility

Decision: Feasible

All required resources are available (internet, computers, test devices, development tools).

### Schedule Feasibility

Decision: Feasible

Time allocated for design, development, integration, testing, and documentation.

### Legal Feasibility

Decision: Feasible

POPIA compliance, RLS, RBAC, HTTPS, no credentials in GitHub.

### Ethical Feasibility

Decision: Feasible

Educational prototype, only necessary information collected, user roles restricted.

### Overall Conclusion

The proposed School-Parental System is feasible with changes. Main strengths: addresses confirmed problems, uses available technologies. Main limitations: large scope, integration complexity, internet dependency.

---

## SWOT ANALYSIS

| Strengths | Weaknesses |
|-----------|------------|
| Addresses confirmed stakeholder problems | Large scope with many modules |
| Centralized platform for all school data | Integration between Flutter, ASP.NET, Supabase is complex |
| Integrated technology solution | Depends on internet connectivity |
| Improves parent-school communication | Some parents may have limited smartphone access |
| Role-based access control | Academic prototype, not production-ready |

| Opportunities | Threats |
|---------------|---------|
| Reduce dependence on paper-based processes | Data breaches or unauthorized access |
| Improve parent access to information | Incorrect role enforcement |
| Expand in future phases | Poor network connectivity |
| | Outdated parent contact information |
| | Project delays due to scope |

---

## PROJECT RISK REGISTER

| Risk ID | Risk | Likelihood | Impact | Score | Level | Mitigation |
|---------|------|------------|--------|-------|-------|------------|
| R01 | Unauthorized access to confidential info | 2 | 3 | 6 | Medium | Enforce RLS, RBAC, HTTPS, JWT |
| R02 | Multi-school data leakage | 2 | 3 | 6 | Medium | Enforce school_id filtering, RLS |
| R03 | Incorrect role enforcement | 2 | 3 | 6 | Medium | Test every role, server-side validation |
| R04 | Integration delays | 3 | 3 | 9 | High | Build and test integration early |
| R05 | Network problems | 2 | 2 | 4 | Medium | Connection status, offline caching |
| R06 | Browser/device issues | 2 | 2 | 4 | Medium | Cross-platform testing |
| R07 | Outdated parent contact info | 3 | 2 | 6 | Medium | Allow staff to update, regular verification |
| R08 | Parents not reading communication | 3 | 2 | 6 | Medium | Notifications, Read/Unread, Acknowledgement |
| R09 | Limited parent smartphone access | 2 | 3 | 6 | Medium | Simple app, alternative communication |
| R10 | Scope delays prototype completion | 3 | 3 | 9 | High | Prioritize core, simplify lower-priority |

---

## REQUIREMENTS CHANGE LOG

| ID | Original Requirement | Change Made | Reason |
|----|---------------------|-------------|--------|
| CH01 | Basic announcement functionality | Expanded to Communication Management | Stakeholder engagement confirmed problems |
| CH02 | Announcements only | Added Notifications and Events | Schools use multiple communication forms |
| CH03 | No communication confirmation | Added Read/Unread Tracking | Difficulty confirming receipt |
| CH04 | No acknowledgement mechanism | Added Parent Acknowledgement | Allow schools to confirm receipt |
| CH05 | Important Notices as separate type | Combined with Announcement priority | Reduce duplicate functionality |
| CH06 | Assumed mobile reaches all parents | Added connectivity/device constraint | Some parents lack smartphones/data |
| CH07 | Assumed contact info is current | Added requirement to maintain contact details | Outdated contact info identified as problem |
| CH08 | Broad proposed system | Added requirement prioritisation | Feasibility and schedule analysis |

---

## REFINED PROJECT SCOPE

Following the feasibility study and risk analysis, the project prioritises:

### Core Functionality (Phase 1 Priority)

- Authentication
- Role-Based Access Control
- School Management
- Student Management
- Teacher Management
- Class Management
- Attendance
- Grades
- Parent Access
- School Announcements

### Secondary Functionality (Phase 2 Priority)

- Notifications
- Events
- Parent Acknowledgement
- Communication Statistics
- Advanced Reporting

### Excluded from Prototype

- Payment processing
- Payroll
- Library management
- Transport tracking
- Online classroom
- Government system integration

---

## REFERENCES

1. Microsoft. (2024). ASP.NET Core Documentation. https://learn.microsoft.com/en-us/aspnet/core/
2. Flutter. (2024). Flutter Documentation. https://docs.flutter.dev/
3. Supabase. (2024). Supabase Documentation. https://supabase.com/docs
4. PostgreSQL. (2024). PostgreSQL Documentation. https://www.postgresql.org/docs/
5. Protection of Personal Information Act (POPIA). https://popia.co.za/
6. IEEE. (1998). IEEE Std 830-1998: IEEE Recommended Practice for Software Requirements Specifications.

---

## AI USE

Artificial Intelligence (AI) tools were used during the preparation of this document to assist with:

- Structuring the SRS according to industry standard formats
- Rewording sentences for clarity, conciseness, and formal technical writing style
- Suggesting appropriate terminology, acronyms, and definitions
- Organizing content based on the reference document structure

All AI-generated content was reviewed, validated, and modified by all group members to ensure accuracy, relevance, and compliance with project requirements.

---

## CONTACT

| Field | Details |
|-------|---------|
| Group Email | brightwebcrafters@cut.ac.za |
| Institution | Central University of Technology |
| Module | ITC327W |
| Repository | https://github.com/BRIGHTWEBCRAFTER/School-Parental-System |

---

## LICENSE

This project is developed as part of an academic requirement at the Central University of Technology (CUT).

Academic Use Only

Copyright 2026 Bright Web Crafters

---

## ACKNOWLEDGEMENTS

- Reitzpark Primary School
- Lenakeng Technical School
- Thabong Primary School



Phase 1 Status: Complete

Next Phase: Phase 2 - System Architecture and Design
