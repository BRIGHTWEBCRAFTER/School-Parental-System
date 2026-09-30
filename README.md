# School-Parental System


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
| Year | 2026 |
| Repository | https://github.com/BRIGHTWEBCRAFTER/School-Parental-System |

### Team Members

| Name | Student Number | Role |
|------|----------------|------|
| L.P Moshoeu | 223046876 | Team Leader |
| A Sithole | 223000460 | Group Member |
| M.A Nkuna | 224000274 | Group Member |
| S.T Pheko | 223050336 | Group Member |
| T.M.C Motone | 224027806 | Group Member |
| P.A Luthada | 222023335 | Group Member |

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
|----|-------------|----------|
| NFR20 | Communication shall only be accessible to authorized users | Medium |
| NFR21 | Unread communication shall be visually distinguished | Medium |
| NFR22 | Important and Urgent announcements shall be visually distinguished | Medium |
| NFR23 | The system shall provide clear feedback for Parent Acknowledgement | High |


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

# PHASE 2 - SYSTEM ARCHITECTURE AND DESIGN

---

## PROJECT PROGRESS - PHASE 2

### Completed

- Phase 1 SRS (requirements, stakeholder engagement, feasibility)
- System Architecture Design
- Use Case Diagram (all actors and relationships)
- Sequence Diagrams (Flutter+Supabase, ASP.NET+Supabase)
- Entity Relationship Diagram (ERD)
- Flutter Mobile Interface Designs
- ASP.NET Web Interface Designs
- Design Alignment Table
- Microsoft Project Schedule
- GitHub Repository Setup
- Supabase Project Setup

### Next Steps (Phase 3)

- Flutter application development
- ASP.NET Core Web API development
- Supabase database implementation (tables, RLS policies)
- Integration testing
- User acceptance testing
- Final presentation preparation

---

## PHASE 2 DELIVERABLES

| # | Deliverable | Location | Status |
|---|-------------|----------|--------|
| 1 | System Architecture Diagram | docs/architecture/ | Complete |
| 2 | Use Case Diagram | docs/uml/ | Complete |
| 3 | Sequence Diagram 1 (Flutter + Supabase) | docs/uml/ | Complete |
| 4 | Sequence Diagram 2 (ASP.NET + Supabase) | docs/uml/ | Complete |
| 5 | Entity Relationship Diagram (ERD) | docs/erd/ | Complete |
| 6 | Flutter Mobile Wireframes | docs/wireframes/flutter/ | Complete |
| 7 | ASP.NET Web Wireframes | docs/wireframes/aspnet/ | Complete |
| 8 | Design Alignment Table | docs/design-alignment/ | Complete |
| 9 | Original Design Source Links | docs/design-sources/ | Complete |
| 10 | Microsoft Project Schedule | project-management/microsoft-project/ | Complete |
| 11 | Project Progress Tracker | project-management/progress-tracker/ | Complete |
| 12 | Individual Contributions | docs/contributions/ | Complete |

---

## SYSTEM ARCHITECTURE

### Three-Tier Architecture

The School-Parental System follows a three-tier architecture that integrates a Flutter mobile application, an ASP.NET Core web application, and a shared Supabase backend.


### Main User Roles

| Role | Primary Interaction |
|------|---------------------|
| Super Admin | ASP.NET Web Application |
| School Admin | ASP.NET Web Application |
| Teacher | ASP.NET Web Application |
| Parent | Flutter Mobile Application and Web Browser |

### Flutter Mobile Application

The Flutter mobile application is the primary interface for parents and guardians. Main responsibilities:

- Parent login using email and password
- Real-time attendance records for children
- Academic grades and assessment results
- School announcements with read/unread status
- Parent acknowledgement for important announcements
- Upcoming school events
- Download term reports
- Notifications for important updates
- Direct messaging with school admin

### ASP.NET Core Web Application

The ASP.NET Core web application serves administrators and teachers. Main responsibilities:

- Super Admin dashboard for managing multiple schools
- School Admin dashboard for managing a single school
- Manage students, teachers, and classes
- Mark attendance and record grades
- Create announcements and events
- Communication statistics and read/acknowledgement tracking
- Generate attendance and grade reports
- Manage user accounts and access codes

