using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EditParentViewModel
// Purpose          : Holds editable fields and validation for parent view model.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EditParentViewModel object.
//
public class EditParentViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


    [Display(Name = "Guardian Number")]
    //
    // Property Name    : GuardianNumber
    // Purpose          : Gets or sets the guardian number.
    // Data Type        : string
    //
    public string GuardianNumber { get; set; } = "";


    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";


    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email Address")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";



    //
    // Property Name    : Phone
    // Purpose          : Validates an updated South African
    //                    phone number.
    // Re-use           : Edit Teacher and Edit Parent pages.
    // Input Parameters : Phone number.
    // Output Type      : string
    //

    [Display(Name = "Phone Number")]
    [RegularExpression(
        @"^(0[1-9][0-9]{8}|\+27[1-9][0-9]{8})$",
        ErrorMessage =
            "Use 0821234567 or +27821234567. " +
            "Numbers only.")]
    public string? Phone { get; set; }



    [Display(Name = "Gender")]
    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string?
    //
    public string? Gender { get; set; }



    [Display(Name = "Alternative Phone")]
    [RegularExpression(
        @"^(0[1-9][0-9]{8}|\+27[1-9][0-9]{8})$",
        ErrorMessage =
            "Enter a valid South African phone number, " +
            "e.g. 0821234567 or +27821234567. " +
            "Letters are not allowed.")]

    //
    // Property Name    : AlternativePhone
    // Purpose          : Stores an optional alternative
    //                    South African phone number.
    // Re-use           : Teacher and Parent forms.
    // Data Type        : string?
    // Validation       : Must start with 0 or +27
    //                    if a number is provided.
    //

    public string? AlternativePhone { get; set; }



    [Display(Name = "Address")]
    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }
}