namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : SchoolAdminDashboardViewModel
// Purpose          : Combines data displayed on the school admin dashboard view model page.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminDashboardViewModel object.
//
public class SchoolAdminDashboardViewModel
{
    //
    // Property Name    : AdminName
    // Purpose          : Gets or sets the admin name.
    // Data Type        : string
    //
    public string AdminName { get; set; } = "";
    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : TotalLearners
    // Purpose          : Total learners displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalLearners { get; set; }
    //
    // Property Name    : TotalTeachers
    // Purpose          : Total teachers displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalTeachers { get; set; }
    //
    // Property Name    : TotalClasses
    // Purpose          : Total classes displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalClasses { get; set; }

    //
    // Property Name    : AttendanceRate
    // Purpose          : Computed or stored attendance rate value.
    // Data Type        : decimal
    //
    public decimal AttendanceRate { get; set; }
    //
    // Property Name    : ActiveAnnouncements
    // Purpose          : Gets or sets the active announcements.
    // Data Type        : int
    //
    public int ActiveAnnouncements { get; set; }

    //
    // Property Name    : AttendanceOverview
    // Purpose          : Collection of attendance overview used by this model.
    // Data Type        : List<AttendanceOverviewItem>
    //
    public List<AttendanceOverviewItem> AttendanceOverview { get; set; } = [];
    //
    // Property Name    : RecentAnnouncements
    // Purpose          : Collection of recent announcements used by this model.
    // Data Type        : List<AnnouncementItem>
    //
    public List<AnnouncementItem> RecentAnnouncements { get; set; } = [];
    //
    // Property Name    : UpcomingEvents
    // Purpose          : Collection of upcoming events used by this model.
    // Data Type        : List<EventItem>
    //
    public List<EventItem> UpcomingEvents { get; set; } = [];
    //
    // Property Name    : RecentActivities
    // Purpose          : Collection of recent activities used by this model.
    // Data Type        : List<ActivityItem>
    //
    public List<ActivityItem> RecentActivities { get; set; } = [];
    //
    // Property Name    : ClassPerformance
    // Purpose          : Collection of class performance used by this model.
    // Data Type        : List<ClassPerformanceItem>
    //
    public List<ClassPerformanceItem> ClassPerformance { get; set; } = [];
}

//
// Class Name       : AttendanceOverviewItem
// Purpose          : Supplies attendance overview item information to MVC views and actions.
// Re-use           : Passed between Attendance controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceOverviewItem object.
//
public class AttendanceOverviewItem
{
    //
    // Property Name    : Date
    // Purpose          : Date or time associated with date.
    // Data Type        : string
    //
    public string Date { get; set; } = "";
    //
    // Property Name    : Percentage
    // Purpose          : Computed or stored percentage value.
    // Data Type        : decimal
    //
    public decimal Percentage { get; set; }
}

//
// Class Name       : AnnouncementItem
// Purpose          : Supplies announcement item information to MVC views and actions.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementItem object.
//
public class AnnouncementItem
{
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";
}

//
// Class Name       : EventItem
// Purpose          : Supplies event item information to MVC views and actions.
// Re-use           : Passed between Events controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EventItem object.
//
public class EventItem
{
    //
    // Property Name    : Month
    // Purpose          : Gets or sets the month.
    // Data Type        : string
    //
    public string Month { get; set; } = "";
    //
    // Property Name    : Day
    // Purpose          : Gets or sets the day.
    // Data Type        : string
    //
    public string Day { get; set; } = "";
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";
}

//
// Class Name       : ActivityItem
// Purpose          : Supplies activity item information to MVC views and actions.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ActivityItem object.
//
public class ActivityItem
{
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";
    //
    // Property Name    : TimeAgo
    // Purpose          : Gets or sets the time ago.
    // Data Type        : string
    //
    public string TimeAgo { get; set; } = "";
    //
    // Property Name    : Type
    // Purpose          : Gets or sets the type.
    // Data Type        : string
    //
    public string Type { get; set; } = "";
}

//
// Class Name       : ClassPerformanceItem
// Purpose          : Supplies class performance item information to MVC views and actions.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassPerformanceItem object.
//
public class ClassPerformanceItem
{
    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";
    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }
    //
    // Property Name    : AverageGrade
    // Purpose          : Gets or sets the average grade.
    // Data Type        : decimal
    //
    public decimal AverageGrade { get; set; }
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";
}