### Supabase Backend

| Service | Purpose |
|---------|---------|
| PostgreSQL Database | Stores all system data |
| Supabase Auth | Handles authentication for all user roles |
| Row Level Security (RLS) | Enforces data access rules at the database level |
| Supabase Storage | Stores profile pictures and documents |
| Real-time Subscriptions | Enables real-time updates |

### Data Flow

1. Users interact with the Flutter mobile app or ASP.NET web application
2. The client sends HTTPS requests to the ASP.NET Core Web API
3. The API validates the JWT token and checks user permissions
4. The API queries or updates the Supabase PostgreSQL database
5. Supabase RLS policies enforce data access rules
6. The API returns the response to the client
7. The client displays the data to the user

### Integration Explanation

The three components operate as one integrated system:

- The Flutter app and ASP.NET web app share the same backend API and database
- Supabase Auth provides a single authentication system for all roles
- JWT tokens are used consistently across both clients
- Row Level Security ensures data isolation between schools and roles
- Real-time subscriptions allow instant updates to all connected clients

### Design Source Link

| Design | Tool | Link |
|--------|------|------|
| System Architecture Diagram | Draw.io | [INSERT LINK] |

---

## UML DESIGN

### Use Case Diagram


#### System Boundary

The School-Parental System is clearly labelled as the system boundary, with all use cases placed inside.

#### Actors

| Actor | Description |
|-------|-------------|
| Super Admin | Manages multiple schools and system-wide information |
| School Admin | Manages a single school's data |
| Teacher | Manages assigned classes, attendance, and grades |
| Parent | Views children's information and school communication |

#### Use Cases

| Use Case | Actor | Description |
|----------|-------|-------------|
| Login | All Actors | Authenticate using credentials |
| Manage Schools | Super Admin | Add, edit, or delete schools |
| Manage School Admins | Super Admin | Add, edit, or remove school admins |
| Manage Teachers | School Admin | Add, edit, or remove teachers |
| Manage Students | School Admin | Add, edit, or remove students |
| Manage Classes | School Admin | Create, edit, or assign classes |
| Manage Subjects | School Admin | Create, edit, or remove subjects |
| Mark Attendance | Teacher | Record attendance for assigned classes |
| Record Grades | Teacher | Record grades for assigned students |
| View Attendance | Parent, Teacher, School Admin | View attendance records |
| View Grades | Parent, Teacher, School Admin | View grade records |
| View Announcements | All Actors | View school announcements |
| Send Message | Parent | Send messages to school admin |
| View Messages | Parent, School Admin | View sent and received messages |
| View Reports | School Admin, Super Admin | Generate and view reports |

#### UML Relationships

| Relationship | Where Used |
|--------------|------------|
| <<include>> | Login includes Authenticate |
| <<extend>> | Send Message extends View Messages |
| Generalization | Super Admin and School Admin are specialized Users |

#### Design Source Link

| Design | Tool | Link |
|--------|------|------|
| Use Case Diagram | Draw.io | [INSERT LINK] |

---

### Sequence Diagram 1: Parent Views Attendance (Flutter + Supabase)

#### Participants

| Participant | Role |
|-------------|------|
| Parent | End user |
| Flutter App | Mobile client |
| Supabase Auth | Authentication service |
| Supabase Database | Data storage |

#### Sequence of Interactions

1. Parent opens the Flutter app
2. Parent enters email and password
3. Flutter app sends authentication request to Supabase Auth
4. Supabase Auth validates credentials
5. Supabase Auth returns JWT token
6. Parent requests to view attendance
7. Flutter app sends request to Supabase Database with JWT token
8. Supabase Database validates token and checks RLS policies
9. Supabase Database returns attendance records for the parent's child
10. Flutter app displays attendance records to the parent

#### Requirements Alignment

| Requirement | Use Case |
|-------------|----------|
| P4 - Parent can view children's attendance | View Attendance |

#### Design Source Link

