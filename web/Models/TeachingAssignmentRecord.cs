using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("teaching_assignments")]
//
// Class Name       : TeachingAssignmentRecord
// Purpose          : Represents teaching assignment data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeachingAssignmentRecord object.
//
public class TeachingAssignmentRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("teacher_id")]
    //
    // Property Name    : TeacherId
    // Purpose          : Identifies the related teacher record.
    // Data Type        : Guid
    //
    public Guid TeacherId { get; set; }

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

    [Column("assigned_at")]
    //
    // Property Name    : AssignedAt
    // Purpose          : Date or time associated with assigned at.
    // Data Type        : DateTime
    //
    public DateTime AssignedAt { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

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