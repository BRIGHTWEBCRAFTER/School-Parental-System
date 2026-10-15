using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

public class AddTeacherViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = "";


    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = "";


    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }


    [Display(Name = "Specialisation")]
    public string? Specialization { get; set; }


    [Display(Name = "Joining Date")]
    [DataType(DataType.Date)]
    public DateTime? JoiningDate { get; set; }


    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }


    public string? Gender { get; set; }


    [Display(Name = "Alternative Phone")]
    public string? AlternativePhone { get; set; }


    public string? Address { get; set; }
}