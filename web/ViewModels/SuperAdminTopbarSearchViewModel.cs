using SchoolParentalSystem.Web.ViewModels.SuperAdmin;
namespace SchoolParentalSystem.Web.ViewModels;

// Class Name: SuperAdminTopbarSearchViewModel
// Purpose: Present school and user search results from the Super Admin top bar.
public class SuperAdminTopbarSearchViewModel
{
    public string Query { get; set; } = "";
    public List<SuperAdminSchoolItemViewModel> Schools { get; set; } = new();
    public List<SuperAdminUserItemViewModel> Users { get; set; } = new();
    public bool LoadFailed { get; set; }
}
