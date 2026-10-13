namespace SchoolParentalSystem.Web.ViewModels;

public class LearnerDetailsViewModel
{
    public string Id { get; set; } = "";
    public string AdmissionNumber { get; set; } = "";
    public string FullName { get; set; } = "";

    public DateTime? DateOfBirth { get; set; }

    public string Gender { get; set; } = "";
    public string Grade { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Address { get; set; } = "";

    public string GuardianName { get; set; } = "";
    public string Relationship { get; set; } = "";

    public bool IsActive { get; set; }
}