using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("announcements")]
//
// Class Name       : AnnouncementRecord
// Purpose          : Represents announcement data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementRecord object.
//
public class AnnouncementRecord : BaseModel
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

    [Column("title")]
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    [Column("content")]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";

    [Column("priority")]
    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";

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

    [Column("acknowledgement_required")]
    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }

    [Column("expires_at")]
    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }

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