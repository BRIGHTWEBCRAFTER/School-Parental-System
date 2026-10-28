
using System;
using System.Collections.Generic;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherDashboardViewModel
// Purpose          : Combines data displayed on the teacher dashboard view model page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDashboardViewModel object.
//
public class TeacherDashboardViewModel
{
    //
    // Property Name    : HasTeacherProfile
    // Purpose          : Indicates whether teacher profile is available.
    // Data Type        : bool
    //
    public bool HasTeacherProfile { get; set; }
    //
    // Property Name    : DataLoadFailed
    // Purpose          : Gets or sets the data load failed.
    // Data Type        : bool
    //
    public bool DataLoadFailed { get; set; }

    //
    // Property Name    : AttendanceLoadFailed
    // Purpose          : Gets or sets the attendance load failed.
    // Data Type        : bool
    //
    public bool AttendanceLoadFailed { get; set; }
    //
    // Property Name    : AnnouncementsLoadFailed
    // Purpose          : Gets or sets the announcements load failed.
    // Data Type        : bool
    //
    public bool AnnouncementsLoadFailed { get; set; }
    //
    // Property Name    : EventsLoadFailed
    // Purpose          : Gets or sets the events load failed.
    // Data Type        : bool
    //
    public bool EventsLoadFailed { get; set; }

    // ASSIGNED CLASSES

    //
    // Property Name    : AssignedClassCount
    // Purpose          : Computed or stored assigned class count value.
    // Data Type        : int
    //
    public int AssignedClassCount { get; set; }

    //
    // Property Name    : AssignedClassSummary
    // Purpose          : Gets or sets the assigned class summary.
    // Data Type        : string
    //
    public string AssignedClassSummary { get; set; }
        = "No class-teacher assignment yet";

    //
    // Property Name    : ClassesTaughtCount
    // Purpose          : Computed or stored classes taught count value.
    // Data Type        : int
    //
    public int ClassesTaughtCount { get; set; }

    //
    // Property Name    : ClassesTaughtSummary
    // Purpose          : Gets or sets the classes taught summary.
    // Data Type        : string
    //
    public string ClassesTaughtSummary { get; set; }
        = "No teaching assignments yet";


    // TODAY'S ATTENDANCE

    //
    // Property Name    : AttendanceTodayCount
    // Purpose          : Computed or stored attendance today count value.
    // Data Type        : int
    //
    public int AttendanceTodayCount { get; set; }

    //
    // Property Name    : PresentTodayCount
    // Purpose          : Computed or stored present today count value.
    // Data Type        : int
    //
    public int PresentTodayCount { get; set; }

    //
    // Property Name    : AbsentTodayCount
    // Purpose          : Computed or stored absent today count value.
    // Data Type        : int
    //
    public int AbsentTodayCount { get; set; }


    // COMMUNICATION

    //
    // Property Name    : ActiveAnnouncementsCount
    // Purpose          : Computed or stored active announcements count value.
    // Data Type        : int
    //
    public int ActiveAnnouncementsCount { get; set; }


    // DASHBOARD LISTS

    //
    // Property Name    : TeachingClasses
    // Purpose          : Collection of teaching classes used by this model.
    // Data Type        : List<TeacherDashboardClassItem>
    //
    public List<TeacherDashboardClassItem>
        TeachingClasses
    { get; set; } = new();

    //
    // Property Name    : AttendanceByClass
    // Purpose          : Collection of attendance by class used by this model.
    // Data Type        : List<TeacherDashboardAttendanceItem>
    //
    public List<TeacherDashboardAttendanceItem>
        AttendanceByClass
    { get; set; } = new();

    //
    // Property Name    : RecentAnnouncements
    // Purpose          : Collection of recent announcements used by this model.
    // Data Type        : List<TeacherDashboardAnnouncementItem>
    //
    public List<TeacherDashboardAnnouncementItem>
        RecentAnnouncements
    { get; set; } = new();

    //
    // Property Name    : UpcomingEvents
    // Purpose          : Collection of upcoming events used by this model.
    // Data Type        : List<TeacherDashboardEventItem>
    //
    public List<TeacherDashboardEventItem>
        UpcomingEvents
    { get; set; } = new();
}


// ==========================================
// CLASS ITEM
// ==========================================


//
// Class Name       : TeacherDashboardClassItem
// Purpose          : Combines data displayed on the teacher dashboard class item page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDashboardClassItem object.
//
public class TeacherDashboardClassItem
{
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : SubjectName
    // Purpose          : Stores the subject name.
    // Data Type        : string
    //
    public string SubjectName { get; set; } = "";

    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string
    //
    public string RoomNumber { get; set; } = "";

    // LIVE TIMETABLE FROM SCHOOL ADMIN
    //
    // Property Name    : Schedule
    // Purpose          : Gets or sets the schedule.
    // Data Type        : string
    //
    public string Schedule { get; set; } =
        "Not scheduled";
}



// ==========================================
// ATTENDANCE ITEM
// ==========================================

//
// Class Name       : TeacherDashboardAttendanceItem
// Purpose          : Combines data displayed on the teacher dashboard attendance item page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDashboardAttendanceItem object.
//
public class TeacherDashboardAttendanceItem
{
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : Total
    // Purpose          : Total  displayed or calculated for this record.
    // Data Type        : int
    //
    public int Total { get; set; }

    //
    // Property Name    : Present
    // Purpose          : Gets or sets the present.
    // Data Type        : int
    //
    public int Present { get; set; }

    //
    // Property Name    : Late
    // Purpose          : Gets or sets the late.
    // Data Type        : int
    //
    public int Late { get; set; }

    //
    // Property Name    : Absent
    // Purpose          : Gets or sets the absent.
    // Data Type        : int
    //
    public int Absent { get; set; }

    //
    // Property Name    : Excused
    // Purpose          : Gets or sets the excused.
    // Data Type        : int
    //
    public int Excused { get; set; }
}


// ==========================================
// ANNOUNCEMENT ITEM
// ==========================================

//
// Class Name       : TeacherDashboardAnnouncementItem
// Purpose          : Combines data displayed on the teacher dashboard announcement item page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDashboardAnnouncementItem object.
//
public class TeacherDashboardAnnouncementItem
{
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "";

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


// ==========================================
// EVENT ITEM
// ==========================================

//
// Class Name       : TeacherDashboardEventItem
// Purpose          : Combines data displayed on the teacher dashboard event item page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDashboardEventItem object.
//
public class TeacherDashboardEventItem
{
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string
    //
    public string Venue { get; set; } = "";

    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; }

    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }
}
