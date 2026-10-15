using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("teachers")]
public class TeacherRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("employee_id")]
    public string EmployeeId { get; set; } = "";

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("specialization")]
    public string? Specialization { get; set; }

    [Column("joining_date")]
    public DateTime? JoiningDate { get; set; }

    [Column("alternative_phone")]
    public string? AlternativePhone { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}