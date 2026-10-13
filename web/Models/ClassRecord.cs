using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("classes")]
public class ClassRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("code")]
    public string Code { get; set; } = "";

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("grade")]
    public string? Grade { get; set; }

    [Column("section")]
    public string? Section { get; set; }

    [Column("academic_year")]
    public int AcademicYear { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}