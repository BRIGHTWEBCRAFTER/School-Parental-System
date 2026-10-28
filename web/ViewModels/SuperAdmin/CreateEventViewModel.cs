using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : CreateEventViewModel
// Purpose          : Holds input and validation data for creating event view model.
// Re-use           : Passed between Events controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : CreateEventViewModel object.
//
public class CreateEventViewModel
{
    [Required(ErrorMessage = "Event name is required.")]
    [Display(Name = "Event Name")]
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";


    [Display(Name = "Description")]
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string?
    //
    public string? Description { get; set; }


    [Required(ErrorMessage = "Event date is required.")]
    [Display(Name = "Event Date")]
    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; } =
        DateTime.Today;


    [Display(Name = "Start Time")]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }


    [Display(Name = "End Time")]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? EndTime { get; set; }


    [Display(Name = "Venue")]
    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string?
    //
    public string? Venue { get; set; }


    [Required]
    [Display(Name = "Send To")]
    //
    // Property Name    : Target
    // Purpose          : Gets or sets the target.
    // Data Type        : string
    //
    public string Target { get; set; } =
        "Specific School";


    [Display(Name = "School")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }


    //
    // Property Name    : Targets
    // Purpose          : Collection of targets used by this model.
    // Data Type        : List<string>
    //
    public List<string> Targets { get; set; } =
    [
        "Specific School",
        "All Schools"
    ];


    //
    // Property Name    : Schools
    // Purpose          : Collection of schools used by this model.
    // Data Type        : List<SuperAdminEventSchoolItemViewModel>
    //
    public List<SuperAdminEventSchoolItemViewModel>
        Schools
    { get; set; } = [];
}


//
// Class Name       : SuperAdminEventSchoolItemViewModel
// Purpose          : Describes one super admin event school item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminEventSchoolItemViewModel object.
//
public class SuperAdminEventSchoolItemViewModel
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