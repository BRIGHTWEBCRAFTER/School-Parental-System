namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ParentsPageViewModel
// Purpose          : Combines data displayed on the parents page view model page.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentsPageViewModel object.
//
public class ParentsPageViewModel
{
    //
    // Property Name    : TotalParents
    // Purpose          : Total parents displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalParents { get; set; }

    //
    // Property Name    : ActiveParents
    // Purpose          : Gets or sets the active parents.
    // Data Type        : int
    //
    public int ActiveParents { get; set; }

    //
    // Property Name    : InactiveParents
    // Purpose          : Gets or sets the inactive parents.
    // Data Type        : int
    //
    public int InactiveParents { get; set; }

    //
    // Property Name    : Parents
    // Purpose          : Collection of parents used by this model.
    // Data Type        : List<ParentListItemViewModel>
    //
    public List<ParentListItemViewModel> Parents { get; set; } = [];
}


//
// Class Name       : ParentListItemViewModel
// Purpose          : Describes one parent list item view model row shown in a page.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentListItemViewModel object.
//
public class ParentListItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : GuardianNumber
    // Purpose          : Gets or sets the guardian number.
    // Data Type        : string
    //
    public string GuardianNumber { get; set; } = "";

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
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";

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