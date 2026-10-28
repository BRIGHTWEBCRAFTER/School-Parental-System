using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("announcement_recipients")]
//
// Class Name       : AnnouncementRecipientRecord
// Purpose          : Represents announcement recipient data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementRecipientRecord object.
//
public class AnnouncementRecipientRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("announcement_id")]
    //
    // Property Name    : AnnouncementId
    // Purpose          : Identifies the related announcement record.
    // Data Type        : Guid
    //
    public Guid AnnouncementId { get; set; }

    [Column("user_id")]
    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }

    [Column("is_read")]
    //
    // Property Name    : IsRead
    // Purpose          : Indicates whether read applies.
    // Data Type        : bool
    //
    public bool IsRead { get; set; }

    [Column("read_at")]
    //
    // Property Name    : ReadAt
    // Purpose          : Date or time associated with read at.
    // Data Type        : DateTime?
    //
    public DateTime? ReadAt { get; set; }

    [Column("is_acknowledged")]
    //
    // Property Name    : IsAcknowledged
    // Purpose          : Indicates whether acknowledged applies.
    // Data Type        : bool
    //
    public bool IsAcknowledged { get; set; }

    [Column("acknowledged_at")]
    //
    // Property Name    : AcknowledgedAt
    // Purpose          : Date or time associated with acknowledged at.
    // Data Type        : DateTime?
    //
    public DateTime? AcknowledgedAt { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}