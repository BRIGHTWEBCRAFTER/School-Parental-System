namespace SchoolParentalSystem.Web.ViewModels;

public class TeachersPageViewModel
{
    public int TotalTeachers { get; set; }

    public int ActiveTeachers { get; set; }

    public int InactiveTeachers { get; set; }

    public List<TeacherListItemViewModel> Teachers { get; set; } = [];
}


public class TeacherListItemViewModel
{
    public Guid Id { get; set; }

    public string EmployeeId { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Phone { get; set; } = "";

    public string Specialization { get; set; } = "";

    public DateTime? JoiningDate { get; set; }

    public bool IsActive { get; set; }

    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}