| Design | Tool | Link |
|--------|------|------|
| Sequence Diagram 1 | Draw.io | [INSERT LINK] |

---

### Sequence Diagram 2: Teacher Marks Attendance (ASP.NET + Supabase)


#### Participants

| Participant | Role |
|-------------|------|
| Teacher | End user |
| ASP.NET Web App | Web client |
| ASP.NET Web API | Backend service |
| Supabase Database | Data storage |

#### Sequence of Interactions

1. Teacher logs into the ASP.NET web application
2. ASP.NET Web API authenticates the teacher with Supabase Auth
3. Supabase Auth returns JWT token
4. Teacher selects a class and opens the attendance page
5. Teacher marks each student as Present, Absent, or Late
6. Teacher clicks Save Attendance
7. ASP.NET Web API validates the teacher's role and class assignment
8. ASP.NET Web API sends attendance records to Supabase Database
9. Supabase Database validates RLS policies
10. Supabase Database stores attendance records
11. Supabase Database returns success response
12. ASP.NET Web API returns confirmation to the web app
13. Web app displays confirmation to the teacher

#### Requirements Alignment

| Requirement | Use Case |
|-------------|----------|
| T5 - Teacher can mark attendance for assigned classes | Mark Attendance |

#### Design Source Link

| Design | Tool | Link |
|--------|------|------|
| Sequence Diagram 2 | Draw.io | [INSERT LINK] |

---

## DATABASE DESIGN: ENTITY RELATIONSHIP DIAGRAM (ERD)



### Entities and Tables

| # | Entity | Description |
|---|--------|-------------|
| 1 | Super_Admin | System administrator |
| 2 | School | School information |
| 3 | School_Admin | School administrator |
| 4 | Parent | Parent/Guardian |
| 5 | Student | Student information |
| 6 | Parent_Student | Links parents to students |
| 7 | Class | Class information |
| 8 | Teacher | Teacher information |
| 9 | Subject | Subject information |
| 10 | Attendance | Attendance records |
| 11 | Grade | Grade records |
| 12 | Message | Parent-School communication |

### Entity Details

#### Super_Admin

| Field | Type | Key |
|-------|------|-----|
| super_admin_id | UUID | PK |
| name | VARCHAR(255) | |
| email | VARCHAR(255) | |
| password | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| created_at | TIMESTAMP | |

#### School

| Field | Type | Key |
|-------|------|-----|
| school_id | UUID | PK |
| name | VARCHAR(255) | |
| address | TEXT | |
| contact_number | VARCHAR(20) | |
| email | VARCHAR(255) | |
| created_at | TIMESTAMP | |
| super_admin_id | UUID | FK |

#### School_Admin

| Field | Type | Key |
|-------|------|-----|
| school_admin_id | UUID | PK |
| name | VARCHAR(255) | |
| email | VARCHAR(255) | |
| password | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| created_at | TIMESTAMP | |
| school_id | UUID | FK |

#### Parent

| Field | Type | Key |
|-------|------|-----|
| parent_id | UUID | PK |
| name | VARCHAR(255) | |
| email | VARCHAR(255) | |
| password | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| address | TEXT | |
| occupation | VARCHAR(255) | |
| created_at | TIMESTAMP | |

#### Student

| Field | Type | Key |
|-------|------|-----|
| student_id | UUID | PK |
| name | VARCHAR(255) | |
| surname | VARCHAR(255) | |
| date_of_birth | DATE | |
| gender | VARCHAR(10) | |
| email | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| address | TEXT | |
| enrollment_date | DATE | |
| status | VARCHAR(20) | |
| class_id | UUID | FK |
| school_id | UUID | FK |

#### Parent_Student

| Field | Type | Key |
|-------|------|-----|
| parent_id | UUID | PK, FK |
| student_id | UUID | PK, FK |
| relationship | VARCHAR(50) | |
| created_at | TIMESTAMP | |

#### Class

| Field | Type | Key |
|-------|------|-----|
| class_id | UUID | PK |
| name | VARCHAR(50) | |
| grade_level | VARCHAR(10) | |
| section | VARCHAR(10) | |
| created_at | TIMESTAMP | |
| teacher_id | UUID | FK |
| school_id | UUID | FK |

