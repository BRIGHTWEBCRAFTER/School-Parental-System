using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ResetPasswordViewModel
// Purpose          : Supplies reset password view model information to MVC views and actions.
// Re-use           : Passed between Authentication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ResetPasswordViewModel object.
//
public class ResetPasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [Display(Name = "New Password")]
    //
    // Property Name    : NewPassword
    // Purpose          : Gets or sets the new password.
    // Data Type        : string
    //
    public string NewPassword { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword),
        ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    //
    // Property Name    : ConfirmPassword
    // Purpose          : Gets or sets the confirm password.
    // Data Type        : string
    //
    public string ConfirmPassword { get; set; } = "";

    //
    // Property Name    : AccessToken
    // Purpose          : Gets or sets the access token.
    // Data Type        : string
    //
    public string AccessToken { get; set; } = "";
    //
    // Property Name    : RefreshToken
    // Purpose          : Gets or sets the refresh token.
    // Data Type        : string
    //
    public string RefreshToken { get; set; } = "";
}