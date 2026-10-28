using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

namespace SchoolParentalSystem.Web.Services.SuperAdmin;

//
// Class Name       : SuperAdminDashboardService
// Purpose          : Encapsulates super admin dashboard operations for the
//                    School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminDashboardService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService _sessionService;


    //
    // Method Name      : SuperAdminDashboardService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminDashboardService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    // -----------------------------------------------------
    // SUPER ADMIN DASHBOARD
    // -----------------------------------------------------

    //
    // Method Name      : GetDashboardAsync
    // Purpose          : Retrieves dashboard.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string adminName
    // Output Type      : Task<SuperAdminDashboardViewModel>.
    //

    public async Task<SuperAdminDashboardViewModel>
        GetDashboardAsync(
            string adminName)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // SCHOOLS
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<SchoolStatusRecord>()
                .Get();

        var schools =
            schoolResponse.Models;


        // -------------------------------------------------
        // USERS
        // -------------------------------------------------

        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Get();

        var users =
            userResponse.Models;


        // -------------------------------------------------
        // SCHOOL DETAILS
        // -------------------------------------------------

        var schoolDetailResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();

        var schoolDetails =
            schoolDetailResponse.Models;


        // -------------------------------------------------
        // LEARNERS
        // -------------------------------------------------

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Get();

        var learners =
            learnerResponse.Models;


        // -------------------------------------------------
        // ANNOUNCEMENTS
        // -------------------------------------------------

        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();

        var announcements =
            announcementResponse.Models;


        // -------------------------------------------------
        // EVENTS
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();

        var events =
            eventResponse.Models;


        // -------------------------------------------------
        // UPCOMING EVENTS
        // -------------------------------------------------

        var upcomingEvents =
            events
                .Where(x =>
                    x.EventDate.Date >=
                    DateTime.UtcNow.Date)
                .OrderBy(x =>
                    x.EventDate)
                .Take(4)
                .Select(x =>
                    new SuperAdminEventItemViewModel
                    {
                        Id =
                            x.Id,

                        Month =
                            x.EventDate
                                .ToString("MMM")
                                .ToUpperInvariant(),

                        Day =
                            x.EventDate
                                .ToString("dd"),

                        Title =
                            x.EventName,

                        Time =
                            x.StartTime.ToString(),

                        SchoolName =
                            schoolDetails
                                .FirstOrDefault(s =>
                                    s.Id == x.SchoolId)
                                ?.Name
                            ?? "Unknown School"
                    })
                .ToList();


        // -------------------------------------------------
        // RECENT SCHOOLS
        // -------------------------------------------------

        var recentSchools =
            schoolDetails
                .OrderByDescending(x =>
                    x.CreatedAt)
                .Take(5)
                .Select(x =>
                    new RecentSchoolItemViewModel
                    {
                        Id =
                            x.Id,

                        SchoolName =
                            x.Name,

                        Location =
                            string.IsNullOrWhiteSpace(
                                x.TownCity)
                                ? x.Address
                                : x.TownCity,

                        LearnerCount =
                            learners.Count(learner =>
                                learner.IsActive &&
                                learner.SchoolId ==
                                x.Id),

                        Status =
                            string.IsNullOrWhiteSpace(
                                x.Status)
                                ? "Pending"
                                : x.Status
                    })
                .ToList();


        // -------------------------------------------------
        // SCHOOL ENROLLMENT
        // -------------------------------------------------

        var enrollmentOverview =
            schoolDetails
                .Select(school =>
                    new SystemEnrollmentItemViewModel
                    {
                        Label =
                            school.Name,

                        LearnerCount =
                            learners.Count(learner =>
                                learner.IsActive &&
                                learner.SchoolId ==
                                school.Id)
                    })
                .OrderByDescending(x =>
                    x.LearnerCount)
                .Take(5)
                .ToList();


        // -------------------------------------------------
        // RECENT ACTIVITY
        // -------------------------------------------------

        var recentActivities =
            schoolDetails
                .Select(x => new
                {
                    Description =
                        $"School registered: {x.Name}",

                    ActivityDate =
                        x.CreatedAt,

                    Type =
                        "School"
                })
                .Concat(
                    announcements.Select(x => new
                    {
                        Description =
                            $"Announcement created: {x.Title}",

                        ActivityDate =
                            x.CreatedAt,

                        Type =
                            "Announcement"
                    }))
                .Concat(
                    events.Select(x => new
                    {
                        Description =
                            $"Event created: {x.EventName}",

                        ActivityDate =
                            x.CreatedAt,

                        Type =
                            "Event"
                    }))
                .OrderByDescending(x =>
                    x.ActivityDate)
                .Take(6)
                .Select(x =>
                    new SuperAdminActivityItemViewModel
                    {
                        Description =
                            x.Description,

                        TimeAgo =
                            x.ActivityDate
                                .ToLocalTime()
                                .ToString(
                                    "dd MMM yyyy HH:mm"),

                        Type =
                            x.Type
                    })
                .ToList();


        // -------------------------------------------------
        // BUILD DASHBOARD
        // -------------------------------------------------

        var model =
            new SuperAdminDashboardViewModel
            {
                AdminName =
                    string.IsNullOrWhiteSpace(adminName)
                        ? "Super Admin"
                        : adminName,

                TotalSchools =
                    schools.Count,

                ActiveSchools =
                    schools.Count(x =>
                        string.Equals(
                            x.Status,
                            "Active",
                            StringComparison.OrdinalIgnoreCase)),

                PendingSchools =
                    schools.Count(x =>
                        string.Equals(
                            x.Status,
                            "Pending",
                            StringComparison.OrdinalIgnoreCase)),

                InactiveSchools =
                    schools.Count(x =>
                        string.Equals(
                            x.Status,
                            "Inactive",
                            StringComparison.OrdinalIgnoreCase)),

                TotalLearners =
                    learners.Count(x =>
                        x.IsActive),

                TotalParents =
                    users.Count(x =>
                        x.IsActive &&
                        string.Equals(
                            x.Role,
                            "Parent",
                            StringComparison.OrdinalIgnoreCase)),

                TotalTeachers =
                    users.Count(x =>
                        x.IsActive &&
                        string.Equals(
                            x.Role,
                            "Teacher",
                            StringComparison.OrdinalIgnoreCase)),

                TotalSchoolAdmins =
                    users.Count(x =>
                        x.IsActive &&
                        string.Equals(
                            x.Role,
                            "School Admin",
                            StringComparison.OrdinalIgnoreCase)),

                ActiveAnnouncements =
                    announcements.Count(x =>
                        !x.ExpiresAt.HasValue ||
                        x.ExpiresAt.Value >
                        DateTime.UtcNow),

                EnrollmentOverview =
                    enrollmentOverview,

                UpcomingEvents =
                    upcomingEvents,

                RecentActivities =
                    recentActivities,

                RecentSchools =
                    recentSchools
            };


        return model;
    }


    // -----------------------------------------------------
    // SYSTEM ACTIVITY
    // -----------------------------------------------------

    //
    // Method Name      : GetSystemActivityAsync
    // Purpose          : Retrieves system Activity.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<SystemActivityViewModel>.
    //

    public async Task<SystemActivityViewModel>
        GetSystemActivityAsync()
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();

        var schools =
            schoolResponse.Models;


        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();

        var announcements =
            announcementResponse.Models;


        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();

        var events =
            eventResponse.Models;


        var activities =
            schools
                .Select(x => new
                {
                    Description =
                        $"School registered: {x.Name}",

                    ActivityDate =
                        x.CreatedAt,

                    Type =
                        "School"
                })
                .Concat(
                    announcements.Select(x => new
                    {
                        Description =
                            $"Announcement created: {x.Title}",

                        ActivityDate =
                            x.CreatedAt,

                        Type =
                            "Announcement"
                    }))
                .Concat(
                    events.Select(x => new
                    {
                        Description =
                            $"Event created: {x.EventName}",

                        ActivityDate =
                            x.CreatedAt,

                        Type =
                            "Event"
                    }))
                .OrderByDescending(x =>
                    x.ActivityDate)
                .Select(x =>
                    new SuperAdminActivityItemViewModel
                    {
                        Description =
                            x.Description,

                        TimeAgo =
                            x.ActivityDate
                                .ToLocalTime()
                                .ToString(
                                    "dd MMM yyyy HH:mm"),

                        Type =
                            x.Type
                    })
                .ToList();


        return new SystemActivityViewModel
        {
            Activities =
                activities
        };
    }
}