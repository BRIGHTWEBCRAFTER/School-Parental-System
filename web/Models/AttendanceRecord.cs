using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("attendance")]
//
// Class Name       : AttendanceRecord
// Purpose          : Represents attendance data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceRecord object.
//
public class AttendanceRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("learner_id")]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    [Column("attendance_date")]
    //
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; }

    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    [Column("check_in_time")]
    //
    // Property Name    : CheckInTime
    // Purpose          : Date or time associated with check in time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? CheckInTime { get; set; }

    [Column("remarks")]
    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string?
    //
    public string? Remarks { get; set; }

    [Column("marked_by")]
    //
    // Property Name    : MarkedBy
    // Purpose          : Gets or sets the marked by.
    // Data Type        : Guid
    //
    public Guid MarkedBy { get; set; }

    [Column("subject_id")]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid?
    //
    public Guid? SubjectId { get; set; }

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