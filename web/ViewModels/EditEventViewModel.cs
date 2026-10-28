using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EditEventViewModel
// Purpose          : Holds editable fields and validation for event view model.
// Re-use           : Passed between Events controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EditEventViewModel object.
//
public class EditEventViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(150)]
    [Display(Name = "Event name")]
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";


    [StringLength(1000)]
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string?
    //
    public string? Description { get; set; }


    [Required(ErrorMessage = "Event date is required.")]
    [Display(Name = "Event date")]
    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; }


    [Display(Name = "Start time")]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }


    [Display(Name = "End time")]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? EndTime { get; set; }


    [StringLength(200)]
    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string?
    //
    public string? Venue { get; set; }


    [Required(ErrorMessage = "Target audience is required.")]
    [Display(Name = "Target audience")]
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


    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<AnnouncementClassOptionViewModel>
    //
    public List<AnnouncementClassOptionViewModel>
        Classes
    { get; set; } = [];


    //
    // Property Name    : Grades
    // Purpose          : Collection of grades used by this model.
    // Data Type        : List<string>
    //
    public List<string>
        Grades
    { get; set; } = [];
}