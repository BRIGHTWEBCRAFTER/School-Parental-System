using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.ViewModels
{
    public class LearnerDetailsViewModel
    {
        public Guid Id { get; set; }

        public string AdmissionNumber { get; set; } = "";

        public string FullName { get; set; } = "";

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = "";

        public string Grade { get; set; } = "";

        public string ClassName { get; set; } = "Not assigned";

        public string Address { get; set; } = "";

        public bool IsActive { get; set; }

        public string Status =>
            IsActive ? "Active" : "Inactive";


        // Guardian information
        public string GuardianName { get; set; } =
            "Not linked";

        public string GuardianNumber { get; set; } =
            "";

        public string GuardianPhone { get; set; } =
            "";

        public string GuardianEmail { get; set; } =
            "";

        public string Relationship { get; set; } =
            "";
    }
}