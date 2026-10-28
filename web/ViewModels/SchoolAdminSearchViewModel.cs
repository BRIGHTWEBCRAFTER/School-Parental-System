
namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : SchoolAdminSearchViewModel
// Purpose          : Supplies school admin search view model information to MVC views and actions.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminSearchViewModel object.
//
public class SchoolAdminSearchViewModel
{
    //
    // Property Name    : Query
    // Purpose          : Gets or sets the query.
    // Data Type        : string
    //
    public string Query { get; set; } = "";

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<SchoolAdminSearchItemViewModel>
    //
    public List<SchoolAdminSearchItemViewModel> Learners
    { get; set; } = new();

    //
    // Property Name    : Teachers
    // Purpose          : Collection of teachers used by this model.
    // Data Type        : List<SchoolAdminSearchItemViewModel>
    //
    public List<SchoolAdminSearchItemViewModel> Teachers
    { get; set; } = new();

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<SchoolAdminSearchItemViewModel>
    //
    public List<SchoolAdminSearchItemViewModel> Classes
    { get; set; } = new();

    //
    // Property Name    : TotalResults
    // Purpose          : Total results displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalResults =>
        Learners.Count +
        Teachers.Count +
        Classes.Count;
}

//
// Class Name       : SchoolAdminSearchItemViewModel
// Purpose          : Describes one school admin search item view model row shown in a page.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminSearchItemViewModel object.
//
public class SchoolAdminSearchItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";

    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";
}
