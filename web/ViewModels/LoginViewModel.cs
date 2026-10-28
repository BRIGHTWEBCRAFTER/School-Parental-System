using System.ComponentModel.DataAnnotations;


namespace SchoolParentalSystem.Web.ViewModels;


//
// Class Name       : LoginViewModel
// Purpose          : Supplies login view model information to MVC views and actions.
// Re-use           : Passed between Authentication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : LoginViewModel object.
//
public class LoginViewModel
{
    [Required(
        ErrorMessage = "Email address is required.")]
    [EmailAddress(
        ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";


    [Required(
        ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    //
    // Property Name    : Password
    // Purpose          : Gets or sets the password.
    // Data Type        : string
    //
    public string Password { get; set; } = "";


    [Required(
        ErrorMessage = "Please select your role.")]
    [Display(Name = "Role")]
    //
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "";


    [Display(Name = "Access Code")]
    //
    // Property Name    : AccessCode
    // Purpose          : Gets or sets the access code.
    // Data Type        : string
    //
    public string AccessCode { get; set; } = "";
}