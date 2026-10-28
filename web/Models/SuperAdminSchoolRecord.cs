using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("schools")]
//
// Class Name       : SuperAdminSchoolRecord
// Purpose          : Represents super admin school data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminSchoolRecord object.
//
public class SuperAdminSchoolRecord : BaseModel
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
    // Data Type        : string
    //
    public string Name { get; set; } = "";


    [Column("address")]
    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string
    //
    public string Address { get; set; } = "";


    [Column("phone")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }


    [Column("email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }


    [Column("logo_url")]
    //
    // Property Name    : LogoUrl
    // Purpose          : Gets or sets the logo URL.
    // Data Type        : string?
    //
    public string? LogoUrl { get; set; }


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


    [Column("admission_prefix")]
    //
    // Property Name    : AdmissionPrefix
    // Purpose          : Gets or sets the admission prefix.
    // Data Type        : string?
    //
    public string? AdmissionPrefix { get; set; }


    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string?
    //
    public string? Status { get; set; }


    [Column("school_code")]
    //
    // Property Name    : SchoolCode
    // Purpose          : Gets or sets the school code.
    // Data Type        : string?
    //
    public string? SchoolCode { get; set; }


    [Column("school_type")]
    //
    // Property Name    : SchoolType
    // Purpose          : Gets or sets the school type.
    // Data Type        : string?
    //
    public string? SchoolType { get; set; }


    [Column("province")]
    //
    // Property Name    : Province
    // Purpose          : Gets or sets the province.
    // Data Type        : string?
    //
    public string? Province { get; set; }


    [Column("town_city")]
    //
    // Property Name    : TownCity
    // Purpose          : Gets or sets the town city.
    // Data Type        : string?
    //
    public string? TownCity { get; set; }


    [Column("principal_name")]
    //
    // Property Name    : PrincipalName
    // Purpose          : Gets or sets the principal name.
    // Data Type        : string?
    //
    public string? PrincipalName { get; set; }


    [Column("max_learners")]
    //
    // Property Name    : MaxLearners
    // Purpose          : Gets or sets the max learners.
    // Data Type        : int?
    //
    public int? MaxLearners { get; set; }


    [Column("notes")]
    //
    // Property Name    : Notes
    // Purpose          : Gets or sets the notes.
    // Data Type        : string?
    //
    public string? Notes { get; set; }
}