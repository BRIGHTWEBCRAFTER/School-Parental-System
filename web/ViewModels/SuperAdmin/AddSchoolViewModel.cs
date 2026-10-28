using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : AddSchoolViewModel
// Purpose          : Holds input and validation data for creating school view model.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AddSchoolViewModel object.
//
public class AddSchoolViewModel
{
    [Required(ErrorMessage = "School name is required.")]
    [Display(Name = "School Name")]
    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";


    [Required(ErrorMessage = "School address is required.")]
    [Display(Name = "Address")]
    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string
    //
    public string Address { get; set; } = "";


    [Display(Name = "School Code")]
    //
    // Property Name    : SchoolCode
    // Purpose          : Gets or sets the school code.
    // Data Type        : string?
    //
    public string? SchoolCode { get; set; }


    [Display(Name = "Admission Prefix")]
    //
    // Property Name    : AdmissionPrefix
    // Purpose          : Gets or sets the admission prefix.
    // Data Type        : string?
    //
    public string? AdmissionPrefix { get; set; }


    [Display(Name = "School Type")]
    //
    // Property Name    : SchoolType
    // Purpose          : Gets or sets the school type.
    // Data Type        : string?
    //
    public string? SchoolType { get; set; }


    [Display(Name = "Province")]
    //
    // Property Name    : Province
    // Purpose          : Gets or sets the province.
    // Data Type        : string?
    //
    public string? Province { get; set; }


    [Display(Name = "Town / City")]
    //
    // Property Name    : TownCity
    // Purpose          : Gets or sets the town city.
    // Data Type        : string?
    //
    public string? TownCity { get; set; }


    [Display(Name = "Principal Name")]
    //
    // Property Name    : PrincipalName
    // Purpose          : Gets or sets the principal name.
    // Data Type        : string?
    //
    public string? PrincipalName { get; set; }


    [Phone]
    [Display(Name = "Phone")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }


    [EmailAddress]
    [Display(Name = "Email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }


    [Range(
        1,
        100000,
        ErrorMessage = "Maximum learners must be greater than zero.")]
    [Display(Name = "Maximum Learners")]
    //
    // Property Name    : MaxLearners
    // Purpose          : Gets or sets the max learners.
    // Data Type        : int?
    //
    public int? MaxLearners { get; set; }


    [Required]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "Active";


    [Display(Name = "Notes")]
    //
    // Property Name    : Notes
    // Purpose          : Gets or sets the notes.
    // Data Type        : string?
    //
    public string? Notes { get; set; }


    //
    // Property Name    : SchoolTypes
    // Purpose          : Collection of school types used by this model.
    // Data Type        : List<string>
    //
    public List<string> SchoolTypes { get; set; } =
    [
        "Primary School",
        "Secondary School",
        "Combined School",
        "Special School"
    ];


    //
    // Property Name    : Provinces
    // Purpose          : Collection of provinces used by this model.
    // Data Type        : List<string>
    //
    public List<string> Provinces { get; set; } =
    [
        "Eastern Cape",
        "Free State",
        "Gauteng",
        "KwaZulu-Natal",
        "Limpopo",
        "Mpumalanga",
        "Northern Cape",
        "North West",
        "Western Cape"
    ];


    //
    // Property Name    : Statuses
    // Purpose          : Collection of statuses used by this model.
    // Data Type        : List<string>
    //
    public List<string> Statuses { get; set; } =
    [
        "Active",
        "Pending",
        "Inactive"
    ];
}