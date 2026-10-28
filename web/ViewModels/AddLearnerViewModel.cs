using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : AddLearnerViewModel
// Purpose          : Holds input and validation data for creating learner view model.
// Re-use           : Passed between Learner controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AddLearnerViewModel object.
//
public class AddLearnerViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    //
    // Property Name    : DateOfBirth
    // Purpose          : Gets or sets the date of birth.
    // Data Type        : DateTime?
    //
    public DateTime? DateOfBirth { get; set; }

    [Required]
    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";

    [Required]
    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";

    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }


    [Required(ErrorMessage = "Class is required.")]
    [Display(Name = "Class")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid?
    //
    public Guid? ClassId { get; set; }

    //
    // Property Name    : AvailableClasses
    // Purpose          : Collection of available classes used by this model.
    // Data Type        : List<ClassOptionViewModel>
    //
    public List<ClassOptionViewModel> AvailableClasses { get; set; } = [];

    [Display(Name = "Guardian Name")]
    //
    // Property Name    : GuardianName
    // Purpose          : Gets or sets the guardian name.
    // Data Type        : string?
    //
    public string? GuardianName { get; set; }

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Please select a parent or guardian.")]
    [Display(Name = "Parent / Guardian")]
    //
    // Property Name    : ParentId
    // Purpose          : Identifies the related parent record.
    // Data Type        : Guid?
    //
    public Guid? ParentId { get; set; }

    [Required(ErrorMessage = "Please select the relationship.")]
    //
    // Property Name    : Relationship
    // Purpose          : Gets or sets the relationship.
    // Data Type        : string
    //
    public string Relationship { get; set; } = "";
}
//
// Class Name       : ClassOptionViewModel
// Purpose          : Supplies a selectable class option view model item to a Razor view.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassOptionViewModel object.
//
public class ClassOptionViewModel
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
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";
}



//
// Class Name       : ParentSearchResultViewModel
// Purpose          : Carries the result of parent search result view model to a view.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentSearchResultViewModel object.
//
public class ParentSearchResultViewModel
{
    [JsonPropertyName("parent_id")]
    //
    // Property Name    : ParentId
    // Purpose          : Identifies the related parent record.
    // Data Type        : Guid
    //
    public Guid ParentId { get; set; }

    [JsonPropertyName("guardian_number")]
    //
    // Property Name    : GuardianNumber
    // Purpose          : Gets or sets the guardian number.
    // Data Type        : string
    //
    public string GuardianNumber { get; set; } = "";

    [JsonPropertyName("full_name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    [JsonPropertyName("phone")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }

    [JsonPropertyName("email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }
}