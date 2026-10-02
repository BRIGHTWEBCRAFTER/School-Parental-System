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
| System Architecture Diagram | Draw.io | [[INSERT LINK](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&title=system%20architecture%20editable%20link&dark=auto#R%3Cmxfile%3E%3Cdiagram%20name%3D%22Page-1%22%20id%3D%22rOOHMCs30niOwS1nVKly%22%3E5V3ZkqO4Ev0aR7ge7GDHfqyll%2BmY6qnb7mXmUQUqm2mM3CBquV9%2FJSE2gW1sI5a%2BDxVlBAiReTJTOkqJiX67ff0Qgt3mHrnQn2iK%2BzrR7yaapinKkvyjJW9JiWrqi6RkHXouL8sLVt5%2FIS9UeGnsuTAqXYgR8rG3Kxc6KAigg0tlIAzRS%2FmyJ%2BSXn7oDa%2F5EJS9YOcCHlct%2BeC7eJKULzc7LP0JvvUmfrFr8jbcgvZhXHG2Ai14KRfq7iX4bIoSTX9vXW%2BhT6aVySe57v%2Bds1rAQBrjJDc%2Bfvt38%2B%2FXzMtK1u832T%2FvXt%2B%2FmTKvWwiuO8FsqAwxfybmbDd76pEAlP182HoarHXDoFS9E9aQswiH6CW%2BRj0JSGKCAnLt58nxfKAK%2Btw7IoUOeCEn5zTMMsUckfs1PbD3XpY%2B%2BCVEcuJA2XyFHSbOegR%2FzZk00y6fteiQ%2F1phdkRQ8IfIuxTewfsUoPTGLGMauyQWqsXtNbuPn04pWzoZgjFzxwOQC2E8Ukh%2Bz1VuE4ZYcX4cOFYKD4xCmDybCT55dbg8pLrSRvQV5ZfhakDdX2QeIthCHb%2BSSTQFVOofQS45APTWQ1%2FQSfvxWth%2FA8b%2FOas4hQn5wlJyAGL0JYsqaq0NLAUw1ij1DRppZFdJCEJJalpEpS0bmeKyK4nXFG6VWNJGh%2FVsEw4gKRVNWGPyEG%2BJHWYkFtrSZwWO0axHd6lJAt2Z2o7k0OkF3LQaAqi5RHDrwQGV2Ree02lTaxJ9s0BoFwH%2BXlwraya%2F5E6Edh8m%2FEOM3HilBjFEZRPDVw3%2FT2%2Be2yQ%2F%2FKZy6e%2BVVs4O3km%2FFIFxDfOCFVLtemyH0AfaeyxKr0w279ToMwVvhgh3yAhwVan6gBbnybUOda8uS%2FrWFMlfMcrgT71KyS%2FbfRX4krdlTh%2BAwdEPAXSIxfpcAvUw%2B56PR3o%2B9Xn2tateYrG2WTNYSZLeQZrJ2iya76NdkteYmS14tfPs7vYwe%2FFM8k9%2FEjk41dKsPQzfsY%2FZavUU%2F2crbMs7FUI3TqjFOqxxPDb0sM3nGabVonMvRxNPMONO7hmafFyl1OVDkZ%2F37A2FJMzoKS6oyUCktj7sHsysZHXAGQxso1Qj%2FRLZB38M2VHiMVbwjLdOUa3frBfUkwkG%2BwfWez6ZEGjdy%2BgM%2BXp3ROFJcal9LA0aljGCrKwSPiEA7PNRvHSnRDgQVQk2EdPGiHlFdbIYI7BOa2A22NaujzptqjAfce%2FGc%2BaWvEDgb6lf3u6waiJ3k7yQoXyDCDHPRke5HxGEe133C45%2Bt%2Bnv06PmwB%2B0L%2FTKrs7BmDbTzqi6qQjIEll8TSDtpZLE6Jn5ON8oDIcM0OxLToSG%2FbFfiwyfcgiOJfdHefS8tuQcBnUMmgmOiiHhXR5wYydxEfme1ru8efCF1ZNMuX%2BAOhZQyO3A3KSw27wy41MycZXaWTS%2BWwJJ1SdoHS48d6g7AwhWcgkRxAQZnYuXadakCApfRa17SEY5dyCjWtmrESafp3BpvQwgwtQ3HB1EEo6x6ckSFe3GTuTwBxjBwQeDA7AkhM50JzSsBNIPkvPpz4w4Con0HbpPGplJ6vqDtX%2BCv2AtZ5c7PAL34lM3csn4Kr%2F6ZowUD7EUEk6c96XLHUDvPI%2FRIxXCrSss80HocjnTnGkpQDhv4%2FyKgHBTucwpFSzjBspjhpvcWzKoFd1C1qotcAPMo0C06G6nGUTdSF8hmQy33sVR5eQuN2Oax24az8Xyq4F2I1iGMToRyTZC4KDYQc4N0rkVTAoS9J%2FLu2ENB%2F6gzl4JH1qWhrlEy2FhIgrTgYlbx2nEYOEn8BlRVkDxCeaS5qCX6qatUQREfquCVdHm5gr8Dgdj%2B1ImAjy1jl6j8d7sjFFMZMi3BQxjqmSJ%2FIg8eHIetJAekrRxT6o4yV01rUkwPmCuKMTmcIsCOHmDoESVQgI8nb8DosZMy5HCxeph%2FfveVXPKDRYrr3c6vdiaOR4vjRHZrTZ4mLb31PSLWqzNaKYcu15RypBM6Qroly5MZI%2BoIsWngxJ%2BpRiOkv%2FdjjFlGwn0aquox2gSMyXRqDfYegfNzzZo6c5I3pRDEIQiiVKw3dAyQn%2FMpGGYuCH9Ow%2FXjlCiXaE9J%2F10l%2F%2BkZjXLd9KD44%2BqqWbSeZi9dhXsdrvfMF8uAe7oKKUvbMeZmV4gfUdfucOLDQ4ie2RqztKf%2BFAcOBTZ5HKZaeEIU%2BiDJXKDgyUlRPozM6NwtowsTDq8dHWcrH%2BqJeH0pTcGNsvxGyyh8Y2OxksJSajrGG3LcZDifl6QJLrc5uXU7KXBvBdgIT23GzO%2BhLs6pK5tTurSidFJduUXbbRz0xn%2BIyV6GNKdn%2Fl%2BwbDlh7AXE922ZWqdF9uw2hU101Rh0KVeW%2BUqR%2Bz0Jck4ZcefQdw9xtJmUuTvQPXjFdZyGkHtriEuO2gPziJJvG0fwjEupD%2BLJuxYC997ZR1naFfpn0sK32WgefSAZPLYuMNjCOl15UuLjtlYYqFTkRQYqcK%2FpJgQU8rRX4DnMWECIq8UFSZfVUuCY9MUkJ5lmylxRtclBpokciFRRgX0ylwL7tFzKZ58qEGqWnFJLURUwdGhq7chqtL2ry3Sht22LMykJJirrP6sVLcoVWWLmnuSFpKbRIs5TNY6LaVVseyIwrZZErJt7oN0N02q2yqz3vI79BH1fKPN64zUNfa6W064rEUm2%2Fba5jFJanMqNzbJMMbDo5xhbDoKFVUQBDX3K4rDp14S%2BpnHK7jNO%2FcZRqNGS6WGMOCoI%2BPj168NqQudG6SYo1w9%2FMF3%2FimHUGtcnLvRRhT19TGljQavRaGGgmvm0%2BuszU0a0Q0EE29KGsPZCZJkkKmNEk0lDMBOzO82MeVFUN2Zia50po9Hqm4Eq4%2FKpeXPf3nXxDjwCql%2FlBjg%2FIeO5puRvtSGCogcrGD57Tpm77SoXTVzPqC2Ejb2kUT3WYPdraTCzK4rJkpbSaTWa9utDTFpNJBBT1%2FRFR2JKF0YMT0wNEkCFtBiJUhrRFMNeF33xbqLV%2FM99M8xdpXwKMVs17HlX%2FsXucaTRXVpBCNceaR3XL%2B8M%2B2jtNc0n%2BIJ8OKNhnAZswNOFHQKFkK0tbZSSAJ2Yry7j908%2F%2Ffh6eMJWzvoEs%2Bx%2BbE1EWxtB%2F%2FPjk%2B7e35jq7uYhfl3GLrr73uf2xi2BbXTpcq25zTuAWUd2%2BoAivA7h6j9%2FnplpWs7Ik%2BJCdU1CTK2FdI%2FkQAf%2B82T07MtQXmVJLCFbFRkdh84hZ9pau5JMLsjSaAbRoA%2FiotA%2BG8O3mU6XcJMgOJSWZTtY3SUr9k9uUwursY2agWI64kkHijL69rV%2BaET5v3s90flBy97rdlDINgyQy6zU7NghECvmQi8DQZMFhB450TEFpDvkxGKqY68OLQuQRMCDcbNf0l07HHBBzK5pwP6e68VtvocuW8fJ5ahMvadJMunhMcK1j1hRk2Qnbtxh6oKHaCNtutZDDHYzsRpKUWReZ7qwj708R9rjfEZLjvTiMd%2B7VxLYA%2Fa9nYjtgbXfBbTETWgZ9ZUqXMw%2Blafw33uFzbst8PyM%2FFrd01nhiE89NQwrHxBpYMBX5%2BSAOCfb%2Fy%2BcLL2Bbsxz%2FWnbdj7AdIlDH3vFCHMXswqFYMvCXerRGmWTSUsSm5QSxLJ8scs2xk9z2waSujU7lnHVNHVLwIYt%2BqT2MrfqEdMIID2E8LqPfmllwxI3%2FJfmztUx5%2B1UhsSFPII%2FgqcQkBbF4sfuKuPnwiLn6a2PYsrifEQR9oI1%2BTWjcYDPSlBWmX5mETz6%2FOgL9D16dHXAw8tYRG0vhR4A6fHNhe%2BNSQTNmZn5BxEoM7NXL%2Fpt9YjXPpLUa5Q%2BqaLMFc2YXJ7Te4g2H0hcMISU3oUY65vGBWNxxLvJDgyN2J%2Bh5EHowhB4lm1wycW3lGfnjQbBYwkOrQ347gGdEuekuvLep9%2Fr7WGHtOooULU6Q8ZQM65qOCRNWM9qi1JKp%2Frbn8kfasJVE6bN6ExKQ%2B2nN5CSOP6VKCVtPFISLW7RmZCG%2BqHnOiEpWsV%2Fm2KWkURR%2FVZzoeqczsLTsdopm3zV9AHoC894O1n%2BUcI5Hpj1TxPV8IbtFJJ%2BBaF2G1WFbeiwZ%2B%2FMS8eJdZ%2FQrPQdu0TYmBeeVBCmzcvgYrxARjdIBFrhmRRmUbI4orRIicEP0ZM5%2FzEtL2oSsigvxVqDPfaXnTn932qUos9LwFKqPm0fz9Q%2B9ArNCCGOQwbA5OsfylRYmtUmuhosB9C1pejIpE09aL%2FVoi2DAqwwZ7k6OkUlAViJE0tcU77NFodWsu1RPu2mTOGcUrGwOjnXJeRMszPAjXnNeQVwJgUcJ0uqqWZFcNUCsKDZaz%2F%2F%2FJDisWhI6mPLDSJG0ftUrYycKbiuKb%2BHH6abaLGGyPZaQky0RepOGoSafVtjICMhtZwPuuyu69BjNkP7hmZRQ7sNURRVOg1sIS2b0JLo1W8Q3pQfTE00ZlbHR0dgS%2F89Zmt8E2PkFs2niuTNpNWMkMRc5DawRw5DRCWTT1oQiGzukQvpFf8D%3C%2Fdiagram%3E%3C%2Fmxfile%3E)] | system-architecture.drawio |
| Use Case Diagram 1 | Draw.io | [[INSERT LINK](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#R%3Cmxfile%3E%3Cdiagram%20name%3D%22Page-1%22%20id%3D%22S9ZDkLS9Wo1ej6lvENuU%22%3E7V1bc6M2FP41fswOd8Nj4mTTne7OpMlumz51FFBsphh5QV7H%2FfWVQNhGODYGHdlOkskkSAgBR9%2B56kgM7NH05TZDs8k3EuFkYBnRy8C%2BHliWGZhD9o%2FXLEWNYbplzTiLI1G3rniI%2F8NVQ1E7jyOc1xpSQhIaz%2BqVIUlTHNJaHcoysqg3eyZJ%2Fa4zNMaNiocQJc3av%2BKITspa3zXW9b%2FheDyp7mwa4swUVY1FRT5BEVlsVNk3A3uUEULLo%2BnLCCecehVdyus%2Bv3J29WAZTmmbC5L7fyY3F98f7%2B9H337Gt3%2FMfrj5xZZeRFVOlxUNMjJPI8y7MQb21WISU%2FwwQyE%2Fu2DDzuomdJqwkskOy6t%2FoWQurhYVOKP4ZeMO4iFvMZlimi1Zk8kmHY2hINtiTXWnAoXox6pGQYDLFkUkxny86ntNFnYgKHMAlaw2VGLjO%2BOH82lyGVKSMVrwl44Zlr6iJ5zckTymMUlZkydCKZluNLhM4jE%2FQck%2Baj7MZzhjVZfRNE47kdZrEtau01UiKxxdnTZ0Zf0wZscdkHePx3FOC3Ll4YRJjbwTwdwmwVZcXiHRr5PMDKBI5p4OFAeWl7BHuMpnKK3d3%2Fs550KtHK%2BLvBywS%2F7UGS46qhqwo7H4P7DsoDxKyprPZbdlg4di%2BPTC3gODvddmDNm78TdHYjhC1hTzgXwmKX0Qzcwt4zaNo4if5MMSxun4K37m97DXNfeCKEUVzci%2FeEQSDpLrlKT8yoxQRNFTcQsu8WckTmlBBPeK%2FTKyjIxP7sBljzliZXNdZr%2B8eUZHJGVdo7h4P4xyusA53SfaqkG%2BKwiD2KGHphyF6VM%2BK64wHpaMoaedAGB5DQD4dQCs9ItAgAMFAGEU4ahhezQRQeZZiHf0FTQFZhpdcsuHlcgMM0xcsRphWZmMSlcRyieFOjfrI1LX8%2BxJsuWjOFUU%2FuZn2ECL4vXLZsvrZVV6ienjxvHGVay0vogXqmvKd6AoG2O6X1%2FUZVAJESE1NkuMgXDxPlXtuNZGlLbBJsMJovGv%2BuBsin9vBzJEd3ecadbYsiUN4VhSF%2BVAi6s2zTipI9fZ01FJxUZHBVBX79gduz6szs4KnZ2hQj0xfp%2BHIc7z53miSnUHktC3JGPHgGL6QCvhUkL7k8%2FZb%2Fm4ksy0hlD0M02FUtMBl5pGN6lpqpea%2FkFSM07DZB7hkxCajieBy%2B8oNG25oyoKoUlompZC7Fq7sCtMuNfxuRVntvSzE3a8cIezmBGFW6S9YN9D8bfGYHu8rYT%2Fa8KsNd58V%2BrI0oq3KuoCpWy%2BkjGr%2BNLNE%2BvgT9tgWtmydVHKuGEsmkHpYSeQKAYWgbCAozYlxeIipABuvjTIBubzWa5KFWCD2y%2Bn4%2FVVwuwc3T5X1ihyF%2B01ynEtmCp6oga%2BO02YN2Z%2BW4dFLU7J%2Ft4LutZBC9lLNDSjV2XEzd0F3rdpf28VwQAGuGlLOOkaJZMNSO3islWYTM88zXeMwgnuZnnunzXxfbNGaQcsAGSrDABVA9SPiw0oLv4U%2BE6dk232xHt0GC%2FJXR6XwRk90XKjgZhJepX%2FPclcsoXVvIZN2WM76bHSOn2lR%2FUYuqSHDey%2BhxlGFPP3SVOG6xBPsXgRCF9ecrECF0xAAPvyU5TybCHt2QQWHMUclSK1l0Rdiz7D9uqiz3O6SD4gS6ulQK2wqN1iUiXzdFtMtsr4SD8TXakPqQ1wRzLRXTk02xpwcujN1gw4pRGND8CdvoQLnHpHvqUXcCqDED39l%2B1R3k4x3jPH26Euii%2BFsjw57Vp2aST4BoYhgWuXSzOUc3G7BkRs369L2yqgqwv8wHlDKIo4iHqEOrpMS%2B4KqfYzzIOTsYbOMmBpb0t60WENdVdOx7WGqnlPcN9ZsCiTsEaEKYr1udGNTEl16xhURibfIbdW4Dtfbg00c6sFy62rAGGYoDzH%2BlhUTvBTx6K6YoOaKeaCJaw4wHk%2Bl3nOZ7BqGoES%2FoeRJDsCJWUeVkfJVou2%2BqR%2BhyTj5i%2BiRepLyi7URLShLEHVEU1peOakZ%2FsOTLVqq1RdEKX6ur%2F8qrYcDuugaSwobe3ESmrX1xyjdj7SSPpBEiaEo8%2FOc1zNgPM%2FANcLcEeahVMGOFezhHM%2FPNl%2BgNO0UgUOcL5mwAH7ZSvbeJyhSJ8z4YNtHuAqTdk4RbsYhIO6mK3yosHOcy%2BS%2FevL%2BTzQPAbsf65iH8dIJzMNsLCuq9IFfYcpKS6MB6pPGfpDzcpQpblvfQBOF%2BB6pKQcGXAqp5lP0pgADrIBIe7gvHkpTSQIDkoyUWboHBnO1XtAGTp%2FxnjBasqZC77CdD1HIe9UpcfN8MAyw733s9gGRjJUWHxLfozE3kNPM3tbsOy99mOKeTSkcx4NbNWcZ38wcj9Gtt4%2BI2sOSHjAqQUrRi6jf4WmBo7%2FyZYP2PYrHnAwJyTzhNOr3EBO60pBRxKKcCsFPZWRHW9naOeNbcayNQv%2FTDdjsTtvJ3fkZF0POJm%2BkpUbGzAly5o47SAA2mwmKQ3QEE6KKs2vh9%2BNSUkYTokA8M5mO8kOFtZexm694U1jIwO9FlblTmixEhq7dGuadJU3b2vMq6nbsFtlNKQaG0hx8Slw%2FLrIGO7fP6SD1dBnzxF3W3LOmZgSMvQCOW2iraBwjD0dqRMUzre7fLb4%2FCOPpj%2Fd36%2BNX7PbLxdKw3y7cP02Z5wsmOBAAyYS3DpvI6hxDfRWuAFn%2B2x88QWFTEcBpiHIX3yR19N20T1bSaYy3%2BdtZ%2BTtQpxmBvW6M6g213IruQ7ZheUDRacr5iXpBDcRu5VcQ1gxf%2ByPhKhYpbiVbno%2FrqLvGyH13fBNFX7aVvp1jOrsYnldQR1zj6BSE9LZxa3vIqJjyVs5to7oyDlfmhVzlWKmBNs7p3HPd8aiteo%2BkyiDrHcan8HoOmHR4ILO4GXF9Sdsy%2BbrLwHbN%2F8D%3C%2Fdiagram%3E%3C%2Fmxfile%3E)] | use-case-diagram-1.drawio |
| Use Case Diagram 2 | Draw.io | [[INSERT LINK](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&title=final%20draft.drawio&dark=auto#R%3Cmxfile%3E%3Cdiagram%20name%3D%22Page-1%22%20id%3D%22S9ZDkLS9Wo1ej6lvENuU%22%3E7V1bc5s4FP41nn1Khzv4MUnT7M62O9mku80%2B7Sig2mwxckFukv76lUDYRmAbYx0ZJ%2Bl0WhBCwNF3LjoXeWRfzp6uMzSffiIRTkaWET2N7Pcjy7JN02X%2F8ZbnssW03KBsmWRxJNpWDXfxTywaDdG6iCOc1zpSQhIaz%2BuNIUlTHNJaG8oy8ljv9pUk9afO0QQ3Gu5ClDRbv8QRnZatgWus2n%2FF8WRaPdk0xJUZqjqLhnyKIvK41mRfjezLjBBaHs2eLnHCqVfRpbzvw4aryxfLcEq73JDc%2Fju9Ovt8f3t7%2Bel7fP3n%2FC83P2sZRTTl9LmiQUYWaYT5MMbIvnicxhTfzVHIrz6yaWdtUzpL2JnJDsu7f6BkIe4WDTij%2BGntCeIlrzGZYZo9sy7TGh19QbbHFdVtT7SJcSxHnAtwVadIzPlkOfaKLOxAUGYPKlldqMTmd84PF7PkPKQkY7TgHx0zLH1EDzi5IXlMY5KyLg%2BEUjJb63CexBN%2BgZJd1LxbzHHGms6jWZz2JG2Tsn6dsGemVaes6UGR1ulCWjYO43fcA3y3eBLntKBYHk6Z4Mh70cxtkmzJ6BUYPYlkBhTJ3OGgcWR5CXuFi3yO0trzve8LLtfK%2BTrLywk752%2Bd4WKgqgM7moj%2FR5Y9Lo%2BSsuVDOWzZ4a6YPuXI93Yg33WhptHrMo3s8%2FjHIzEjIeuK%2BVx%2BJSm9E93MlqmbxVHEL%2FKZCeN08hF%2F5c%2BwVy23gi5FE83IN3xJEo6T9ylJ%2BZ0ZoYiih%2BIRXO7PSZzSggjuBfvLyHJpvHNHLnvNS3Zurs7ZX949o5ckZUOjuPg%2BjHL6iHO6S8BV83xTEAaxQw%2FNOBDTh3xe3GHcPTOenvXCgOU1IBDUIWAHEgKgAOCX4%2BKoYYE0EUEWWYi3jDVuysw0Ouf2Dzsjc8wwccFahH1lMipdRCifFkrdrM9IXduzN8me78Wl4uQffoVNtDh9%2F7Te8%2F1zdfYU0%2Fu147W72NnqJn5S3VN%2BA0XZBNPdKqMuhkqICMGxfsYYCBffU7VOan3EWRtsMpwgGv%2BoT866BtimFMVwN5xp1rDl17Fly%2BAqJ1rctW7MSQO59o6BSio2BiqAuvzG%2FtgNYNV2VqjtDBUaivH7Igxxnn9dJKq097jO885Yl%2FIeayVcSujh5HN2Gz9OoMteNE2FUtMBl5pGP6lpqpeawV5SM07DZBHhQQhNx62Da%2Bkb2Fdo2tJApuNqFZqmpRC71jbsChNuMz5bcWZLf7bCjp%2Fc4CxmROEW6UGwP0Dxd8Zgd7yZkjKwZGXQGW%2B%2BKw1kacVb5XuBUjYfyYQ1%2FNZvMdZjSb10pal38Ni6KGVcMRbNdOnh5dpGPcWAHTclxeLCq6DdfAEEmqtSBdjg9stwVn2VMDvFZV9jeTHurVGOa8FU3hM18N1qwrww89vaz2sxJPt7J%2Bg6Oy0MaSBbM3pVetzcbeB9mfZ3qwiGMMDl6FNfL5l1bHHZyU2mJ1TzGaNwivtZnssJ2RwyDGQVB2ZC2So9QNUMHcbGBhQbvxsHTp2VbfbGO5QYP5OHPC6HM3qi57UOIpS0UQC4G1bgK9iUI3YTH46cF9BXfFiuXvFhA6%2Ffwwwjivn3pCnDdYhnWHwIxGK%2B4WK3wSQE8Gp%2BhlKeNaQ%2FpQDMq247KmXqQSJ1JfsM26vLPs%2FpI%2FqAbK2OErXConabSZXQ020z2So9JIcZ6UpXkdoAdyQj3ZHFeWfAyc43TzPglPo03gA3fAkXOHXAVXE5XYBT6YY4cAHT6uc1O%2BJN8bLnuHjbd43iS8Fk1zG2os4zZfvX3GdR4zuKfCK2H9TR73p60Q%2BcOoSiiKOo8nbwNMXyUXmx0OEXE4yyFGfa7Ha4KJw9HoyxdJIeTbstK0aHsdRfdx3XWKreG3xpLRg4Xz2ryb1GhCmKe66%2FvRY%2BtiQ%2BlkS8DVZg4qj0ab5CPq4m5mT52LY087EFy8dL12KYoDzHcKpW8lY4cm6gOhbV5VTUTTEfjGLAKULnec6DXzVd0VARlPB%2FGI2yA0nbpi%2FM7aR15fi2OtJ2qgE7JI08JBmnJKJFGk3KboTCoyQJPTg8KnX0DDpwuGfaVlct64Jo2c0r743q098VGem8GpbQ52v2djtvKSmHQRLGGaTR8As0Ay54A9xBgDtSPE8Z4BzNEs59W9oeBjhdVS9QgHNNzYADXqgtbeNJhiLAdZpExgCsDt1VmvwxaLvYeDeWckpWOSYA6XQujLLoYzDLpY%2B9w0eS5e3LsQ1o7gZe%2BS7dMFpy4uSyaUMuDFHH5CoXv68wrcaFWfvqU8OBoVkNq1xoWG%2BA0wW4A9Jqjgw4lbHwYZsxMO49IMTtm1jjyVVieybKKLN0joznytCCsnT%2BjvEjaymjKLxQdhUekTfc0mP9eGCxO%2B%2F1lAzBiIYKiy9pISOjb6yZvS1Y9l4tZIoQHtIYwvPBKns8%2B42RD2Nk6%2BUzsmaPhAec5rBk5NLxWGhqYNejVKo3BgvJe8DenJAsEk6vch88rfWO8p4fY7DiPU%2Bla8fb6tt5YXvKtJYSnOieMnbvXfGOnFLsARcEVLJybR%2Bp5LkmTnsIgC6bSkkT5INlD3tKqwDgN5VS4odTIgC8k9kVs4eFtZOxu0oIGcmNLbGBJURlfmixEhqbjWuK98p70AVgWxD7Kr0h1dxAiot3Yyeoiwx%2F9y4oPawG5aHeUzElJOiN5YyNzqaEbPPCpX78EXw%2FS%2FDD%2FdN%2FP6%2Byxe9foj%2BuF0P6nZYbUbTUQ1gsJcPmn6mQd1tyVSwuWkna0%2BGybXpOwN%2ByqwK6o1CwdHtbNrha5LqE%2Bi9F7QiojOV4TP%2FKY8mMkN0KwNIB2E%2Fzo4yniNqTX4pqFP2uVxUJJK3UU7px7Wv0vMLkce4dXZW2IQj2Cq6CyYIGboFlQU%2BX2YmrtT5g3iZKB7ZNYOX574dlt%2F%2BmzL6k1wKtWPa16DUt%2BZDSDyaYDfmiTJ0Npwxm6Nlp2zAHnZwmlxL0ZdHm5pNwW3m20uu15qapUTcwaDvUdNozMU2d7XRsMFd0eGXW0%2F5OgSGDWU77NY9kPMlRMrg0rHYsb4EujFegEWDQW0c2VpFl2U5KazBiQWmqgS6bqoLisSWD7CFw9hEMDc%2F0yQoG4DJSFH5LyWNSMIumjCS52kzJbzS0E69nSek2hTm8hAReTRrURYVrgUmH%2FQKMJ5aqIDF7IFu0fX9iyIdLVWifpbcyy4NQrmmzA1V%2BBbtRqwSXPNeOt%2BHsH3SSeAOyubQ5sgJl%2B0ay04wQut6dGTHTTyTCvMf%2F%3C%2Fdiagram%3E%3C%2Fmxfile%3E)] | use-case-diagram-2.drawio |
| Use Case Diagram 3 | Draw.io | [INSERT LINK] | use-case-diagram-3.drawio |
| Sequence Diagram 1 | Draw.io | [[INSERT LINK](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&title=Sequence%20editable%20link&dark=auto#R%3Cmxfile%3E%3Cdiagram%20name%3D%22Page-1%22%20id%3D%22aHxls8P9WnKr221zRO1U%22%3E7V1bl6M4kv41Pjv7UD7cL495ralzumZqK7u6d%2FalDsZkmi1sPBhXVvavX0nchZAQCIxzlTPdbTAOpLh8kiIUoZV%2Bt%2F%2F1MfGOu8%2FxNohWmrL9tdLvV5qmGaoO%2FgPvvOV3dMXO7rwk4Ta7p1Y3nsK%2Fgvymkt89h9vg1HgwjeMoDY%2FNm358OAR%2B2rjnJUn82nzsOY6abz16L0HrxpPvRe27f4bbdJfftUyj%2BuLvQfiyK16tWm72zd4rns67ctp52%2Fi1dkt%2FWOl3SRyn2af9r7sgguwrGJP97rHj27JlSXBICT%2F4dgqSf27%2BFzJFUyJvAySDHsp%2FFnmH9Ns%2BuvdSL79v3640a6XpgPU6%2BKig%2F1v%2FPsP23W7hc%2BXVSr9pfLsylFPqJel5H63Mu8PpR3gA7fL2kA3xIfZ3SbwPoOiSc4A%2FcErfogD9Nwl9EoWMb%2BHhBcrPi04tCrfxry%2Fedps9AVqPff0FNCz0wyPoMP4Y%2FCcNU%2FT%2Bb4fwOQy24NNT8O9zcPDhzfvQe8morB60laOsXB0%2B4O%2BADmakAfM9%2BPHp7ZQG%2B4Ko56dxUj5Qv%2FN74Pm7IKnfutnuw8avwOeKt0%2FnY1B%2FqsZ1oOIn%2BGr4REkE%2FnOsuoyT%2BxxvQtRf7wD7%2BmewgV%2BD34SHNEiePT8gvuPbJwbdm3O6A10NfS8NY3I74SOQCtSljXcKCD3N7pN%2BfH9bdC7nqaZ8AE%2B95M%2BA9iGdfIC9gAofvwB2AFxIgi1sFtCbnMPhTy8Nyh6BfzcJoUYiUn94UbjNnu2mUvQJ3Y5gq%2B5qz%2BZE0AO5QSnFj5qvvc9t6r%2FOQfIG%2Fns%2BoW5gL64TqTUi503ty3tI8wO5X5%2B91N8hLDwfWk3bBlS6RctJrP8apOfkUD30e%2FwjyH5seftj9uxTcDpl6tEgm0mhTjQXMiIMxhYk7xvfBz9HIwBsnHfabWIvQV0IIqROTd5%2FOvxcHvf%2FAdu%2Bn1oED2Dogz34DBgGRzgufj%2FtwDClKb%2FlJpTRgkzO2wpNOCDYQONmZl5Me%2F0KsfaE7GYXRtv%2FgHL7ksTPCKPuMuHDCQD6fJOmoA1eBs11tbqLvFMm8ee4n5njEj%2F58RGBfzbQKX%2F7%2BttT9sRjGGWQsnmr%2BIS9Hjb9P7EXZ5Jrq0JLVk%2FpeZvRRM1v9RTnAqnjaBjHRZC1gC7r%2B%2FAEpgJvRSeQlYFX7UMfN7Eh0v0jDKAm3RwOQNH9YA%2BezDvxjzgFw202WJywTn07%2BvE%2BG6YffqKfcIn0MciMy2u%2FNQk80MXH8yH7ACctaYC%2FPsjeWdeBbMD%2FkCnJYEHfxfv9%2BVDrtACpEVib86ylK1%2Bzzn8rOv8EOn8%2BDRXtjf%2FjEL9GwfYlwJrBJ61vx3yM9SqC%2B%2By1SeDHSSGmM64FLLbfxYfnMIFTt3PxitM5G0HGsTwjtyU2%2BcTiaTH9Y6IhNOzC4nOEyK%2B%2BIrbgWosp16QgCJoWvhzQk37WzuE20d3PLgxsdbu%2FQAvuYxKthMIJfnRpPp03%2BzAbKx6hXW7RBbEbtb4OtZ6y4UmpH5X8qoaCpcI%2BRJMwbqFVFlW9ojCGkRJgmmhfnoMZYD4pqSMSedAcMrh8OoCpISR4QCOb13jLIxo6Goz%2FI0jC5zdRfPdh7%2BD8mR%2FLOhhfo%2BhnrynWb138zxaaLPwql8hfg2OclCYN1khvYI1ItuJsSJoYu7KGDcarXqB00xoXyo71klXGYkxSJUcLAfQBKJqwbrZwOpCjUhN6c2VpiSkHay4BPfwK%2FDN6MMxt57G09kpIRUMvDE9EznOAE43fRGjCWfxQTj7Dgx%2Bticr0HHkvLe70xS2%2FPk%2Bo8eoCkERkNj8g1dxfLFRqgFFmUPkF9JZOgUrIK%2FjhNdwGJGhq%2BvbgKA4ZOBicPkbxBjkj77MXNTwvx8CHK646ktwHqRdG%2FQVW4zQmtdqb2aDEFBhCJrSUQeAEPyFvxn0QBYhWIbqBQFRzQIJBIynXY92CiQ%2FR2yXwqJvjHKDE5DcnMt0l8en0gTyQXiEMdfOYH4tWhgKmBzCIUXNg31HiKojuCx5ZOZxhqKf6kZ0FbsrYz63%2F98e969zZXx%2Fv%2Fsf6K1T%2F%2BvWvDxr8gYZFi%2FK4mLcpAlpKO2yUR5KyQAy685LEZyj2NPEOp%2BzRW%2BiqhME0FdwPgF1%2BAqTzyyj2fwTbj9mP7mGLEYEi2gIeyroO3%2FITyD%2F4VXtx3t6PQbwP0iRbXuatKG9VUS3wIe9bcVmLc9HjXuYt0TV%2F3wyIfdoWLDiCDm6%2F63bz%2B1uAG08Vp0BfoNh26T7KmfEcH9K7OAKABR%2FQFfSX38%2Fjm2rxXEEIXoPWvECGRsEzpAj5BMwjuslv78PtFj57224O6Mg56NVJuv4YBP2p6wlN6xrqcymm9NCtXT1Wa2Y%2Fe60FdlVt7aq6qamG5pi2auS2URBVtLWqO1b%2BpebkfS%2Bi20oReZ5Gg7sUFUY%2FvusOVVEBLN3AeDi4OsSHoCmeU5rEPwKSgLJvitC3iiLBp12wrV188cCaKIHigK%2FXIDz8CtP%2FzpEAfv4X%2FLw2FTu%2Fvv9V%2B%2FL%2BLb%2Fwz8lPRBheJBBuyivQ%2BFxLLILSnOJz4gdfwEobMDtIno6en8EO%2FGnqJS9B2vEl3Zb6mIzdNhnkEu0K0fe2ISmvDnm1bToJIjAu%2F2wynWSC%2BU%2B%2FxCFaR%2BVGrSsNI9YNa20YrqUUf3qTYN76jEZ9fwZGVlUNZa3aml1gidN6jdmknHcdp4wAo%2Byu6FGwDJJ2j4MQXqJN9F13r3ooxHvKsGxHDoaatrY0w7Dzwc40G4OhbmIK7eAKLXb4w7Z25CuTw%2BaUL1Dq0XqiKu9PL98NharDNdRFkQ2wXkZo9hhGhfQqbHObYsUgEIwCBeEcwBuA2YJw1YH%2FI0J4H4XRCRqxidM03sMJOuTiref%2FeEGNLGjl7Zpo%2FKRtxeljgO4MQ6sUstBBN0eGD8raVvVuLEDUgAi8%2BmrvCMe9E2WY1pqTbdVUMVzJKJJ%2FbRprx3YUq%2FgzTSIx7lHedMFswdTMYr2g08nOMcR37FvrhkR1JSFxHkikiYaOhqaykmi4FPlyo6FuCAZDW1WbQOPoHGBoKVTQcrS1W%2F%2FDGt8XGh3VXIM1kKvns0eb2OJZoZGyv7MbHjG%2Fi4THyeCRJR4GRNZXORIirwwijcI%2FIgwjXdNtII5mqPkCtR9KOhoNvypq%2FJ4hQHOtGLZeeIa62jkrNjbTArrhUJdwiJtKDxfrRIhJEBoDJDUJktckeT4cVdamPTmM2joXjLLQriTHP8GkAzQgbFCmsXNgKjMlqpUY1Y27xkri7mJwd6BgGdisS2y%2BJu3gnuMqhminKO4H0BWTb45LXaMjakrtzxoG1HR%2FA3zLpYGalWLaDcumhOXlwHIvMTJA2JAgfE26wDtBtpgbRMYGpnTd5sJgegiposYfm6LHvAiUF%2BuDtSTKXocP1pTouSAZZ49QOK1rdX%2FQkry1umOtHZ6ZLNUZAKnZ9TnmNK7bqtGzQiqhpkg3lNL3s%2F%2B%2FNLPLTVi7JMdAWcJG%2FTEoi%2FrGRgmJx4ubzRquaI8CjsMA6blwmIGQFTnB7l5ImIbws7p78fJL3WjcO2lDGtn0aEyXHwOT58gEkUpwMaRVHVcw0uK%2BW8NU%2BGa8VN8tpCZixkv33bLeMgfqEsvVdSMuPYlFGtusiNstOwbaErJzJNouVwF40dbWRaMt7qU1HI0LbeleWkhNBNoyXLb4a5yL7GkYXbGTXLezE7JNmbM1l%2Fd3WuEyMF0mfC1HQ7g9wromOscBR2xTU%2FkQm46liFxtb8MkCWBVmy8SYxte1RivbdwNzjJ7bO7QnGCp0lHZkolny1ENflQuspeEobJjNstlmKbOhcp0f0JFTXRojkB5Vpfw4Crv5Frv3XisSTxejn9jUvEzgFumw12VDvG6SDTNnhraHVNk6K8iJ9gJTSA8B7T3PDqjG6llJt2CkJpHmgzg1STwXpNK8AKvKXrLBe7osFRboGsaUmv4jAf6OejeFMZbZql7w3%2FYEOXIoW7Ylol4s5XLES9RBnTLDLzlqMUAF7ToTcktZDZckS7oipxgtzOB8BwIPOrkNY7z17qxWWbjzYXNs8qagdoyZW9JCtMj6aQuMI70aU30zBv3eFgOX4iR7peoqIl2ZhMoz%2BrMJp9k2Q3LMn1vQZ6OPlJkAK7gLD92%2FokhoXmZvhBD9D49HJJtlS%2B%2ByEDOipxgJzSB8LxO6L4nAbPOA%2B5GcZk5uCAUn0jwDOAXnHgotWdZcK7htXeEe1Bsgy%2BmSPdzVNRE%2B7MJlOeAc8qp6t24LHMI53J6sMTDQE%2BZIrgcGXN7HyydcrqNGGy0OeN%2BDAgryQn2LhMIz4GNtbPu8RORs9NDc1cjdQ4rs%2F%2FmwkpecTGwUyb8LUfm3NhpmFPvVXMUvsgcfTFfURPtuSVQngM7q4OSW%2BdGd2KlJdPuFrTeZ0mQAZ8yt%2B6q1IB34W6pU%2FthHYMvNMbCQWNwbIwB3W3C8%2FphM%2FPcEic%2BrOmpJZPpFgS5%2FDKlg7AtU%2BmuSjF4QdjAT3sU7iFwbL5gGH0dX1ET7T0lUJ65NAW0giKXKs%2B9yq%2B%2BosUneTsSHhHvxGlN4vT8FSnEyZSB0zJzbjmKwe1r0IoMWHEojKGbq3Dmxan6GkCmWqKl2kFOsJ%2BWQHiJxSegHbwc0JN%2BZuKUHb6WTJpbarmJTjky0FaTaLsYZeBGW10Vjba448HV%2BaJidPdARU20Z5dAed49ud3To65CAx2zJdrEVya%2FLchBMZ3sGZAt0%2BSuSoG4t4Eporc6tEDd4gvXsbDXGhyvYwwXbcLzepN%2FDzx%2Fh%2BZYfMUlLJkJtyCg5pMnA3xltttVKQW3F1kXvVcC91%2BoisJZSZPqZqiR459T010jJNKzlJQ%2Fb%2FZhVjnxEfz7YYsuKFOn%2BiyrE5JlFtxcrovR8mNAsDzlbjlKwO2yAGgzOcDqnElrLBjUh0fqGNjdpjzzXt5yFpSUQZzKw1hNl45Bsg%2FRUepU97DMUbvA%2Ft7BImTArMw2W44e8MfhHNHVeHAngqpYfIE4%2Blq%2FRk60b5hEet5tv5VllnuZOjFU5pMtyHXAFCEDQ2XO2VXpAffGX0V0xm4bZV2%2BABwTC93BITgWgLcpX2Lv76pPcoVMRFsQyLIkyMBYmZt2VWrAjbHa5A4DVeMsLElf1UNy9Wq8mEdZlHe2avW8s9okyGy0mYBPOTOBVdPXlqluc7kNxgmPAcQyy205GsDtMDAc4YXVWzBrcSaxsQDQGhz5YiF4m%2FIc0PrpAL6A9w6o8rbXsNFHVK214dz7A8j%2F%2Ba23e9aW%2BW1z4axASdJB15FZbctRB34vrfBjO1v%2BA9UVeUJcjZxwLy2B9LxeWh9OjoDl9fEgZPvmJZYuw4PAliEDRWXO2VUpwgL9tGBOKdRPW9ET7aclUJ7XT1uzUT8z3PyyE2xl2tmCwLanIBmIq0nEvSZt4EVc0xBeyxZ3J2imyIPaauSEe2oJpOctyJCd6rKCqfrHOCmTkw5e9Aa0jZqPhOpT0ybCMidt%2FmIMguTJgGiZZ7YcpRjgWhCeyODgAOxwppHZ2loz3LJujd1JT7Q%2Fl0B5ibUYMrOmeXBlXtlS6y80ZMcAVplDthwF4A%2BUGdrUvgRdE3kQWo2ccJ8tgfSsZRc48utvWrUDWTNbmTG2IK%2BDcJEzMFommV2V3vD6JzTheb5tFDeFnp1WoyfaI0ygPK9HuFzP3mz3ITTyHjUWbJlstiB47i9LBu7KrLOrUghe3DU04ef44G4JcEOkX7gixz97Zjg8CKTnwN1yonTYnPKZ0vy3brYADx7z2gDNoll5zmp%2BVVbXIkzwTmfUPdokXqbTzXY8Wx%2BJdu00DvrNymU%2B3XIUgdtzorvCK7Tj2G%2BonNlyDIRG9JTqD3P9CHJPV62edRx4%2BBX4Z5QNEObbVR%2FLBKzKzVlM43ruMZaZdXPh7VjxMaBWptUtRwf4odYQnTTXcm8Yhsgz32rkhDupCaQXW%2F7BkblwC%2FJqjCz%2F4MiMuKvSgwVuKzYcoQe%2FIXoCcpNZYO4s4hi4VY9EDkcmxS0IcMeVgnBlMtxVqQH3pmJbNN62HAimKvKMtxo54c5jAumLlX8gO%2FaK0gHK38KDH61XpID8c%2BS90PwJjibh%2BZK1IcRIlgHbMvtuOerB7WpQxNfnaYGywZlbx4JOY6Iz30iUZy0c4dePW6wvWzvxVabWzV4TgiokBlRqEioXI2luqDSF19hp%2BQtMW%2BR5bTVywr2yBNLLLffgyCy3BXkJxpZ7cGVm21UpwgL9spYi9Ai1Gj3RvlgC5YWXe3BkltuCwFZIuQdXprxdlTZwb%2Bt1Jt%2FaZRlCj02ryAn3zBJIz1vuoVEVINuPn1%2FA344q9%2BDIpLj5yz0IkicDomXG23KUgt8LK3xvraHhiGnzhcY0w1krmmGUMGkQ6E2%2Bt7Zq9WVKP7yd0mD%2F4TXcwshKq4bA0%2FmYHUeb79L00BqWFg%2BTWW%2BzV4AYIUIG5Mpkt%2BXoAf8eW2XyFGJbEXrEWkVOuDeXQHrWQhAfo3jjQfu5z%2ByzPSF6OgZ%2B%2BIwOSixTVO%2BD1Asj2nRXpo8tyBUhUtoMZJaJZlelMrz%2BCn3y9AhbF3tuW0VPtIOYQHleB3HNmplVHxyZX7YgQGbLkAGzMsnsqhSB2y2siw7EtbwStiX07LaKHDfMsvwdiHS3v2MOzEUp%2BtBvmGXpw09xgmZGUYC2fpbOxS78dWV%2B2qz1FHoLiwG0MhNtORLn9jQYpujpagtGHYUvusYCO0Rvcudu1eoLFU6A3sANMFtom8cgQfHwE9U9GB%2BiN4p%2F15XpaPPXTxglRSrwAl5I4F2MKnADryZ8W0PLT%2BDoQs9nq8gJd%2FFC0pTU4cWWVHA1CanLcRmMK6mgKzJX7Kr0YIFbdx1L7EltFT3RnlkC5aWWUXBlHtmCQHZUGQU9W6xJjL0WNbj8Zt22O8EVejZbRU64VxaRvqxXdlDi%2FV0Sn04fmCcGuTJxbdH1EwhiZICzzF1bji4M2KaL4Yt46HU1voAYCyARvck9uYy3LLuIgisT1q6oiIKuyGS05UiaG0JnKKLgWkLPX0PkxiMoyw%2FBeM1yiyu4MqdsQb6DkcUVdEXmkV2VIizQQ%2Bu6Yk9OQ%2FTGAzAL5ulvWXjRBVcmli0IhEUUXdAVmV52VdrAi8S6hflEhTsTQK9EnqWWkRsPxAyXBes185yxRgJaz0%2B%2Fq3RnQdM8noFB1VT6Ef1xmkUudPCYebsy7%2BGdQgfBzThJd%2FFLfPCiL9Xd2%2FicRuEBvOBwQF1FjYFFCMAd8AYvRJAReKf0FZYoAK%2FdeVuEB0xo64NcGiECNQio3g83IWgFxXyHjBa7IHzZ5RzScz6%2F5k1Hs6QGfjAmTbpj4SGXOrw0jEa0hdCnIu9HpmMshODDkhYyq4VY7UMuxFpImbj6BQkWpVc9oWIEZNtJwzQKvqsKPWcZ8ChtzsjqQi98eg155zfr8yijuM4J0%2BZfXj7P8kEnkLTxCdg%2B3G4hFbpZfDuEzyHcJaY8wco46AB45T70XhIPeitWD9rKUVZwI4rC4hzdtnSCB2OQbS2f1Zw2oxktm9EdY23ppl5OtvSGDemOu7Ytx1YVGxlR04Q0RYT9lEJiiDUXkJ%2BBjLcpxFSkNA0RMVwpHHNe4gjOy1xFV1rs1ZqQpBlNV0m73HgPFvbll92TLyVna3wB%2BHyEH8%2F76MZP47oq%2FgYR7kt8CtE6trb8wnU1jY9N6yGOEi3mY3v1B0jCMNau6Ri2qtmmabpFEQiCWArfDxwnTNsydAt%2Bp5iUyiAjhZK3hX8dT5JSbR1PWuR2rtqJCk%2FnaeHjIy5pa5w1KZDQ5f3DMEXHSPQPsDQIgaWsVf%2FD3J6DV5E9JW2oS4crzVi351AYYKmWtnaaXoApMcvQejKnZO8yMGsUVjlg8FUdU1EMxyqqZtKxSnEMV7EN1zat6aCqcP4MgCqCcN4nVFUmNB6sdBesVSw1%2BzeWMDo5WJlLByv23EpVtLXa9CROClVWT9aUzF0GVP0eeP4OTrAETKzcPmDVmFi5k4nDHo5Wbfm8T7SaaGI1M1YVjLgqrFJVt4lWAJoKBJkBrMzexlBwdxlglfl%2F3htWmdpgrCKIR2LVYrHKUPtiEsPuHhNvDyX7ugvTAEae4f3XxMOMCrnugyRPT%2BgQsxcNMyhdKSNKDWBTKyQrdnmqLXBTAbpp3Uox0p6MnlwmIX%2BNy7%2BFzwEEJSzkEeW36wEPhiDA2JR64CdJfr1N4uPvSNdyqfhxFHnHU5gNXvBOEvjn5ASM6mtwyjzHypDAyiF4fajtyVjZQAGsf5%2FjtNqZUV6v9BsFFSYqbhQA0XzCviep0ed4E0bQhe8dwA%2BUP4MNlBNUv2fIlWEjp0oYOZXmwGma6tponthg87uh%2B2qWKTVras3axz%2FzORwao2Ab69dx6qW16y2snlW7DrZh%2FTKK%2FR%2FlCNecIapEx8053QHJlnkIg7RWYWutbSv44nRCrb3GSInRZJiq2M5awU5mmY5jvWMlZred%2B2%2FAoLZBorOteJOZw2%2Bb8ka5a%2B2fmV3m93ODVc2OSEle92jI%2FBiM5rZpOGBgB5MPzTTankcsWAWA17JsVbHgbBrMrCfzPJrDgyQE8byP%2BbG7btqCplhDZ8g4KddZm2DR47g6MEzHwWt%2BDp4j8%2B8coieSvJ%2BtLiM2Dll1Ney7uYFkFu%2BHm5zAZ4F1vmW7lqpapmI4ptHeFIFvJGraC%2FjZGtmJa4JljVVsRJp7m53ch8o2FkKyvTQWrkW%2FvSaEJOn2oYO5bpFSPfveUzmEMK3CIdS0lFbBZRXq2q3%2FFaHy3iZiGpQk6Entg7659P1IdIx9EGoUSvsQvDe7aQ%2BOwr%2B0F2MP9PNI3o8Ex9iDXHLMbQ8uHv6ayx5KF6G0h257GJK7I%2B1hjD2oCu0IhUkNgl7Q%2F%2F2IcIxBEE7bkwYxrUGozqVGCHpFyvcjwjEGQTiuRxrEtAZhDoinijEI6YhlGoQrXU6zG4R9qTW1NIgeBiF9TGMNgrTphmoQ%2BoD9I2IMQhbEYBuEdDLNPkK4tH0okxqEjNKxDULGrmc3CEe7UNi6qNsmDYJiEEPKukiDGGUQhs6fBCUmLqdJg2DubpI1xEYaRFG5vNsgsoKY2LTpUsE5VcYi2EYhqvjX%2B%2BHmLEbh4FVaZzMKGbFmGwWhkrQ0iumNoqxsPLdRyF1NbJsYErSWNjHaJkzjQjs55M5Xtk040iYuYROGeqFS3TIwwbaJIXs5pE2MtgnNvdAqW8YmmDZhD9nOIW1itE2o1oUW2XJHB9smhuzokDYx2iYUvF7BXDYh98GybUKTNnEBm3CVCy2xZRSbbRJDtnVIkxhrErZ9oRW2Jk2CaRIyjn0Jk7AutftPRrHZJiGj2GNNwsRKdAywELMQ1twWIkPabAuRIe2xuwF1h1TPuUcE70JWocrlBdssZFR7dF4Rscx50ywc1cTL%2BarOhfxQqnREsc1CBrZnOm4Xq1JrCFlj9Kwb7BKqOC%2FqmLgRx5mwz7%2FCqjVryoAKEX0Z7Syc0WMOuepz0tiMrHYXzurxx7XyH5s7IcONQraLZfiUjFbtGRndC62bAy6jrH6LV9ghJa8CDikpjiilHlEyIxd7QfE4Lgo4JKMH1%2BziQIGSawN2T%2FflGheqXtfBDmbbyltnazi4gtqD1vRJHKe17z4Cduw%2Bx1u4Znn4Pw%3D%3D%3C%2Fdiagram%3E%3C%2Fmxfile%3E)] | sequence-diagram-1.drawio |
| Sequence Diagram 2 | Draw.io | [INSERT LINK] | sequence-diagram-2.drawio |
| Entity Relationship Diagram | Data modeler | [INSERT LINK] | erd-diagram.drawio |
| Flutter Wireframes | Figma | [[INSERT LINK](https://www.figma.com/proto/pG7mL3Oj2Rr37JAT9r7WJv/School-Parental-system?node-id=42-238&p=f&t=DpasXlu4t6tlwa70-1&scaling=scale-down&content-scaling=fixed&page-id=0%3A1&starting-point-node-id=42%3A238)] | N/A |
| ASP.NET Wireframes | Figma | [[INSERT LINK](https://www.figma.com/proto/lPObV3qt8TfARP7lGBJZqU/School-Parental-System?node-id=291-2865&p=f&viewport=419%2C205%2C0.09&t=W84g0s5dosnJV2S6-1&scaling=contain&content-scaling=fixed&starting-point-node-id=284%3A2529&page-id=218%3A3)] | N/A |

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
