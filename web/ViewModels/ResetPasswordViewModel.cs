using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword),
        ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = "";

    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
}