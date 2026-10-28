
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherEditProfileViewModel
// Purpose          : Supplies teacher edit profile view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEditProfileViewModel object.
//
public class TeacherEditProfileViewModel
{
    // =========================================
    // EDITABLE FIELDS
    // =========================================

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage =
            "Full name must contain 2 to 150 characters.")]
    [Display(Name = "Full Name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";


    [StringLength(
        20,
        ErrorMessage =
            "Phone number cannot exceed 20 characters.")]
    [RegularExpression(
        @"^(?:0[1-9][0-9]{8}|\+27[1-9][0-9]{8})$",
        ErrorMessage = "Use a South African number such as 0821234567 or +27821234567. Digits only.")]
    [Display(Name = "Phone Number")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }


    // =========================================
    // READ-ONLY ACCOUNT FIELDS
    // =========================================

    [ValidateNever]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    [ValidateNever]
    //
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "Teacher";

    [ValidateNever]
    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    [ValidateNever]
    //
    // Property Name    : SchoolAddress
    // Purpose          : Gets or sets the school address.
    // Data Type        : string?
    //
    public string? SchoolAddress { get; set; }

    [ValidateNever]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }


    // =========================================
    // DISPLAY HELPERS
    // =========================================

    //
    // Property Name    : AccountStatus
    // Purpose          : Gets or sets the account status.
    // Data Type        : string
    //
    public string AccountStatus =>
        IsActive ? "Active" : "Inactive";
}
