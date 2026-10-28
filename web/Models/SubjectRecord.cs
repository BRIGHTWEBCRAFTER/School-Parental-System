using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("subjects")]
//
// Class Name       : SubjectRecord
// Purpose          : Represents subject data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SubjectRecord object.
//
public class SubjectRecord : BaseModel
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

    [Column("name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";

    [Column("code")]
    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    [Column("description")]
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string?
    //
    public string? Description { get; set; }

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