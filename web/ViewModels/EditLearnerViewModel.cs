using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels
{
    //
    // Class Name       : EditLearnerViewModel
    // Purpose          : Holds editable fields and validation for learner view model.
    // Re-use           : Passed between Learner controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : EditLearnerViewModel object.
    //
    public class EditLearnerViewModel
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


        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        //
        // Property Name    : FullName
        // Purpose          : Stores the full name.
        // Data Type        : string
        //
        public string FullName { get; set; } = "";


        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        //
        // Property Name    : DateOfBirth
        // Purpose          : Gets or sets the date of birth.
        // Data Type        : DateTime?
        //
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        //
        // Property Name    : Gender
        // Purpose          : Gets or sets the gender.
        // Data Type        : string
        //
        public string Gender { get; set; } = "";


        [Required(ErrorMessage = "Grade is required.")]
        //
        // Property Name    : Grade
        // Purpose          : Stores the grade.
        // Data Type        : string
        //
        public string Grade { get; set; } = "";


        [Required(ErrorMessage = "Class is required.")]
        [Display(Name = "Class")]
        //
        // Property Name    : ClassId
        // Purpose          : Identifies the related class record.
        // Data Type        : Guid?
        //
        public Guid? ClassId { get; set; }

        //
        // Property Name    : AvailableClasses
        // Purpose          : Collection of available classes used by this model.
        // Data Type        : List<ClassOptionViewModel>
        //
        public List<ClassOptionViewModel> AvailableClasses { get; set; } = [];


        //
        // Property Name    : Address
        // Purpose          : Stores the address.
        // Data Type        : string?
        //
        public string? Address { get; set; }


        [Required(ErrorMessage = "Please select a parent or guardian.")]
        [Display(Name = "Parent / Guardian")]
        //
        // Property Name    : ParentId
        // Purpose          : Identifies the related parent record.
        // Data Type        : Guid?
        //
        public Guid? ParentId { get; set; }


        //
        // Property Name    : SelectedParentName
        // Purpose          : Gets or sets the selected parent name.
        // Data Type        : string
        //
        public string SelectedParentName { get; set; } = "";

        //
        // Property Name    : SelectedGuardianNumber
        // Purpose          : Gets or sets the selected guardian number.
        // Data Type        : string
        //
        public string SelectedGuardianNumber { get; set; } = "";

        //
        // Property Name    : SelectedParentPhone
        // Purpose          : Gets or sets the selected parent phone.
        // Data Type        : string
        //
        public string SelectedParentPhone { get; set; } = "";

        //
        // Property Name    : SelectedParentEmail
        // Purpose          : Gets or sets the selected parent email.
        // Data Type        : string
        //
        public string SelectedParentEmail { get; set; } = "";


        [Required(ErrorMessage = "Please select the relationship.")]
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
    }
}