using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("admin_registration_codes")]
//
// Class Name       : AdminRegistrationCodeRecord
// Purpose          : Represents admin registration code data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AdminRegistrationCodeRecord object.
//
public class AdminRegistrationCodeRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("code_hash")]
    //
    // Property Name    : CodeHash
    // Purpose          : Gets or sets the code hash.
    // Data Type        : string
    //
    public string CodeHash { get; set; } = "";

    [Column("admin_role")]
    //
    // Property Name    : AdminRole
    // Purpose          : Gets or sets the admin role.
    // Data Type        : string
    //
    public string AdminRole { get; set; } = "";

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }

    [Column("expires_at")]
    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    [Column("used_at")]
    //
    // Property Name    : UsedAt
    // Purpose          : Date or time associated with used at.
    // Data Type        : DateTime?
    //
    public DateTime? UsedAt { get; set; }

    [Column("used_by")]
    //
    // Property Name    : UsedBy
    // Purpose          : Gets or sets the used by.
    // Data Type        : Guid?
    //
    public Guid? UsedBy { get; set; }

    [Column("created_by")]
    //
    // Property Name    : CreatedBy
    // Purpose          : Gets or sets the created by.
    // Data Type        : Guid?
    //
    public Guid? CreatedBy { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}