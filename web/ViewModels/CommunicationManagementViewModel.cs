namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : CommunicationManagementViewModel
// Purpose          : Supplies communication management view model information to MVC views and actions.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : CommunicationManagementViewModel object.
//
public class CommunicationManagementViewModel
{
    //
    // Property Name    : SearchTerm
    // Purpose          : Gets or sets the search term.
    // Data Type        : string
    //
    public string SearchTerm { get; set; } = "";

    //
    // Property Name    : SelectedPriority
    // Purpose          : Gets or sets the selected priority.
    // Data Type        : string
    //
    public string SelectedPriority { get; set; } = "";

    //
    // Property Name    : SelectedTarget
    // Purpose          : Gets or sets the selected target.
    // Data Type        : string
    //
    public string SelectedTarget { get; set; } = "";

    //
    // Property Name    : SelectedStatus
    // Purpose          : Gets or sets the selected status.
    // Data Type        : string
    //
    public string SelectedStatus { get; set; } = "";

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
    // Property Name    : TargetTypes
    // Purpose          : Collection of target types used by this model.
    // Data Type        : List<string>
    //
    public List<string> TargetTypes { get; set; } =
    [
        "Whole School",
        "Role",
        "Grade",
        "Class",
        "Specific Users"
    ];

    //
    // Property Name    : Announcements
    // Purpose          : Collection of announcements used by this model.
    // Data Type        : List<CommunicationAnnouncementItemViewModel>
    //
    public List<CommunicationAnnouncementItemViewModel>
        Announcements
    { get; set; } = [];

    //
    // Property Name    : UpcomingEvents
    // Purpose          : Collection of upcoming events used by this model.
    // Data Type        : List<CommunicationEventItemViewModel>
    //
    public List<CommunicationEventItemViewModel>
        UpcomingEvents
    { get; set; } = [];


    //
    // Property Name    : TotalAnnouncements
    // Purpose          : Total announcements displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalAnnouncements =>
        Announcements.Count;


    //
    // Property Name    : ImportantNoticeCount
    // Purpose          : Computed or stored important notice count value.
    // Data Type        : int
    //
    public int ImportantNoticeCount =>
        Announcements.Count(x =>
            x.Priority == "Important" ||
            x.Priority == "Urgent");


    //
    // Property Name    : TotalRecipients
    // Purpose          : Total recipients displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRecipients =>
        Announcements.Sum(x =>
            x.RecipientCount);


    //
    // Property Name    : TotalRead
    // Purpose          : Total read displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRead =>
        Announcements.Sum(x =>
            x.ReadCount);


    //
    // Property Name    : TotalAcknowledged
    // Purpose          : Total acknowledged displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalAcknowledged =>
        Announcements.Sum(x =>
            x.AcknowledgedCount);


    //
    // Property Name    : ReadRate
    // Purpose          : Computed or stored read rate value.
    // Data Type        : decimal
    //
    public decimal ReadRate =>
        TotalRecipients == 0
            ? 0
            : Math.Round(
                (decimal)TotalRead /
                TotalRecipients * 100,
                1);
}


//
// Class Name       : CommunicationAnnouncementItemViewModel
// Purpose          : Describes one communication announcement item view model row shown in a page.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : CommunicationAnnouncementItemViewModel object.
//
public class CommunicationAnnouncementItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";

    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "";

    //
    // Property Name    : TargetDisplay
    // Purpose          : Gets or sets the target display.
    // Data Type        : string
    //
    public string TargetDisplay { get; set; } = "";

    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }

    //
    // Property Name    : RecipientCount
    // Purpose          : Computed or stored recipient count value.
    // Data Type        : int
    //
    public int RecipientCount { get; set; }

    //
    // Property Name    : ReadCount
    // Purpose          : Computed or stored read count value.
    // Data Type        : int
    //
    public int ReadCount { get; set; }

    //
    // Property Name    : AcknowledgedCount
    // Purpose          : Computed or stored acknowledged count value.
    // Data Type        : int
    //
    public int AcknowledgedCount { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }
}


//
// Class Name       : CommunicationEventItemViewModel
// Purpose          : Describes one communication event item view model row shown in a page.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : CommunicationEventItemViewModel object.
//
public class CommunicationEventItemViewModel
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
    // Property Name    : TargetDisplay
    // Purpose          : Gets or sets the target display.
    // Data Type        : string
    //
    public string TargetDisplay { get; set; } = "";
}