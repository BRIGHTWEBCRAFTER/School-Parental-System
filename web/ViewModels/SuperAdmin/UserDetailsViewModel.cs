namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : UserDetailsViewModel
// Purpose          : Supplies user details view model information to MVC views and actions.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : UserDetailsViewModel object.
//
public class UserDetailsViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

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
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "";

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime?
    //
    public DateTime? CreatedAt { get; set; }
}