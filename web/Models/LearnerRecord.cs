using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("learners")]
public class LearnerRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("admission_number")]
    public string AdmissionNumber { get; set; } = "";

    [Column("full_name")]
    public string FullName { get; set; } = "";

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("grade")]
    public string? Grade { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}