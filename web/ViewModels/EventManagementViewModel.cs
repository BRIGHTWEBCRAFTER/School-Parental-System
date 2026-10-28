namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EventManagementViewModel
// Purpose          : Supplies event management view model information to MVC views and actions.
// Re-use           : Passed between Events controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EventManagementViewModel object.
//
public class EventManagementViewModel
{
    //
    // Property Name    : Events
    // Purpose          : Collection of events used by this model.
    // Data Type        : List<CommunicationEventItemViewModel>
    //
    public List<CommunicationEventItemViewModel>
        Events
    { get; set; } = [];


    //
    // Property Name    : TotalEvents
    // Purpose          : Total events displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalEvents =>
        Events.Count;


    //
    // Property Name    : UpcomingEventsCount
    // Purpose          : Computed or stored upcoming events count value.
    // Data Type        : int
    //
    public int UpcomingEventsCount =>
        Events.Count(x =>
            x.EventDate.Date >=
            DateTime.Today);


    //
    // Property Name    : PastEventsCount
    // Purpose          : Computed or stored past events count value.
    // Data Type        : int
    //
    public int PastEventsCount =>
        Events.Count(x =>
            x.EventDate.Date <
            DateTime.Today);
}