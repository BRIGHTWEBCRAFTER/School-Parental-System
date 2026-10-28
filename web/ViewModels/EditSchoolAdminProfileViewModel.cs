
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EditSchoolAdminProfileViewModel
// Purpose          : Holds editable fields and validation for school admin profile view model.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EditSchoolAdminProfileViewModel object.
//
public class EditSchoolAdminProfileViewModel
{
    // =========================================
    // EDITABLE PROFILE INFORMATION
    // =========================================

    [Required(
        ErrorMessage = "Full name is required.")]
    [StringLength(
        120,
        MinimumLength = 2,
        ErrorMessage = "Full name must be between 2 and 120 characters.")]
    [Display(Name = "Full Name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";


    [StringLength(
        30,
        ErrorMessage = "Phone number cannot exceed 30 characters.")]
    [Phone(
        ErrorMessage = "Please enter a valid phone number.")]
    [Display(Name = "Phone Number")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }
}
