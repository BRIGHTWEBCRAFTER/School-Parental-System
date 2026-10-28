using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : AddTeacherViewModel
// Purpose          : Holds input and validation data for creating teacher view model.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AddTeacherViewModel object.
//
public class AddTeacherViewModel
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



    //
    // Property Name    : Phone
    // Purpose          : Accepts South African phone numbers.
    // Re-use           : Teacher and Parent registration.
    // Input Parameters : Phone number starting with 0 or +27.
    // Output Type      : string
    //

    [Required(ErrorMessage = "Phone number is required.")]
    [Display(Name = "Phone Number")]
    [RegularExpression(
        @"^(0[1-9][0-9]{8}|\+27[1-9][0-9]{8})$",
        ErrorMessage =
            "Enter a valid South African phone number, " +
            "e.g. 0821234567 or +27821234567. " +
            "Letters are not allowed.")]
    public string? Phone { get; set; }



    [Display(Name = "Specialisation")]
    //
    // Property Name    : Specialization
    // Purpose          : Gets or sets the specialization.
    // Data Type        : string?
    //
    public string? Specialization { get; set; }


    [Display(Name = "Joining Date")]
    [DataType(DataType.Date)]
    //
    // Property Name    : JoiningDate
    // Purpose          : Date or time associated with joining date.
    // Data Type        : DateTime?
    //
    public DateTime? JoiningDate { get; set; }


    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    //
    // Property Name    : DateOfBirth
    // Purpose          : Gets or sets the date of birth.
    // Data Type        : DateTime?
    //
    public DateTime? DateOfBirth { get; set; }


    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string?
    //
    public string? Gender { get; set; }


    [Display(Name = "Alternative Phone")]
    //
    // Property Name    : AlternativePhone
    // Purpose          : Gets or sets the alternative phone.
    // Data Type        : string?
    //
    public string? AlternativePhone { get; set; }


    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }
}