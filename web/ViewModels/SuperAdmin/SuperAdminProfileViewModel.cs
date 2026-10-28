namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;


//
// Class Name       : SuperAdminProfileViewModel
// Purpose          : Supplies super admin profile view model information to MVC views and actions.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminProfileViewModel object.
//
public class SuperAdminProfileViewModel
{
    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }

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

    // Profile photo resolved from the owned Supabase avatars file.
    public string? ProfilePicture { get; set; }

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


    // =========================================
    // PROFILE UPDATE
    // =========================================

    //
    // Property Name    : UpdatedFullName
    // Purpose          : Gets or sets the updated full name.
    // Data Type        : string
    //
    public string UpdatedFullName { get; set; } = "";

    //
    // Property Name    : UpdatedPhone
    // Purpose          : Gets or sets the updated phone.
    // Data Type        : string
    //
    public string UpdatedPhone { get; set; } = "";
}