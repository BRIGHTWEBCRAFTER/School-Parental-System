using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models.SuperAdmin;

[Table("schools")]
//
// Class Name       : SchoolRegistrationSchoolRecord
// Purpose          : Represents school registration school data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolRegistrationSchoolRecord object.
//
public class SchoolRegistrationSchoolRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string?
    //
    public string? Name { get; set; }

    [Column("email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }

    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string?
    //
    public string? Status { get; set; }
}