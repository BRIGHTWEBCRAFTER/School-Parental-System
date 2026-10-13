using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("users")]
public class UserProfile : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }


    [Column("email")]
    public string? Email { get; set; }


    [Column("full_name")]
    public string FullName { get; set; } = "";


    [Column("role")]
    public string Role { get; set; } = "";


    [Column("school_id")]
    public Guid? SchoolId { get; set; }


    [Column("phone")]
    public string? Phone { get; set; }


    [Column("profile_picture")]
    public string? ProfilePicture { get; set; }


    [Column("is_active")]
    public bool IsActive { get; set; }
}