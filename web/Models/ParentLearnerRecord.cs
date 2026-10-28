using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("parent_learners")]
//
// Class Name       : ParentLearnerRecord
// Purpose          : Represents parent learner data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentLearnerRecord object.
//
public class ParentLearnerRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("parent_id")]
    //
    // Property Name    : ParentId
    // Purpose          : Identifies the related parent record.
    // Data Type        : Guid
    //
    public Guid ParentId { get; set; }

    [Column("learner_id")]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Column("relationship")]
    //
    // Property Name    : Relationship
    // Purpose          : Gets or sets the relationship.
    // Data Type        : string?
    //
    public string? Relationship { get; set; }

    [Column("is_primary_guardian")]
    //
    // Property Name    : IsPrimaryGuardian
    // Purpose          : Indicates whether primary guardian applies.
    // Data Type        : bool
    //
    public bool IsPrimaryGuardian { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }
}