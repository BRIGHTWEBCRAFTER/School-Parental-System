namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : UserManagementViewModel
// Purpose          : Supplies user management view model information to MVC views and actions.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : UserManagementViewModel object.
//
public class UserManagementViewModel
{
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
    // Property Name    : Users
    // Purpose          : Collection of users used by this model.
    // Data Type        : List<SuperAdminUserItemViewModel>
    //
    public List<SuperAdminUserItemViewModel>
        Users
    { get; set; } = [];
}

//
// Class Name       : SuperAdminUserItemViewModel
// Purpose          : Describes one super admin user item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminUserItemViewModel object.
//
public class SuperAdminUserItemViewModel
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
}