using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

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
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";


    [Required(ErrorMessage = "Announcement message is required.")]
    [Display(Name = "Message")]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";


    [Required]
    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";


    [Display(Name = "Send To")]
    [Required]
    //
    // Property Name    : Target
    // Purpose          : Gets or sets the target.
    // Data Type        : string
    //
    public string Target { get; set; } = "All Schools";


    [Display(Name = "School")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }


    [Display(Name = "Acknowledgement Required")]
    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }


    [Display(Name = "Expiry Date")]
    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }


    //
    // Property Name    : Priorities
    // Purpose          : Collection of priorities used by this model.
    // Data Type        : List<string>
    //
    public List<string> Priorities { get; set; } =
    [
        "Normal",
        "Important",
        "Urgent"
    ];


    //
    // Property Name    : Targets
    // Purpose          : Collection of targets used by this model.
    // Data Type        : List<string>
    //
    public List<string> Targets { get; set; } =
    [
        "All Schools",
        "Specific School"
    ];


    //
    // Property Name    : Schools
    // Purpose          : Collection of schools used by this model.
    // Data Type        : List<SuperAdminAnnouncementSchoolItemViewModel>
    //
    public List<SuperAdminAnnouncementSchoolItemViewModel>
        Schools
    { get; set; } = [];
}


//
// Class Name       : SuperAdminAnnouncementSchoolItemViewModel
// Purpose          : Describes one super admin announcement school item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminAnnouncementSchoolItemViewModel object.
//
public class SuperAdminAnnouncementSchoolItemViewModel
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