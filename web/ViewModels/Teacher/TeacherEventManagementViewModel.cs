
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

/* ==========================================
   TEACHER EVENTS MANAGEMENT
   ========================================== */

//
// Class Name       : TeacherEventManagementViewModel
// Purpose          : Supplies teacher event management view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEventManagementViewModel object.
//
public class TeacherEventManagementViewModel
{
    //
    // Property Name    : HasTeacherProfile
    // Purpose          : Indicates whether teacher profile is available.
    // Data Type        : bool
    //
    public bool HasTeacherProfile { get; set; }

    //
    // Property Name    : DataLoadFailed
    // Purpose          : Gets or sets the data load failed.
    // Data Type        : bool
    //
    public bool DataLoadFailed { get; set; }

    // Events available to this Teacher.
    // The service must restrict these records
    // to authorized school and class data.

    //
    // Property Name    : Events
    // Purpose          : Collection of events used by this model.
    // Data Type        : List<TeacherEventItemViewModel>
    //
    public List<TeacherEventItemViewModel> Events
    { get; set; } = new();

    // ==========================================
    // SUMMARY STATISTICS
    // ==========================================

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
        Events.Count(e =>
            e.EventDate.Date >= DateTime.Today);

    //
    // Property Name    : PastEventsCount
    // Purpose          : Computed or stored past events count value.
    // Data Type        : int
    //
    public int PastEventsCount =>
        Events.Count(e =>
            e.EventDate.Date < DateTime.Today);
}

/* ==========================================
   TEACHER EVENT ITEM
   ========================================== */

//
// Class Name       : TeacherEventItemViewModel
// Purpose          : Describes one teacher event item view model row shown in a page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEventItemViewModel object.
//
public class TeacherEventItemViewModel
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

    // ==========================================
    // TARGET AUDIENCE
    // ==========================================

    // Only Class or Specific Users.

    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "Class";

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : TargetDisplay
    // Purpose          : Gets or sets the target display.
    // Data Type        : string
    //
    public string TargetDisplay =>
        TargetType switch
        {
            "Class" =>
                string.IsNullOrWhiteSpace(ClassName)
                    ? "Assigned Class"
                    : ClassName,

            "Specific Users" =>
                string.IsNullOrWhiteSpace(ClassName)
                    ? "Specific Parent"
                    : $"Specific Parent — {ClassName}",

            _ => "Unknown"
        };

    // ==========================================
    // EVENT STATUS
    // ==========================================

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status
    {
        get
        {
            var eventDate = EventDate.Date;
            var today = DateTime.Today;

            if (eventDate < today)
                return "Past";

            if (eventDate == today)
                return "Today";

            return "Upcoming";
        }
    }
}
