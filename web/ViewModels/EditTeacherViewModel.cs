using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EditTeacherViewModel
// Purpose          : Holds editable fields and validation for teacher view model.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EditTeacherViewModel object.
//
public class EditTeacherViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : EmployeeId
    // Purpose          : Identifies the related employee record.
    // Data Type        : string
    //
    public string EmployeeId { get; set; } = "";

    [Required]
    [Display(Name = "Full Name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

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


    [Display(Name = "Specialisation")]
    //
    // Property Name    : Specialization
    // Purpose          : Gets or sets the specialization.
    // Data Type        : string?
    //
    public string? Specialization { get; set; }

    [Display(Name = "Joining Date")]
    //
    // Property Name    : JoiningDate
    // Purpose          : Date or time associated with joining date.
    // Data Type        : DateTime?
    //
    public DateTime? JoiningDate { get; set; }

    [Display(Name = "Date of Birth")]
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


    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }
}