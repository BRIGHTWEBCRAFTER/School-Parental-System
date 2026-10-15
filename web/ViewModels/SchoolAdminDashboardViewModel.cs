namespace SchoolParentalSystem.Web.ViewModels;

public class SchoolAdminDashboardViewModel
{
    public string AdminName { get; set; } = "";
    public string SchoolName { get; set; } = "";

    public int TotalLearners { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalClasses { get; set; }

    public decimal AttendanceRate { get; set; }
    public int ActiveAnnouncements { get; set; }

    public List<AttendanceOverviewItem> AttendanceOverview { get; set; } = [];
    public List<AnnouncementItem> RecentAnnouncements { get; set; } = [];
    public List<EventItem> UpcomingEvents { get; set; } = [];
    public List<ActivityItem> RecentActivities { get; set; } = [];
    public List<ClassPerformanceItem> ClassPerformance { get; set; } = [];
}

public class AttendanceOverviewItem
{
    public string Date { get; set; } = "";
    public decimal Percentage { get; set; }
}

public class AnnouncementItem
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

public class EventItem
{
    public string Month { get; set; } = "";
    public string Day { get; set; } = "";
    public string Title { get; set; } = "";
}

public class ActivityItem
{
    public string Description { get; set; } = "";
    public string TimeAgo { get; set; } = "";
    public string Type { get; set; } = "";
}

public class ClassPerformanceItem
{
    public string ClassName { get; set; } = "";
    public int LearnerCount { get; set; }
    public decimal AverageGrade { get; set; }
    public string Status { get; set; } = "";
}