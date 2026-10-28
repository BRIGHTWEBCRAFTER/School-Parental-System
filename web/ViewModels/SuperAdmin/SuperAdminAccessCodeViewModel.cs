namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;


//
// Class Name       : SuperAdminAccessCodeViewModel
// Purpose          : Supplies super admin access code view model information to MVC views and actions.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminAccessCodeViewModel object.
//
public class SuperAdminAccessCodeViewModel
{
    // School selected by the Super Admin
    //
    // Property Name    : SelectedSchoolId
    // Purpose          : Identifies the related selected school record.
    // Data Type        : Guid?
    //
    public Guid? SelectedSchoolId { get; set; }


    // Active schools available for selection
    //
    // Property Name    : Schools
    // Purpose          : Collection of schools used by this model.
    // Data Type        : List<AccessCodeSchoolItemViewModel>
    //
    public List<AccessCodeSchoolItemViewModel> Schools { get; set; }
        = new();


    // Generated registration code.
    // This is only shown immediately after generation.
    //
    // Property Name    : GeneratedCode
    // Purpose          : Gets or sets the generated code.
    // Data Type        : string
    //
    public string GeneratedCode { get; set; } = "";


    // School the generated code belongs to
    //
    // Property Name    : GeneratedSchoolName
    // Purpose          : Gets or sets the generated school name.
    // Data Type        : string
    //
    public string GeneratedSchoolName { get; set; } = "";


    // When the code expires
    //
    // Property Name    : GeneratedCodeExpiresAt
    // Purpose          : Date or time associated with generated code expires at.
    // Data Type        : DateTime?
    //
    public DateTime? GeneratedCodeExpiresAt { get; set; }


    // Used to show success feedback
    //
    // Property Name    : CodeGeneratedSuccessfully
    // Purpose          : Gets or sets the code generated successfully.
    // Data Type        : bool
    //
    public bool CodeGeneratedSuccessfully { get; set; }
}


//
// Class Name       : AccessCodeSchoolItemViewModel
// Purpose          : Describes one access code school item view model row shown in a page.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AccessCodeSchoolItemViewModel object.
//
public class AccessCodeSchoolItemViewModel
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
}