#### Teacher

| Field | Type | Key |
|-------|------|-----|
| teacher_id | UUID | PK |
| name | VARCHAR(255) | |
| email | VARCHAR(255) | |
| password | VARCHAR(255) | |
| phone | VARCHAR(20) | |
| qualification | VARCHAR(255) | |
| created_at | TIMESTAMP | |
| school_id | UUID | FK |

#### Subject

| Field | Type | Key |
|-------|------|-----|
| subject_id | UUID | PK |
| name | VARCHAR(100) | |
| description | TEXT | |
| created_at | TIMESTAMP | |

#### Attendance

| Field | Type | Key |
|-------|------|-----|
| attendance_id | UUID | PK |
| date | DATE | |
| status | VARCHAR(20) | |
| remarks | TEXT | |
| student_id | UUID | FK |
| class_id | UUID | FK |
| teacher_id | UUID | FK |

#### Grade

| Field | Type | Key |
|-------|------|-----|
| grade_id | UUID | PK |
| subject | VARCHAR(100) | |
| score | DECIMAL(5,2) | |
| term | VARCHAR(20) | |
| remarks | TEXT | |
| student_id | UUID | FK |
| teacher_id | UUID | FK |
| class_id | UUID | FK |

#### Message

| Field | Type | Key |
|-------|------|-----|
| message_id | UUID | PK |
| subject | VARCHAR(255) | |
| message | TEXT | |
| created_at | TIMESTAMP | |
| sender_id | UUID | FK |
| receiver_id | UUID | FK |

### Primary Keys

| Entity | Primary Key |
|--------|-------------|
| Super_Admin | super_admin_id |
| School | school_id |
| School_Admin | school_admin_id |
| Parent | parent_id |
| Student | student_id |
| Parent_Student | parent_id, student_id |
| Class | class_id |
| Teacher | teacher_id |
| Subject | subject_id |
| Attendance | attendance_id |
| Grade | grade_id |
| Message | message_id |

### Foreign Keys

| Entity | Foreign Key | References |
|--------|-------------|------------|
| School | super_admin_id | Super_Admin(super_admin_id) |
| School_Admin | school_id | School(school_id) |
| Student | class_id | Class(class_id) |
| Student | school_id | School(school_id) |
| Parent_Student | parent_id | Parent(parent_id) |
| Parent_Student | student_id | Student(student_id) |
| Class | teacher_id | Teacher(teacher_id) |
| Class | school_id | School(school_id) |
| Teacher | school_id | School(school_id) |
| Attendance | student_id | Student(student_id) |
| Attendance | class_id | Class(class_id) |
| Attendance | teacher_id | Teacher(teacher_id) |
| Grade | student_id | Student(student_id) |
| Grade | teacher_id | Teacher(teacher_id) |
| Grade | class_id | Class(class_id) |
| Message | sender_id | Parent(parent_id) |
| Message | receiver_id | School_Admin(school_admin_id) |

### Relationships and Cardinality

| Relationship | Cardinality |
|--------------|-------------|
| Super_Admin manages School | One-to-Many |
| School has School_Admin | One-to-Many |
| School has Teacher | One-to-Many |
| School has Student | One-to-Many |
| School has Class | One-to-Many |
| Parent has Student (via Parent_Student) | Many-to-Many |
| Student enrolls in Class | Many-to-One |
| Teacher teaches Class | One-to-One |
| Teacher records Attendance | One-to-Many |
| Teacher records Grade | One-to-Many |
| Teacher has Subject | One-to-Many |
| Class has Attendance | One-to-Many |
| Class has Grade | One-to-Many |
| Student has Attendance | One-to-Many |
| Student has Grade | One-to-Many |
| Parent sends Message | One-to-Many |
| School_Admin receives Message | One-to-Many |

### Design Source Link

| Design | Tool | Link |
|--------|------|------|
| Entity Relationship Diagram | Draw.io | [INSERT LINK] |

---

## USER INTERFACE DESIGN

