using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("events")]
//
// Class Name       : EventRecord
// Purpose          : Represents event data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EventRecord object.
//
public class EventRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("created_by")]
    //
    // Property Name    : CreatedBy
    // Purpose          : Gets or sets the created by.
    // Data Type        : Guid
    //
    public Guid CreatedBy { get; set; }

    [Column("event_name")]
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";

    [Column("description")]
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string?
    //
    public string? Description { get; set; }

    [Column("event_date")]
    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; }

    [Column("start_time")]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }

    [Column("end_time")]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? EndTime { get; set; }

    [Column("venue")]
    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string?
    //
    public string? Venue { get; set; }

    [Column("target_type")]
    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "";

    [Column("target_role")]
    //
    // Property Name    : TargetRole
    // Purpose          : Gets or sets the target role.
    // Data Type        : string?
    //
    public string? TargetRole { get; set; }

    [Column("target_grade")]
    //
    // Property Name    : TargetGrade
    // Purpose          : Gets or sets the target grade.
    // Data Type        : string?
    //
    public string? TargetGrade { get; set; }

    [Column("target_class_id")]
    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    //
    // Property Name    : UpdatedAt
    // Purpose          : Date or time associated with updated at.
    // Data Type        : DateTime
    //
    public DateTime UpdatedAt { get; set; }
}