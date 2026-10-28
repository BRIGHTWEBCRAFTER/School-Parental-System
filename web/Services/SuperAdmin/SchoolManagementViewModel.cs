namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : SchoolManagementViewModel
// Purpose          : Holds school management view model data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

public class SchoolManagementViewModel
{
    public List<SuperAdminSchoolItemViewModel>
        Schools
    { get; set; } = [];
}


//
// Class Name       : SuperAdminSchoolItemViewModel
// Purpose          : Holds super admin school item view model data used by
//                    the application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

public class SuperAdminSchoolItemViewModel
{
    public Guid Id { get; set; }

    public string SchoolName { get; set; } = "";

    public string SchoolCode { get; set; } = "";

    public string SchoolType { get; set; } = "";

    public string Province { get; set; } = "";

    public string TownCity { get; set; } = "";

    public string PrincipalName { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Email { get; set; } = "";

    public int LearnerCount { get; set; }

    public string Status { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}