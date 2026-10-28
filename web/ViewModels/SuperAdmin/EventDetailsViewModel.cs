namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : EventDetailsViewModel
// Purpose          : Supplies event details view model information to MVC views and actions.
// Re-use           : Passed between Events controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EventDetailsViewModel object.
//
public class EventDetailsViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";

    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; }

    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }

    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? EndTime { get; set; }

    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string
    //
    public string Venue { get; set; } = "";

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}