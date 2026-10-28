namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : GeneratedRegistrationCodeResult
// Purpose          : Carries the result of generated registration code result to a view.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GeneratedRegistrationCodeResult object.
//
public class GeneratedRegistrationCodeResult
{
    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : SchoolEmail
    // Purpose          : Gets or sets the school email.
    // Data Type        : string
    //
    public string SchoolEmail { get; set; } = "";

    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime
    //
    public DateTime ExpiresAt { get; set; }
}