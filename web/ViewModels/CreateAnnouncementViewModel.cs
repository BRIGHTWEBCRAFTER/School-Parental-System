using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : CreateAnnouncementViewModel
// Purpose          : Holds input and validation data for creating announcement view model.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : CreateAnnouncementViewModel object.
//
public class CreateAnnouncementViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150)]
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";


    [Required(ErrorMessage = "Message is required.")]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";


    [Required(ErrorMessage = "Priority is required.")]
    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";


    [Required(ErrorMessage = "Target audience is required.")]
    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "Whole School";


    //
    // Property Name    : TargetRole
    // Purpose          : Gets or sets the target role.
    // Data Type        : string?
    //
    public string? TargetRole { get; set; }

    //
    // Property Name    : TargetGrade
    // Purpose          : Gets or sets the target grade.
    // Data Type        : string?
    //
    public string? TargetGrade { get; set; }

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }


    [Display(Name = "Require parent acknowledgement")]
    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }


    [Display(Name = "Expiry date")]
    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }


    //
    // Property Name    : SelectedUserIds
    // Purpose          : Identifies the associated selected user records.
    // Data Type        : List<Guid>
    //
    public List<Guid> SelectedUserIds { get; set; } = [];


    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<AnnouncementClassOptionViewModel>
    //
    public List<AnnouncementClassOptionViewModel>
        Classes
    { get; set; } = [];


    //
    // Property Name    : Users
    // Purpose          : Collection of users used by this model.
    // Data Type        : List<AnnouncementUserOptionViewModel>
    //
    public List<AnnouncementUserOptionViewModel>
        Users
    { get; set; } = [];


    //
    // Property Name    : Grades
    // Purpose          : Collection of grades used by this model.
    // Data Type        : List<string>
    //
    public List<string> Grades { get; set; } = [];
}


//
// Class Name       : AnnouncementClassOptionViewModel
// Purpose          : Supplies a selectable announcement class option view model item to a Razor view.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementClassOptionViewModel object.
//
public class AnnouncementClassOptionViewModel
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
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    //
    // Property Name    : DisplayName
    // Purpose          : Gets or sets the display name.
    // Data Type        : string
    //
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Code)
            ? Name
            : $"{Name} ({Code})";
}


//
// Class Name       : AnnouncementUserOptionViewModel
// Purpose          : Supplies a selectable announcement user option view model item to a Razor view.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementUserOptionViewModel object.
//
public class AnnouncementUserOptionViewModel
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
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : DisplayName
    // Purpose          : Gets or sets the display name.
    // Data Type        : string
    //
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Email)
            ? $"{FullName} — {Role}"
            : $"{FullName} — {Role} — {Email}";
}