### Flutter Mobile Screens

| Screen | Description | Linked Requirement |
|--------|-------------|-------------------|
| Login | Parent login with email and password | P1 |
| Parent Dashboard | Overview of child's information | P3 |
| Attendance View | Monthly attendance records | P4 |
| Grades View | Subject grades and assessments | P5 |
| Announcements List | School announcements | P6 |
| Events List | Upcoming and past events | P9 |
| Reports | Download term reports | P7 |
| Learner Profile | Child's personal information | P3 |
| Messages | Send and receive messages | P8 |

### ASP.NET Web Pages

| Page | Description | Linked Requirement |
|------|-------------|-------------------|
| Admin Login | Admin login | SA12 |
| Super Admin Dashboard | Overview of all schools | SA2, SA7 |
| Manage Schools | Add, edit, delete schools | SA3, SA4, SA5 |
| Add School | Create new school | SA3 |
| School Details | View school information | SA2 |
| School Admin Dashboard | Overview of single school | SA13 |
| Manage Users | Manage learners, parents, teachers | SA6, SA14, SA16 |
| Add Learner | Create new student | SA14 |
| Add Parent | Create new parent | SA14 |
| Add Teacher | Create new teacher | SA16 |
| Class Management | Create and manage classes | SA18, SA19 |
| Teacher Dashboard | Overview of assigned classes | T3 |
| Class Details | View students in a class | T4 |
| Mark Attendance | Record attendance for a class | T5 |
| Attendance Records | View attendance history | T6 |
| Record Grades | Record grades for students | T7 |
| Create Announcement | Create and target announcements | SA22 |
| Create Event | Create school events | SA23 |
| Attendance Reports | School-wide attendance reports | SA20 |
| Grade Reports | School-wide grade reports | SA21 |
| Messages | View and respond to messages | SA25 |

### Design Source Links

| Design | Tool | Link |
|--------|------|------|
| Flutter Wireframes | Figma | [INSERT LINK] |
| ASP.NET Wireframes | Figma | [INSERT LINK] |

---

## DESIGN ALIGNMENT TABLE

| # | Requirement | Use Case | Sequence Diagram | ERD Element | Interface |
|---|-------------|----------|------------------|-------------|-----------|
| 1 | Parent views attendance | View Attendance | SD1: Parent Views Attendance | Attendance table | Flutter: Attendance View |
| 2 | Teacher marks attendance | Mark Attendance | SD2: Teacher Marks Attendance | Attendance table | ASP.NET: Mark Attendance |
| 3 | Parent views grades | View Grades | SD1: Parent Views Grades | Grade table | Flutter: Grades View |
| 4 | Teacher records grades | Record Grades | SD2: Teacher Records Grades | Grade table | ASP.NET: Record Grades |
| 5 | Admin manages students | Manage Students | SD2: Admin Adds Student | Student table | ASP.NET: Add Student |
| 6 | Admin manages teachers | Manage Teachers | SD2: Admin Adds Teacher | Teacher table | ASP.NET: Add Teacher |
| 7 | Admin manages classes | Manage Classes | SD2: Admin Creates Class | Class table | ASP.NET: Class Management |
| 8 | Parent views announcements | View Announcements | SD1: Parent Views Announcements | Announcements table | Flutter: Announcements |
| 9 | Parent sends message | Send Message | SD1: Parent Sends Message | Message table | Flutter: Messages |
| 10 | Admin views messages | View Messages | SD2: Admin Views Messages | Message table | ASP.NET: Messages |
| 11 | Parent views events | View Events | SD1: Parent Views Events | Events table | Flutter: Events |
| 12 | Admin manages events | Manage Events | SD2: Admin Creates Event | Events table | ASP.NET: Create Event |
| 13 | Admin views reports | View Reports | SD2: Admin Views Reports | Attendance, Grade | ASP.NET: Reports |
| 14 | Super Admin manages schools | Manage Schools | SD2: Super Admin Adds School | School table | ASP.NET: Manage Schools |
| 15 | Admin manages subjects | Manage Subjects | SD2: Admin Adds Subject | Subject table | ASP.NET: Subject Management |

