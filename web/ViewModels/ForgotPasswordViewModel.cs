using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = "";
}