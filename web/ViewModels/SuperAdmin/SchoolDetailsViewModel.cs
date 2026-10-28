namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : SchoolDetailsViewModel
// Purpose          : Supplies school details view model information to MVC views and actions.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolDetailsViewModel object.
//
public class SchoolDetailsViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string
    //
    public string Address { get; set; } = "";

    //
    // Property Name    : SchoolCode
    // Purpose          : Gets or sets the school code.
    // Data Type        : string
    //
    public string SchoolCode { get; set; } = "";

    //
    // Property Name    : AdmissionPrefix
    // Purpose          : Gets or sets the admission prefix.
    // Data Type        : string
    //
    public string AdmissionPrefix { get; set; } = "";

    //
    // Property Name    : SchoolType
    // Purpose          : Gets or sets the school type.
    // Data Type        : string
    //
    public string SchoolType { get; set; } = "";

    //
    // Property Name    : Province
    // Purpose          : Gets or sets the province.
    // Data Type        : string
    //
    public string Province { get; set; } = "";

    //
    // Property Name    : TownCity
    // Purpose          : Gets or sets the town city.
    // Data Type        : string
    //
    public string TownCity { get; set; } = "";

    //
    // Property Name    : PrincipalName
    // Purpose          : Gets or sets the principal name.
    // Data Type        : string
    //
    public string PrincipalName { get; set; } = "";

    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string
    //
    public string Phone { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : MaxLearners
    // Purpose          : Gets or sets the max learners.
    // Data Type        : int?
    //
    public int? MaxLearners { get; set; }

    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    //
    // Property Name    : Notes
    // Purpose          : Gets or sets the notes.
    // Data Type        : string
    //
    public string Notes { get; set; } = "";

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}