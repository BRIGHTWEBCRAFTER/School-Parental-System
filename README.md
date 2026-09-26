# School-Parental System (School Portal System)

## ITC327W – Phase 2 Project

### Group: Bright Web Crafters

---

## Project Overview

The School-Parental System is a centralized, web and mobile-based platform designed to streamline school management for educational institutions. The system provides a single platform for administrators, teachers, and parents to manage and access student information, including attendance, grades, and announcements, in real time.

By replacing manual, paper-based processes, the system aims to improve data accuracy, enhance parent-school communication, and support better academic decision-making.

### Participating Schools
- Reitzpark Primary School
- Lenakeng Technical School
- Thabong Primary School

---

## Technology Stack

| Component | Technology |

| Mobile Application | Flutter |
| Web Application / API | ASP.NET Core 8 Web API |
| Backend / Database | Supabase (PostgreSQL) |
| Authentication | Supabase Auth (JWT) |
| Storage | Supabase Storage |
| Security | Row Level Security (RLS), Role-Based Access Control (RBAC) |

---

## System Architecture

The system consists of three integrated components:

1. **Flutter Mobile Application** – Provides mobile functionality for parents and students (view attendance, grades, announcements, notifications).
2. **ASP.NET Core Web Application** – Provides web functionality for Super Admins, School Admins, and Teachers (manage schools, learners, teachers, classes, attendance, grades, communication).
3. **Supabase Backend** – Shared backend providing authentication, PostgreSQL database, storage, and Row Level Security.

Both applications communicate with the shared Supabase backend via HTTPS/API requests and receive JSON responses.

**Architecture Diagram:** [View on Draw.io](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#Uhttps%3A%2F%2Fdrive.google.com%2Fuc%3Fid%3D1CIYwvn3JjYWEF-mdmsHK64CJf6675N-G%26export%3Ddownload)

---

## User Roles

| Role | Access Level |
|------|-------------|
| Super Admin | Manages multiple schools, system-wide data, all users |
| School Admin | Manages their own school (learners, teachers, classes, attendance, grades, communication) |
| Teacher | Views assigned classes, records attendance, captures grades, creates class announcements |
| Parent | Views their children's attendance, grades, announcements, events; acknowledges important notices |

---

## Documentation

### Software Requirements Specification (SRS)
The complete SRS is available in the project documentation folder. It includes:
- Problem Statement
- Introduction, Scope, Aims, Objectives
- Stakeholder Profile and Current Process
- Functional and Non-Functional Requirements
- Data Requirements
- Risk Analysis and Feasibility Study
- SWOT Analysis
- Requirements Change Log
- Refined Project Scope

**Phase 2 additions include:**
- System Architecture Design
- UML Diagrams (Use Case, Sequence)
- Entity Relationship Diagram (ERD)
- Flutter and ASP.NET Interface Designs
- Design Alignment Table

### UML Diagrams

| Diagram | Tool | Source Link |
|---------|------|-------------|
| Use Case Diagram | Draw.io | [View](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#R%3Cmxfile%3E...) |
| Sequence Diagram | Draw.io | [View](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#R%3Cmxfile%3E...) |
| ERD | Data Modeler | [View](Logical.pdf) |
| System Architecture | Draw.io | [View](https://viewer.diagrams.net/?tags=%7B%7D&lightbox=1&highlight=0000ff&edit=_blank&layers=1&nav=1&dark=auto#Uhttps%3A%2F%2Fdrive.google.com%2Fuc%3Fid%3D1CIYwvn3JjYWEF-mdmsHK64CJf6675N-G%26export%3Ddownload) |

### Interface Designs (Figma)

| Design | Tool | Source Link |
|--------|------|-------------|
| Flutter Mobile Wireframes | Figma | [View Prototype](https://www.figma.com/proto/pG7mL3Oj2Rr37JAT9r7WJv/School-Parental-system?node-id=42-238&p=f&t=0WVSbhhXYOH6dG98-1&scaling=scale-down&content-scaling=fixed&page-id=0%3A1&starting-point-node-id=42%3A238) |
| ASP.NET Web Wireframes | Figma | [View Prototype](https://www.figma.com/proto/lPObV3qt8TfARP7lGBJZqU/School-Parental-System?node-id=224-463&p=f&viewport=-107%2C84%2C0.19&t=7iRvLo52rvJGZx2o-1&scaling=contain&content-scaling=fixed&page-id=218%3A2&starting-point-node-id=281%3A2266) |

---

## Database Schema

The Supabase PostgreSQL database contains the following main entities:

| Entity | Description |
|--------|-------------|
| Schools | School information (name, address, contact details) |
| Users | All system users with role assignments |
| Learners | Student records (admission number, name, grade, parent link) |
| Teachers | Teacher records (employee ID, specialization, assigned classes) |
| Classes | Class information (grade, section, teacher, room) |
| Enrollments | Learner-class enrollment records |
| Attendance | Daily attendance records (present, absent, late, excused) |
| Grades | Assessment grades with percentage and performance level |
| Announcements | School announcements with priority and target audience |
| Announcement Recipients | Read/unread and acknowledgement tracking |
| Notifications | User notifications |
| Events | School events (meetings, sports, exams) |

**ERD:** [View Full ERD](Logical.pdf)

---

## GitHub Repository Structure
