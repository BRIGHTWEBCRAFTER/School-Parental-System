namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : SuperAdminDashboardViewModel
// Purpose          : Combines data displayed on the super admin dashboard view model page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminDashboardViewModel object.
//
public class SuperAdminDashboardViewModel
{
    //
    // Property Name    : AdminName
    // Purpose          : Gets or sets the admin name.
    // Data Type        : string
    //
    public string AdminName { get; set; } = "";


    // -----------------------------------------------------
    // SYSTEM STATISTICS
    // -----------------------------------------------------

    //
    // Property Name    : TotalSchools
    // Purpose          : Total schools displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalSchools { get; set; }

    //
    // Property Name    : ActiveSchools
    // Purpose          : Gets or sets the active schools.
    // Data Type        : int
    //
    public int ActiveSchools { get; set; }

    //
    // Property Name    : PendingSchools
    // Purpose          : Gets or sets the pending schools.
    // Data Type        : int
    //
    public int PendingSchools { get; set; }

    //
    // Property Name    : InactiveSchools
    // Purpose          : Gets or sets the inactive schools.
    // Data Type        : int
    //
    public int InactiveSchools { get; set; }

    //
    // Property Name    : TotalLearners
    // Purpose          : Total learners displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalLearners { get; set; }

    //
    // Property Name    : TotalParents
    // Purpose          : Total parents displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalParents { get; set; }

    //
    // Property Name    : TotalTeachers
    // Purpose          : Total teachers displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalTeachers { get; set; }

    //
    // Property Name    : TotalSchoolAdmins
    // Purpose          : Total school admins displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalSchoolAdmins { get; set; }

    //
    // Property Name    : ActiveAnnouncements
    // Purpose          : Gets or sets the active announcements.
    // Data Type        : int
    //
    public int ActiveAnnouncements { get; set; }


    // -----------------------------------------------------
    // DASHBOARD SECTIONS
    // -----------------------------------------------------

    //
    // Property Name    : EnrollmentOverview
    // Purpose          : Collection of enrollment overview used by this model.
    // Data Type        : List<SystemEnrollmentItemViewModel>
    //
    public List<SystemEnrollmentItemViewModel>
        EnrollmentOverview
    { get; set; } = [];


    //
    // Property Name    : UpcomingEvents
    // Purpose          : Collection of upcoming events used by this model.
    // Data Type        : List<SuperAdminEventItemViewModel>
    //
    public List<SuperAdminEventItemViewModel>
        UpcomingEvents
    { get; set; } = [];


    //
    // Property Name    : RecentActivities
    // Purpose          : Collection of recent activities used by this model.
    // Data Type        : List<SuperAdminActivityItemViewModel>
    //
    public List<SuperAdminActivityItemViewModel>
        RecentActivities
    { get; set; } = [];


    //
    // Property Name    : RecentSchools
    // Purpose          : Collection of recent schools used by this model.
    // Data Type        : List<RecentSchoolItemViewModel>
    //
    public List<RecentSchoolItemViewModel>
        RecentSchools
    { get; set; } = [];
}


//
// Class Name       : SystemEnrollmentItemViewModel
// Purpose          : Describes one system enrollment item view model row shown in a page.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SystemEnrollmentItemViewModel object.
//
public class SystemEnrollmentItemViewModel
{
    //
    // Property Name    : Label
    // Purpose          : Gets or sets the label.
    // Data Type        : string
    //
    public string Label { get; set; } = "";

    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }
}


//
// Class Name       : SuperAdminEventItemViewModel
// Purpose          : Describes one super admin event item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminEventItemViewModel object.
//
public class SuperAdminEventItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

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

    //
    // Property Name    : Time
    // Purpose          : Date or time associated with time.
    // Data Type        : string
    //
    public string Time { get; set; } = "";

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";
}


//
// Class Name       : SuperAdminActivityItemViewModel
// Purpose          : Describes one super admin activity item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminActivityItemViewModel object.
//
public class SuperAdminActivityItemViewModel
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
// Class Name       : RecentSchoolItemViewModel
// Purpose          : Describes one recent school item view model row shown in a page.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : RecentSchoolItemViewModel object.
//
public class RecentSchoolItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : Location
    // Purpose          : Gets or sets the location.
    // Data Type        : string
    //
    public string Location { get; set; } = "";

    //
    // Property Name    : AdministratorName
    // Purpose          : Gets or sets the administrator name.
    // Data Type        : string
    //
    public string AdministratorName { get; set; } = "";

    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";
}