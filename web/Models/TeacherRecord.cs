using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("teachers")]
//
// Class Name       : TeacherRecord
// Purpose          : Represents teacher data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherRecord object.
//
public class TeacherRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("employee_id")]
    //
    // Property Name    : EmployeeId
    // Purpose          : Identifies the related employee record.
    // Data Type        : string
    //
    public string EmployeeId { get; set; } = "";

    [Column("user_id")]
    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("date_of_birth")]
    //
    // Property Name    : DateOfBirth
    // Purpose          : Gets or sets the date of birth.
    // Data Type        : DateTime?
    //
    public DateTime? DateOfBirth { get; set; }

    [Column("gender")]
    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string?
    //
    public string? Gender { get; set; }

    [Column("specialization")]
    //
    // Property Name    : Specialization
    // Purpose          : Gets or sets the specialization.
    // Data Type        : string?
    //
    public string? Specialization { get; set; }

    [Column("joining_date")]
    //
    // Property Name    : JoiningDate
    // Purpose          : Date or time associated with joining date.
    // Data Type        : DateTime?
    //
    public DateTime? JoiningDate { get; set; }

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

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }
}