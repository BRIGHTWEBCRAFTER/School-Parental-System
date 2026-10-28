using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ForgotPasswordViewModel
// Purpose          : Supplies forgot password view model information to MVC views and actions.
// Re-use           : Passed between Authentication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ForgotPasswordViewModel object.
//
public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";
}