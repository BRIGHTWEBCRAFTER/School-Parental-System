using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : SchoolAdminRegistrationViewModel
// Purpose          : Supplies school admin registration view model information to MVC views and actions.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminRegistrationViewModel object.
//
public class SchoolAdminRegistrationViewModel
{
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

    [Display(Name = "Phone Number")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string
    //
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [Display(Name = "Password")]
    //
    // Property Name    : Password
    // Purpose          : Gets or sets the password.
    // Data Type        : string
    //
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    //
    // Property Name    : ConfirmPassword
    // Purpose          : Gets or sets the confirm password.
    // Data Type        : string
    //
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "Please select your school.")]
    [Display(Name = "School")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }

    [Required(ErrorMessage = "Registration code is required.")]
    [Display(Name = "Registration Code")]
    //
    // Property Name    : RegistrationCode
    // Purpose          : Gets or sets the registration code.
    // Data Type        : string
    //
    public string RegistrationCode { get; set; } = "";

    //
    // Property Name    : Schools
    // Purpose          : Collection of schools used by this model.
    // Data Type        : List<SchoolRegistrationItemViewModel>
    //
    public List<SchoolRegistrationItemViewModel> Schools { get; set; }
        = new();
}

//
// Class Name       : SchoolRegistrationItemViewModel
// Purpose          : Describes one school registration item view model row shown in a page.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolRegistrationItemViewModel object.
//
public class SchoolRegistrationItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";
}