---

## MICROSOFT PROJECT PLAN

### Project Schedule Screenshot

[INSERT MICROSOFT PROJECT SCREENSHOT HERE]

### Project Activities

| Phase | Activity | Start Date | End Date | Status |
|-------|----------|------------|----------|--------|
| Phase 1 | Requirements Analysis | [Date] | [Date] | Complete |
| Phase 1 | Stakeholder Interviews | [Date] | [Date] | Complete |
| Phase 1 | SRS Documentation | [Date] | [Date] | Complete |
| Phase 2 | System Architecture Design | [Date] | [Date] | Complete |
| Phase 2 | UML Diagrams | [Date] | [Date] | Complete |
| Phase 2 | ERD Design | [Date] | [Date] | Complete |
| Phase 2 | Wireframe Design | [Date] | [Date] | Complete |
| Phase 2 | Design Alignment | [Date] | [Date] | Complete |
| Phase 3 | Backend Development | [Date] | [Date] | Pending |
| Phase 3 | Flutter Development | [Date] | [Date] | Pending |
| Phase 3 | Integration Testing | [Date] | [Date] | Pending |
| Phase 3 | Final Presentation | [Date] | [Date] | Pending |

### Microsoft Project File

| File | Location |
|------|----------|
| SchoolPortal_Schedule.mpp | project-management/microsoft-project/ |

---

## GITHUB AND PROJECT PROGRESS TRACKER

### GitHub Repository

Repository Link: https://github.com/BRIGHTWEBCRAFTER/School-Parental-System

### Repository Structure

| Folder | Contents |
|--------|----------|
| docs/SRS | SRS documents |
| docs/architecture | System architecture diagram |
| docs/uml | Use case and sequence diagrams |
| docs/erd | Entity Relationship Diagram |
| docs/wireframes/flutter | Flutter mobile wireframes |
| docs/wireframes/aspnet | ASP.NET web wireframes |
| docs/design-alignment | Design alignment table |
| docs/design-sources | Original design file links |
| docs/contributions | Individual contributions |
| backend | ASP.NET Core API (Phase 3) |
| mobile | Flutter app (Phase 3) |
| database | SQL scripts |
| project-management | MS Project and Progress Tracker |
| presentations | Phase presentations |

### Project Progress Tracker

| File | Location |
|------|----------|
| ITC327W_Progress_Tracker.xlsx | project-management/progress-tracker/ |

---

## GROUP MEMBERS AND CONTRIBUTIONS

| Student Name | Student Number | Phase 2 Responsibility |
|--------------|----------------|------------------------|
| LP Moshoeu | 223046876 | Figma Mobile Application |
| A Sithole | 223000460 | Figma Web Application |
| MA Nkuna | 224000274 | ERD |
| ST Pheko | 223050336 | System Architecture |
| TMC Motone | 224027806 | Sequence Diagram |
| PA Luthanda | 222023335 | Use Case Diagram |

---

## ORIGINAL DESIGN SOURCE LINKS

| Design | Tool Used | Source Link | Editable File |
|--------|-----------|-------------|---------------|
| System Architecture Diagram | Draw.io | [INSERT LINK] | system-architecture.drawio |
| Use Case Diagram | Draw.io | [INSERT LINK] | use-case-diagram.drawio |
| Sequence Diagram 1 | Draw.io | [INSERT LINK] | sequence-diagram-1.drawio |
| Sequence Diagram 2 | Draw.io | [INSERT LINK] | sequence-diagram-2.drawio |
| Entity Relationship Diagram | Draw.io | [INSERT LINK] | erd-diagram.drawio |
| Flutter Wireframes | Figma | [INSERT LINK] | N/A |
| ASP.NET Wireframes | Figma | [INSERT LINK] | N/A |

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

- Central University of Technology (CUT)
- Reitzpark Primary School
- Lenakeng Technical School
- Thabong Primary School
- All stakeholders who participated in interviews

---

Last Updated: September 2026

Phase 1 Status: Complete

Phase 2 Status: Complete

Next Phase: Phase 3 - Development
