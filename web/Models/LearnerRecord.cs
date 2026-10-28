using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("learners")]
//
// Class Name       : LearnerRecord
// Purpose          : Represents learner data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : LearnerRecord object.
//
public class LearnerRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("admission_number")]
    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";

    [Column("full_name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

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

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("grade")]
    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string?
    //
    public string? Grade { get; set; }

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