using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

public class EditLearnerViewModel
{
    public string Id { get; set; } = "";

    [Required]
    [Display(Name = "Admission Number")]
    public string AdmissionNumber { get; set; } = "";

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

    [Display(Name = "Class")]
    public string ClassName { get; set; } = "";

    public string Address { get; set; } = "";

    [Display(Name = "Guardian Name")]
    public string GuardianName { get; set; } = "";

    public string Relationship { get; set; } = "";

    [Display(Name = "Active Learner")]
    public bool IsActive { get; set; } = true;
}