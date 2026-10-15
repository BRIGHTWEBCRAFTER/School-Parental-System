using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels
{
    public class EditLearnerViewModel
    {
        public Guid Id { get; set; }

        public string AdmissionNumber { get; set; } = "";


        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "";


        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = "";


        [Required(ErrorMessage = "Grade is required.")]
        public string Grade { get; set; } = "";


        [Display(Name = "Class")]
        public Guid? ClassId { get; set; }

        public List<ClassOptionViewModel> AvailableClasses { get; set; } = [];


        public string? Address { get; set; }


        [Required(ErrorMessage = "Please select a parent or guardian.")]
        [Display(Name = "Parent / Guardian")]
        public Guid? ParentId { get; set; }


        public string SelectedParentName { get; set; } = "";

        public string SelectedGuardianNumber { get; set; } = "";

        public string SelectedParentPhone { get; set; } = "";

        public string SelectedParentEmail { get; set; } = "";


        [Required(ErrorMessage = "Please select the relationship.")]
        public string Relationship { get; set; } = "";


        public bool IsActive { get; set; }
    }
}