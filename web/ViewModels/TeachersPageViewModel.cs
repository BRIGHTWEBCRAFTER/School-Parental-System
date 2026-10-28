namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : TeachersPageViewModel
// Purpose          : Combines data displayed on the teachers page view model page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeachersPageViewModel object.
//
public class TeachersPageViewModel
{
    //
    // Property Name    : TotalTeachers
    // Purpose          : Total teachers displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalTeachers { get; set; }

    //
    // Property Name    : ActiveTeachers
    // Purpose          : Gets or sets the active teachers.
    // Data Type        : int
    //
    public int ActiveTeachers { get; set; }

    //
    // Property Name    : InactiveTeachers
    // Purpose          : Gets or sets the inactive teachers.
    // Data Type        : int
    //
    public int InactiveTeachers { get; set; }

    //
    // Property Name    : Teachers
    // Purpose          : Collection of teachers used by this model.
    // Data Type        : List<TeacherListItemViewModel>
    //
    public List<TeacherListItemViewModel> Teachers { get; set; } = [];
}


//
// Class Name       : TeacherListItemViewModel
// Purpose          : Describes one teacher list item view model row shown in a page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherListItemViewModel object.
//
public class TeacherListItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : EmployeeId
    // Purpose          : Identifies the related employee record.
    // Data Type        : string
    //
    public string EmployeeId { get; set; } = "";

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string
    //
    public string Phone { get; set; } = "";

    //
    // Property Name    : Specialization
    // Purpose          : Gets or sets the specialization.
    // Data Type        : string
    //
    public string Specialization { get; set; } = "";

    //
    // Property Name    : JoiningDate
    // Purpose          : Date or time associated with joining date.
    // Data Type        : DateTime?
    //
    public DateTime? JoiningDate { get; set; }

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}