namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : TeacherDetailsViewModel
// Purpose          : Supplies teacher details view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherDetailsViewModel object.
//
public class TeacherDetailsViewModel
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
    // Property Name    : DateOfBirth
    // Purpose          : Gets or sets the date of birth.
    // Data Type        : DateTime?
    //
    public DateTime? DateOfBirth { get; set; }

    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";

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
    // Property Name    : AlternativePhone
    // Purpose          : Gets or sets the alternative phone.
    // Data Type        : string
    //
    public string AlternativePhone { get; set; } = "";

    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string
    //
    public string Address { get; set; } = "";

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
        IsActive ? "Active" : "Inactive";
}