
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : InvitationActivationViewModel
// Purpose          : Collect temporary email PIN
//                    and new user password.
// Re-use           : Teacher and Parent activation.
// Input Parameters : Email, PIN and passwords.
// Output Type      : ViewModel.
//

public class InvitationActivationViewModel
{
    [Required]
    [EmailAddress]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    [Required]
    [RegularExpression(
        @"^[0-9]{6,8}$",
        ErrorMessage = "Enter the PIN from your email.")]
    //
    // Property Name    : Pin
    // Purpose          : Gets or sets the PIN.
    // Data Type        : string
    //
    public string Pin { get; set; } = "";

    [Required]
    [MinLength(12)]
    [DataType(DataType.Password)]
    //
    // Property Name    : NewPassword
    // Purpose          : Gets or sets the new password.
    // Data Type        : string
    //
    public string NewPassword { get; set; } = "";

    [Required]
    [Compare(nameof(NewPassword))]
    [DataType(DataType.Password)]
    //
    // Property Name    : ConfirmPassword
    // Purpose          : Gets or sets the confirm password.
    // Data Type        : string
    //
    public string ConfirmPassword { get; set; } = "";
} // end class
