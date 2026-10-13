using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("parents")]
public class ParentRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("guardian_number")]
    public string GuardianNumber { get; set; } = "";

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}