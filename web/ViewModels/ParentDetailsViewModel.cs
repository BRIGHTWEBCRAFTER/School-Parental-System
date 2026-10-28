namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ParentDetailsViewModel
// Purpose          : Supplies parent details view model information to MVC views and actions.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentDetailsViewModel object.
//
public class ParentDetailsViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : GuardianNumber
    // Purpose          : Gets or sets the guardian number.
    // Data Type        : string
    //
    public string GuardianNumber { get; set; } = "";

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string
    //
    public string Phone { get; set; } = "";

    //
    // Property Name    : AlternativePhone
    // Purpose          : Gets or sets the alternative phone.
    // Data Type        : string
    //
    public string AlternativePhone { get; set; } = "";

    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";

    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string
    //
    public string Address { get; set; } = "";

    // NEW:
    // Used to track when the parent's contact details
    // were last confirmed by the school/parent.
    //
    // Property Name    : ContactLastVerified
    // Purpose          : Gets or sets the contact last verified.
    // Data Type        : DateTime?
    //
    public DateTime? ContactLastVerified { get; set; }

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";

    //
    // Property Name    : LinkedLearners
    // Purpose          : Collection of linked learners used by this model.
    // Data Type        : List<ParentLinkedLearnerViewModel>
    //
    public List<ParentLinkedLearnerViewModel>
        LinkedLearners
    { get; set; } = new();
}


//
// Class Name       : ParentLinkedLearnerViewModel
// Purpose          : Supplies parent linked learner view model information to MVC views and actions.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ParentLinkedLearnerViewModel object.
//
public class ParentLinkedLearnerViewModel
{
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";

    //
    // Property Name    : Relationship
    // Purpose          : Gets or sets the relationship.
    // Data Type        : string
    //
    public string Relationship { get; set; } = "";

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}