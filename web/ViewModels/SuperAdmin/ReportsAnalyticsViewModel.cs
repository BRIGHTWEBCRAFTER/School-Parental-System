namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : ReportsAnalyticsViewModel
// Purpose          : Supplies reports analytics view model information to MVC views and actions.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ReportsAnalyticsViewModel object.
//
public class ReportsAnalyticsViewModel
{
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
    // Property Name    : TotalUsers
    // Purpose          : Total users displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalUsers { get; set; }

    //
    // Property Name    : ActiveUsers
    // Purpose          : Gets or sets the active users.
    // Data Type        : int
    //
    public int ActiveUsers { get; set; }

    //
    // Property Name    : InactiveUsers
    // Purpose          : Gets or sets the inactive users.
    // Data Type        : int
    //
    public int InactiveUsers { get; set; }

    //
    // Property Name    : SchoolAdmins
    // Purpose          : Gets or sets the school admins.
    // Data Type        : int
    //
    public int SchoolAdmins { get; set; }

    //
    // Property Name    : Teachers
    // Purpose          : Gets or sets the teachers.
    // Data Type        : int
    //
    public int Teachers { get; set; }

    //
    // Property Name    : Parents
    // Purpose          : Gets or sets the parents.
    // Data Type        : int
    //
    public int Parents { get; set; }

    //
    // Property Name    : TotalEvents
    // Purpose          : Total events displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalEvents { get; set; }

    //
    // Property Name    : TotalAnnouncements
    // Purpose          : Total announcements displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalAnnouncements { get; set; }

    //
    // Property Name    : Schools
    // Purpose          : Collection of schools used by this model.
    // Data Type        : List<SchoolAnalyticsItemViewModel>
    //
    public List<SchoolAnalyticsItemViewModel>
        Schools
    { get; set; } = [];
}


//
// Class Name       : SchoolAnalyticsItemViewModel
// Purpose          : Describes one school analytics item view model row shown in a page.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAnalyticsItemViewModel object.
//
public class SchoolAnalyticsItemViewModel
{
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    //
    // Property Name    : TotalUsers
    // Purpose          : Total users displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalUsers { get; set; }

    //
    // Property Name    : ActiveUsers
    // Purpose          : Gets or sets the active users.
    // Data Type        : int
    //
    public int ActiveUsers { get; set; }

    //
    // Property Name    : InactiveUsers
    // Purpose          : Gets or sets the inactive users.
    // Data Type        : int
    //
    public int InactiveUsers { get; set; }

    //
    // Property Name    : Teachers
    // Purpose          : Gets or sets the teachers.
    // Data Type        : int
    //
    public int Teachers { get; set; }

    //
    // Property Name    : Parents
    // Purpose          : Gets or sets the parents.
    // Data Type        : int
    //
    public int Parents { get; set; }

    //
    // Property Name    : SchoolAdmins
    // Purpose          : Gets or sets the school admins.
    // Data Type        : int
    //
    public int SchoolAdmins { get; set; }
}