using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("enrollments")]
public class EnrollmentRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("learner_id")]
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("enrollment_date")]
    public DateTime EnrollmentDate { get; set; }

    [Column("academic_year")]
    public int AcademicYear { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}