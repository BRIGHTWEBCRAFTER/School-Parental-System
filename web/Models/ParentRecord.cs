using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("parents")]
//
// Class Name       : ParentRecord
// Purpose          : Represents parent data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentRecord object.
//
public class ParentRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


    [Column("guardian_number")]
    //
    // Property Name    : GuardianNumber
    // Purpose          : Gets or sets the guardian number.
    // Data Type        : string
    //
    public string GuardianNumber { get; set; } = "";


    [Column("user_id")]
    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }


    [Column("gender")]
    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string?
    //
    public string? Gender { get; set; }


    [Column("alternative_phone")]
    //
    // Property Name    : AlternativePhone
    // Purpose          : Gets or sets the alternative phone.
    // Data Type        : string?
    //
    public string? AlternativePhone { get; set; }


    [Column("address")]
    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }


    [Column("contact_last_verified")]
    //
    // Property Name    : ContactLastVerified
    // Purpose          : Gets or sets the contact last verified.
    // Data Type        : DateTime?
    //
    public DateTime? ContactLastVerified { get; set; }


    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }


    
}