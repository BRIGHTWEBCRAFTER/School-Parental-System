namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : SystemActivityViewModel
// Purpose          : Supplies system activity view model information to MVC views and actions.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SystemActivityViewModel object.
//
public class SystemActivityViewModel
{
    //
    // Property Name    : Activities
    // Purpose          : Collection of activities used by this model.
    // Data Type        : List<SuperAdminActivityItemViewModel>
    //
    public List<SuperAdminActivityItemViewModel>
        Activities
    { get; set; } = [];
}