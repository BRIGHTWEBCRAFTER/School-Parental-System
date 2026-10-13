using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolParentalSystem.Web.ViewModels;

public class AddLearnerViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = "";

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = "";

    [Required]
    public string Grade { get; set; } = "";

    public string? Address { get; set; }

    [Display(Name = "Class")]
    public Guid? ClassId { get; set; }

    public List<ClassOptionViewModel> AvailableClasses { get; set; } = [];

    [Display(Name = "Guardian Name")]
    public string? GuardianName { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Please select a parent or guardian.")]
    [Display(Name = "Parent / Guardian")]
    public Guid? ParentId { get; set; }

    [Required(ErrorMessage = "Please select the relationship.")]
    public string Relationship { get; set; } = "";
}
public class ClassOptionViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Grade { get; set; } = "";
}



public class ParentSearchResultViewModel
{
    [JsonPropertyName("parent_id")]
    public Guid ParentId { get; set; }

    [JsonPropertyName("guardian_number")]
    public string GuardianNumber { get; set; } = "";

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = "";

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}