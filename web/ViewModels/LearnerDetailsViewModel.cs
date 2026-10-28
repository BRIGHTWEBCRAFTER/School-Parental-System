using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.ViewModels
{
    //
    // Class Name       : LearnerDetailsViewModel
    // Purpose          : Supplies learner details view model information to MVC views and actions.
    // Re-use           : Passed between Learner controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : LearnerDetailsViewModel object.
    //
    public class LearnerDetailsViewModel
    {
        //
        // Property Name    : Id
        // Purpose          : Unique identifier of this record.
        // Data Type        : Guid
        //
        public Guid Id { get; set; }

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
        // Property Name    : DateOfBirth
        // Purpose          : Gets or sets the date of birth.
        // Data Type        : DateTime?
        //
        public DateTime? DateOfBirth { get; set; }

        //
        // Property Name    : Gender
        // Purpose          : Gets or sets the gender.
        // Data Type        : string
        //
        public string Gender { get; set; } = "";

        //
        // Property Name    : Grade
        // Purpose          : Stores the grade.
        // Data Type        : string
        //
        public string Grade { get; set; } = "";

        //
        // Property Name    : ClassName
        // Purpose          : Stores the class name.
        // Data Type        : string
        //
        public string ClassName { get; set; } = "Not assigned";

        //
        // Property Name    : Address
        // Purpose          : Stores the address.
        // Data Type        : string
        //
        public string Address { get; set; } = "";

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
            IsActive ? "Active" : "Inactive";


        // Guardian information
        //
        // Property Name    : GuardianName
        // Purpose          : Gets or sets the guardian name.
        // Data Type        : string
        //
        public string GuardianName { get; set; } =
            "Not linked";

        //
        // Property Name    : GuardianNumber
        // Purpose          : Gets or sets the guardian number.
        // Data Type        : string
        //
        public string GuardianNumber { get; set; } =
            "";

        //
        // Property Name    : GuardianPhone
        // Purpose          : Gets or sets the guardian phone.
        // Data Type        : string
        //
        public string GuardianPhone { get; set; } =
            "";

        //
        // Property Name    : GuardianEmail
        // Purpose          : Gets or sets the guardian email.
        // Data Type        : string
        //
        public string GuardianEmail { get; set; } =
            "";

        //
        // Property Name    : Relationship
        // Purpose          : Gets or sets the relationship.
        // Data Type        : string
        //
        public string Relationship { get; set; } =
            "";
    }
}