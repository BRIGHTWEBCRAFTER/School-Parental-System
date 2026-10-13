namespace SchoolParentalSystem.Web.ViewModels;

public class LearnersPageViewModel
{
    public int TotalLearners { get; set; }
    public int ActiveLearners { get; set; }
    public int InactiveLearners { get; set; }

    public List<LearnerListItemViewModel> Learners { get; set; } = [];
}

public class LearnerListItemViewModel
{
    public string Id { get; set; } = "";
    public string AdmissionNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Grade { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string Gender { get; set; } = "";
    public string GuardianName { get; set; } = "";
    public string Status { get; set; } = "Active";
}