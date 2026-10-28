
using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("class_schedules")]
//
// Class Name       : SchoolAdminScheduleRecord
// Purpose          : Represents school admin schedule data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminScheduleRecord object.
//
public class SchoolAdminScheduleRecord : BaseModel
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
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    [Column("subject_id")]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    [Column("teacher_id")]
    //
    // Property Name    : TeacherId
    // Purpose          : Identifies the related teacher record.
    // Data Type        : Guid?
    //
    public Guid? TeacherId { get; set; }

    // 1 = Monday, 5 = Friday

    [Column("day_of_week")]
    //
    // Property Name    : DayOfWeek
    // Purpose          : Gets or sets the day of week.
    // Data Type        : int
    //
    public int DayOfWeek { get; set; }

    [Column("start_time")]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan
    //
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan
    //
    public TimeSpan EndTime { get; set; }

    [Column("room")]
    //
    // Property Name    : Room
    // Purpose          : Gets or sets the room.
    // Data Type        : string?
    //
    public string? Room { get; set; }

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
