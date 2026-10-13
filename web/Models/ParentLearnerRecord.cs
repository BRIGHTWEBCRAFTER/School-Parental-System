using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("parent_learners")]
public class ParentLearnerRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("parent_id")]
    public Guid ParentId { get; set; }

    [Column("learner_id")]
    public Guid LearnerId { get; set; }

    [Column("relationship")]
    public string? Relationship { get; set; }

    [Column("is_primary_guardian")]
    public bool IsPrimaryGuardian { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}