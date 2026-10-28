using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

public class SchoolAdminCommunicationService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;


    public SchoolAdminCommunicationService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    public async Task<CommunicationManagementViewModel>
        GetCommunicationAsync(
            Guid schoolId,
            string? searchTerm = null,
            string? priority = null,
            string? targetType = null,
            string? status = null)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // LOAD SCHOOL CLASSES
        // -------------------------------------------------

        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();


        var classes =
            classResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD ANNOUNCEMENTS
        // -------------------------------------------------

        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();


        var schoolAnnouncements =
            announcementResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD ANNOUNCEMENT RECIPIENTS
        // -------------------------------------------------

        var recipientResponse =
            await _supabase
                .From<AnnouncementRecipientRecord>()
                .Get();


        var schoolAnnouncementIds =
            schoolAnnouncements
                .Select(x => x.Id)
                .ToHashSet();


        var recipients =
            recipientResponse.Models
                .Where(x =>
                    schoolAnnouncementIds.Contains(
                        x.AnnouncementId))
                .ToList();


        // -------------------------------------------------
        // VALIDATE FILTER VALUES
        // -------------------------------------------------

        var validPriorities =
            new[]
            {
                "Normal",
                "Important",
                "Urgent"
            };


        var validTargets =
            new[]
            {
                "Whole School",
                "Role",
                "Grade",
                "Class",
                "Specific Users"
            };


        var validStatuses =
            new[]
            {
                "Active",
                "Expired"
            };


        if (!string.IsNullOrWhiteSpace(priority) &&
            !validPriorities.Contains(
                priority,
                StringComparer.OrdinalIgnoreCase))
        {
            priority = null;
        }


        if (!string.IsNullOrWhiteSpace(targetType) &&
            !validTargets.Contains(
                targetType,
                StringComparer.OrdinalIgnoreCase))
        {
            targetType = null;
        }


        if (!string.IsNullOrWhiteSpace(status) &&
            !validStatuses.Contains(
                status,
                StringComparer.OrdinalIgnoreCase))
        {
            status = null;
        }


        // -------------------------------------------------
        // BUILD ANNOUNCEMENT LIST
        // -------------------------------------------------

        var announcements =
            new List<CommunicationAnnouncementItemViewModel>();


        foreach (var announcement in schoolAnnouncements)
        {
            var announcementRecipients =
                recipients
                    .Where(x =>
                        x.AnnouncementId ==
                        announcement.Id)
                    .ToList();


            var recipientCount =
                announcementRecipients.Count;


            var readCount =
                announcementRecipients.Count(x =>
                    x.IsRead);


            var acknowledgedCount =
                announcementRecipients.Count(x =>
                    x.IsAcknowledged);


            var announcementStatus =
                announcement.ExpiresAt.HasValue &&
                announcement.ExpiresAt.Value <
                    DateTime.UtcNow
                    ? "Expired"
                    : "Active";


            var targetDisplay =
                BuildTargetDisplay(
                    announcement,
                    classes);


            announcements.Add(
                new CommunicationAnnouncementItemViewModel
                {
                    Id =
                        announcement.Id,

                    Title =
                        announcement.Title,

                    Content =
                        announcement.Content,

                    Priority =
                        announcement.Priority,

                    TargetDisplay =
                        targetDisplay,

                    AcknowledgementRequired =
                        announcement.AcknowledgementRequired,

                    RecipientCount =
                        recipientCount,

                    ReadCount =
                        readCount,

                    AcknowledgedCount =
                        acknowledgedCount,

                    Status =
                        announcementStatus,

                    CreatedAt =
                        announcement.CreatedAt,

                    ExpiresAt =
                        announcement.ExpiresAt
                });
        }


        // -------------------------------------------------
        // APPLY SEARCH FILTER
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                searchTerm))
        {
            var search =
                searchTerm.Trim();


            announcements =
                announcements
                    .Where(x =>
                        x.Title.Contains(
                            search,
                            StringComparison
                                .OrdinalIgnoreCase) ||

                        x.Content.Contains(
                            search,
                            StringComparison
                                .OrdinalIgnoreCase))
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY PRIORITY FILTER
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                priority))
        {
            announcements =
                announcements
                    .Where(x =>
                        string.Equals(
                            x.Priority,
                            priority,
                            StringComparison
                                .OrdinalIgnoreCase))
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY TARGET FILTER
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                targetType))
        {
            announcements =
                announcements
                    .Where(x =>
                    {
                        var source =
                            schoolAnnouncements
                                .FirstOrDefault(a =>
                                    a.Id == x.Id);

                        return source != null &&
                               string.Equals(
                                   source.TargetType,
                                   targetType,
                                   StringComparison
                                       .OrdinalIgnoreCase);
                    })
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY STATUS FILTER
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                status))
        {
            announcements =
                announcements
                    .Where(x =>
                        string.Equals(
                            x.Status,
                            status,
                            StringComparison
                                .OrdinalIgnoreCase))
                    .ToList();
        }


        announcements =
            announcements
                .OrderByDescending(x =>
                    x.CreatedAt)
                .ToList();


        // -------------------------------------------------
        // LOAD UPCOMING EVENTS
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();


        var schoolEvents =
            eventResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId &&
                    x.EventDate.Date >=
                        DateTime.Today)
                .OrderBy(x =>
                    x.EventDate)
                .ThenBy(x =>
                    x.StartTime)
                .ToList();


        var upcomingEvents =
            new List<CommunicationEventItemViewModel>();


        foreach (var schoolEvent in schoolEvents)
        {
            var targetDisplay =
                BuildEventTargetDisplay(
                    schoolEvent,
                    classes);


            upcomingEvents.Add(
                new CommunicationEventItemViewModel
                {
                    Id =
                        schoolEvent.Id,

                    EventName =
                        schoolEvent.EventName,

                    Description =
                        schoolEvent.Description ?? "",

                    EventDate =
                        schoolEvent.EventDate,

                    StartTime =
                        schoolEvent.StartTime,

                    EndTime =
                        schoolEvent.EndTime,

                    Venue =
                        schoolEvent.Venue ?? "",

                    TargetDisplay =
                        targetDisplay
                });
        }


        // -------------------------------------------------
        // RETURN VIEW MODEL
        // -------------------------------------------------

        return new CommunicationManagementViewModel
        {
            SearchTerm =
                searchTerm ?? "",

            SelectedPriority =
                priority ?? "",

            SelectedTarget =
                targetType ?? "",

            SelectedStatus =
                status ?? "",

            Announcements =
                announcements,

            UpcomingEvents =
                upcomingEvents
        };
    }


    public async Task<CommunicationStatisticsViewModel>
        GetCommunicationStatisticsAsync(Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }

        // -----------------------------------------
        // LOAD SCHOOL ANNOUNCEMENTS
        // -----------------------------------------

        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();

        var schoolAnnouncements =
            announcementResponse.Models
                .Where(x => x.SchoolId == schoolId)
                .ToList();

        var schoolAnnouncementIds =
            schoolAnnouncements
                .Select(x => x.Id)
                .ToHashSet();

        // -----------------------------------------
        // LOAD ANNOUNCEMENT RECIPIENTS
        // -----------------------------------------

        var recipientResponse =
            await _supabase
                .From<AnnouncementRecipientRecord>()
                .Get();

        var recipients =
            recipientResponse.Models
                .Where(x =>
                    schoolAnnouncementIds.Contains(
                        x.AnnouncementId))
                .ToList();

        // -----------------------------------------
        // LOAD SCHOOL EVENTS
        // -----------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();

        var schoolEvents =
            eventResponse.Models
                .Where(x => x.SchoolId == schoolId)
                .ToList();

        // -----------------------------------------
        // ANNOUNCEMENT STATUS
        // -----------------------------------------

        var now = DateTime.UtcNow;

        var expiredAnnouncements =
            schoolAnnouncements.Count(x =>
                x.ExpiresAt.HasValue &&
                x.ExpiresAt.Value < now);

        var activeAnnouncements =
            schoolAnnouncements.Count -
            expiredAnnouncements;

        // -----------------------------------------
        // PRIORITY COUNTS
        // -----------------------------------------

        var normalAnnouncements =
            schoolAnnouncements.Count(x =>
                string.Equals(
                    x.Priority,
                    "Normal",
                    StringComparison.OrdinalIgnoreCase));

        var importantAnnouncements =
            schoolAnnouncements.Count(x =>
                string.Equals(
                    x.Priority,
                    "Important",
                    StringComparison.OrdinalIgnoreCase));

        var urgentAnnouncements =
            schoolAnnouncements.Count(x =>
                string.Equals(
                    x.Priority,
                    "Urgent",
                    StringComparison.OrdinalIgnoreCase));

        // -----------------------------------------
        // RECIPIENT ENGAGEMENT
        // -----------------------------------------

        var totalRecipients =
            recipients.Count;

        var readRecipients =
            recipients.Count(x => x.IsRead);

        var unreadRecipients =
            totalRecipients - readRecipients;

        // -----------------------------------------
        // ACKNOWLEDGEMENTS
        // -----------------------------------------

        var acknowledgementAnnouncementIds =
            schoolAnnouncements
                .Where(x => x.AcknowledgementRequired)
                .Select(x => x.Id)
                .ToHashSet();

        var requiredAcknowledgementRecipients =
            recipients
                .Where(x =>
                    acknowledgementAnnouncementIds.Contains(
                        x.AnnouncementId))
                .ToList();

        var acknowledgementRequiredCount =
            requiredAcknowledgementRecipients.Count;

        var acknowledgedCount =
            requiredAcknowledgementRecipients.Count(x =>
                x.IsAcknowledged);

        // -----------------------------------------
        // EVENT STATISTICS
        // -----------------------------------------

        var today = DateTime.Today;

        var upcomingEvents =
            schoolEvents.Count(x =>
                x.EventDate.Date >= today);

        var pastEvents =
            schoolEvents.Count(x =>
                x.EventDate.Date < today);

        // -----------------------------------------
        // MONTHLY ACTIVITY - LAST 6 MONTHS
        // -----------------------------------------

        var monthlyActivity =
            new List<CommunicationMonthlyActivityViewModel>();

        var currentMonth =
            new DateTime(
                today.Year,
                today.Month,
                1);

        for (var i = 5; i >= 0; i--)
        {
            var month =
                currentMonth.AddMonths(-i);

            var announcementCount =
                schoolAnnouncements.Count(x =>
                    x.CreatedAt.Year == month.Year &&
                    x.CreatedAt.Month == month.Month);

            var eventCount =
                schoolEvents.Count(x =>
                    x.EventDate.Year == month.Year &&
                    x.EventDate.Month == month.Month);

            monthlyActivity.Add(
                new CommunicationMonthlyActivityViewModel
                {
                    MonthLabel =
                        month.ToString("MMM yyyy"),

                    Announcements =
                        announcementCount,

                    Events =
                        eventCount
                });
        }

        // -----------------------------------------
        // RETURN STATISTICS VIEW MODEL
        // -----------------------------------------

        return new CommunicationStatisticsViewModel
        {
            TotalAnnouncements =
                schoolAnnouncements.Count,

            ActiveAnnouncements =
                activeAnnouncements,

            ExpiredAnnouncements =
                expiredAnnouncements,

            NormalAnnouncements =
                normalAnnouncements,

            ImportantAnnouncements =
                importantAnnouncements,

            UrgentAnnouncements =
                urgentAnnouncements,

            TotalRecipients =
                totalRecipients,

            ReadRecipients =
                readRecipients,

            UnreadRecipients =
                unreadRecipients,

            AcknowledgementRequiredRecipients =
                acknowledgementRequiredCount,

            AcknowledgedRecipients =
                acknowledgedCount,

            TotalEvents =
                schoolEvents.Count,

            UpcomingEvents =
                upcomingEvents,

            PastEvents =
                pastEvents,

            MonthlyActivity =
                monthlyActivity
        };
    }


    public async Task<CreateAnnouncementViewModel>
    GetCreateAnnouncementAsync(
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();


        var classes =
            classResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId &&
                    x.IsActive)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToList();


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Get();


        var users =
            userResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId &&
                    x.IsActive)
                .OrderBy(x => x.Role)
                .ThenBy(x => x.FullName)
                .ToList();


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Get();


        var grades =
            learnerResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId &&
                    x.IsActive)
                .Select(x =>
                    x.Grade)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();


        return new CreateAnnouncementViewModel
        {
            Classes =
                classes
                    .Select(x =>
                        new AnnouncementClassOptionViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code
                        })
                    .ToList(),

            Users =
                users
                    .Select(x =>
                        new AnnouncementUserOptionViewModel
                        {
                            Id = x.Id,
                            FullName = x.FullName,
                            Role = x.Role,
                            Email = x.Email ?? ""
                        })
                    .ToList(),

            Grades =
                grades
        };
    }

    public async Task PopulateCreateAnnouncementOptionsAsync(
    CreateAnnouncementViewModel model,
    Guid schoolId)
    {
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        model.Classes =
            options.Classes;

        model.Users =
            options.Users;

        model.Grades =
            options.Grades;
    }

    public async Task<Guid> CreateAnnouncementAsync(
    CreateAnnouncementViewModel model,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var validPriorities =
            new[]
            {
            "Normal",
            "Important",
            "Urgent"
            };


        var validTargets =
            new[]
            {
            "Whole School",
            "Role",
            "Grade",
            "Class",
            "Specific Users"
            };


        if (!validPriorities.Contains(
                model.Priority))
        {
            throw new InvalidOperationException(
                "Invalid announcement priority.");
        }


        if (!validTargets.Contains(
                model.TargetType))
        {
            throw new InvalidOperationException(
                "Invalid target audience.");
        }


        if (model.AcknowledgementRequired &&
            model.Priority == "Normal")
        {
            throw new InvalidOperationException(
                "Only Important or Urgent announcements can require acknowledgement.");
        }


        if (model.TargetType == "Role" &&
            string.IsNullOrWhiteSpace(
                model.TargetRole))
        {
            throw new InvalidOperationException(
                "Please select a target role.");
        }


        if (model.TargetType == "Grade" &&
            string.IsNullOrWhiteSpace(
                model.TargetGrade))
        {
            throw new InvalidOperationException(
                "Please select a target grade.");
        }


        if (model.TargetType == "Class" &&
            !model.TargetClassId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a target class.");
        }


        if (model.TargetType ==
                "Specific Users" &&
            model.SelectedUserIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Please select at least one user.");
        }


        if (model.ExpiresAt.HasValue &&
            model.ExpiresAt.Value <=
                DateTime.Now)
        {
            throw new InvalidOperationException(
                "Expiry date must be in the future.");
        }


        var parameters =
            new
            {
                p_school_id =
                    schoolId,

                p_title =
                    model.Title.Trim(),

                p_content =
                    model.Content.Trim(),

                p_priority =
                    model.Priority,

                p_target_type =
                    model.TargetType,

                p_target_role =
                    model.TargetType == "Role"
                        ? model.TargetRole
                        : null,

                p_target_grade =
                    model.TargetType == "Grade"
                        ? model.TargetGrade
                        : null,

                p_target_class_id =
                    model.TargetType == "Class"
                        ? model.TargetClassId
                        : null,

                p_acknowledgement_required =
                    model.AcknowledgementRequired,

                p_expires_at =
                    model.ExpiresAt,

                p_specific_user_ids =
                    model.TargetType ==
                        "Specific Users"
                        ? model.SelectedUserIds
                            .ToArray()
                        : null
            };


        var created = await _supabase.Rpc(
            "create_school_announcement",
            parameters);

        // The RPC returns the created announcement UUID.
        return Guid.Parse((created.Content ?? "").Trim().Trim('"'));
    }

    public async Task<EditAnnouncementViewModel?>
    GetEditAnnouncementAsync(
        Guid announcementId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();


        var announcement =
            announcementResponse.Models
                .FirstOrDefault(x =>
                    x.Id == announcementId &&
                    x.SchoolId == schoolId);


        if (announcement == null)
        {
            return null;
        }


        var createOptions =
            await GetCreateAnnouncementAsync(
                schoolId);


        var recipientResponse =
            await _supabase
                .From<AnnouncementRecipientRecord>()
                .Get();


        var selectedUserIds =
            recipientResponse.Models
                .Where(x =>
                    x.AnnouncementId ==
                    announcementId)
                .Select(x =>
                    x.UserId)
                .ToList();


        return new EditAnnouncementViewModel
        {
            Id =
                announcement.Id,

            Title =
                announcement.Title,

            Content =
                announcement.Content,

            Priority =
                announcement.Priority,

            TargetType =
                announcement.TargetType,

            TargetRole =
                announcement.TargetRole,

            TargetGrade =
                announcement.TargetGrade,

            TargetClassId =
                announcement.TargetClassId,

            AcknowledgementRequired =
                announcement.AcknowledgementRequired,

            ExpiresAt =
                announcement.ExpiresAt,

            SelectedUserIds =
                selectedUserIds,

            Classes =
                createOptions.Classes,

            Users =
                createOptions.Users,

            Grades =
                createOptions.Grades
        };
    }

    public async Task PopulateEditAnnouncementOptionsAsync(
    EditAnnouncementViewModel model,
    Guid schoolId)
    {
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        model.Classes =
            options.Classes;

        model.Users =
            options.Users;

        model.Grades =
            options.Grades;
    }

    public async Task UpdateAnnouncementAsync(
    EditAnnouncementViewModel model,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var validPriorities =
            new[]
            {
            "Normal",
            "Important",
            "Urgent"
            };


        var validTargets =
            new[]
            {
            "Whole School",
            "Role",
            "Grade",
            "Class",
            "Specific Users"
            };


        if (!validPriorities.Contains(
                model.Priority))
        {
            throw new InvalidOperationException(
                "Invalid announcement priority.");
        }


        if (!validTargets.Contains(
                model.TargetType))
        {
            throw new InvalidOperationException(
                "Invalid target audience.");
        }


        if (model.AcknowledgementRequired &&
            model.Priority == "Normal")
        {
            throw new InvalidOperationException(
                "Only Important or Urgent announcements can require acknowledgement.");
        }


        if (model.TargetType == "Role" &&
            string.IsNullOrWhiteSpace(
                model.TargetRole))
        {
            throw new InvalidOperationException(
                "Please select a target role.");
        }


        if (model.TargetType == "Grade" &&
            string.IsNullOrWhiteSpace(
                model.TargetGrade))
        {
            throw new InvalidOperationException(
                "Please select a target grade.");
        }


        if (model.TargetType == "Class" &&
            !model.TargetClassId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a target class.");
        }


        if (model.TargetType ==
                "Specific Users" &&
            model.SelectedUserIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Please select at least one user.");
        }


        if (model.ExpiresAt.HasValue &&
            model.ExpiresAt.Value <=
                DateTime.Now)
        {
            throw new InvalidOperationException(
                "Expiry date must be in the future.");
        }


        var parameters =
            new
            {
                p_announcement_id =
                    model.Id,

                p_school_id =
                    schoolId,

                p_title =
                    model.Title.Trim(),

                p_content =
                    model.Content.Trim(),

                p_priority =
                    model.Priority,

                p_target_type =
                    model.TargetType,

                p_target_role =
                    model.TargetType == "Role"
                        ? model.TargetRole
                        : null,

                p_target_grade =
                    model.TargetType == "Grade"
                        ? model.TargetGrade
                        : null,

                p_target_class_id =
                    model.TargetType == "Class"
                        ? model.TargetClassId
                        : null,

                p_acknowledgement_required =
                    model.AcknowledgementRequired,

                p_expires_at =
                    model.ExpiresAt,

                p_specific_user_ids =
                    model.TargetType ==
                        "Specific Users"
                        ? model.SelectedUserIds
                            .ToArray()
                        : null
            };


        await _supabase.Rpc(
            "update_school_announcement",
            parameters);
    }


    private static string BuildTargetDisplay(
        AnnouncementRecord announcement,
        List<ClassRecord> classes)
    {
        return announcement.TargetType switch
        {
            "Whole School" =>
                "Whole School",

            "Role" =>
                string.IsNullOrWhiteSpace(
                    announcement.TargetRole)
                    ? "Role"
                    : $"Role: {announcement.TargetRole}",

            "Grade" =>
                string.IsNullOrWhiteSpace(
                    announcement.TargetGrade)
                    ? "Grade"
                    : announcement.TargetGrade,

            "Class" =>
                BuildClassTarget(
                    announcement.TargetClassId,
                    classes),

            "Specific Users" =>
                "Specific Users",

            _ =>
                announcement.TargetType
        };
    }

    public async Task<CommunicationAnnouncementItemViewModel?>
    GetAnnouncementDetailsAsync(
        Guid announcementId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // LOAD SCHOOL CLASSES
        // -------------------------------------------------

        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();


        var classes =
            classResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD ANNOUNCEMENT
        // -------------------------------------------------

        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();


        var announcement =
            announcementResponse.Models
                .FirstOrDefault(x =>
                    x.Id == announcementId &&
                    x.SchoolId == schoolId);


        if (announcement == null)
        {
            return null;
        }


        // -------------------------------------------------
        // LOAD RECIPIENT ACTIVITY
        // -------------------------------------------------

        var recipientResponse =
            await _supabase
                .From<AnnouncementRecipientRecord>()
                .Get();


        var recipients =
            recipientResponse.Models
                .Where(x =>
                    x.AnnouncementId ==
                    announcement.Id)
                .ToList();


        var recipientCount =
            recipients.Count;


        var readCount =
            recipients.Count(x =>
                x.IsRead);


        var acknowledgedCount =
            recipients.Count(x =>
                x.IsAcknowledged);


        // -------------------------------------------------
        // DETERMINE STATUS
        // -------------------------------------------------

        var announcementStatus =
            announcement.ExpiresAt.HasValue &&
            announcement.ExpiresAt.Value <
                DateTime.UtcNow
                ? "Expired"
                : "Active";


        // -------------------------------------------------
        // BUILD TARGET DISPLAY
        // -------------------------------------------------

        var targetDisplay =
            BuildTargetDisplay(
                announcement,
                classes);


        // -------------------------------------------------
        // RETURN DETAILS
        // -------------------------------------------------

        return new CommunicationAnnouncementItemViewModel
        {
            Id =
                announcement.Id,

            Title =
                announcement.Title,

            Content =
                announcement.Content,

            Priority =
                announcement.Priority,

            TargetDisplay =
                targetDisplay,

            AcknowledgementRequired =
                announcement.AcknowledgementRequired,

            RecipientCount =
                recipientCount,

            ReadCount =
                readCount,

            AcknowledgedCount =
                acknowledgedCount,

            Status =
                announcementStatus,

            CreatedAt =
                announcement.CreatedAt,

            ExpiresAt =
                announcement.ExpiresAt
        };
    }

    public async Task DeleteAnnouncementAsync(
    Guid announcementId,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        if (announcementId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Invalid announcement.");
        }


        var parameters =
            new
            {
                p_announcement_id =
                    announcementId,

                p_school_id =
                    schoolId
            };


        await _supabase.Rpc(
            "delete_school_announcement",
            parameters);
    }

    public async Task<CreateEventViewModel>
    GetCreateEventAsync(
        Guid schoolId)
    {
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        return new CreateEventViewModel
        {
            EventDate =
                DateTime.Today,

            Classes =
                options.Classes,

            Grades =
                options.Grades
        };
    }

    public async Task PopulateCreateEventOptionsAsync(
    CreateEventViewModel model,
    Guid schoolId)
    {
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        model.Classes =
            options.Classes;

        model.Grades =
            options.Grades;
    }

    public async Task<Guid> CreateEventAsync(
    CreateEventViewModel model,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var validTargets =
            new[]
            {
            "Whole School",
            "Role",
            "Grade",
            "Class"
            };


        var validRoles =
            new[]
            {
            "School Admin",
            "Teacher",
            "Parent"
            };


        if (string.IsNullOrWhiteSpace(
                model.EventName))
        {
            throw new InvalidOperationException(
                "Event name is required.");
        }


        if (!validTargets.Contains(
                model.TargetType))
        {
            throw new InvalidOperationException(
                "Invalid target audience.");
        }


        if (model.EventDate.Date <
            DateTime.Today)
        {
            throw new InvalidOperationException(
                "Event date cannot be in the past.");
        }


        if (model.StartTime.HasValue &&
            model.EndTime.HasValue &&
            model.EndTime.Value <=
                model.StartTime.Value)
        {
            throw new InvalidOperationException(
                "End time must be later than start time.");
        }


        if (model.TargetType == "Role")
        {
            if (string.IsNullOrWhiteSpace(
                    model.TargetRole) ||
                !validRoles.Contains(
                    model.TargetRole))
            {
                throw new InvalidOperationException(
                    "Please select a valid target role.");
            }
        }


        if (model.TargetType == "Grade" &&
            string.IsNullOrWhiteSpace(
                model.TargetGrade))
        {
            throw new InvalidOperationException(
                "Please select a target grade.");
        }


        if (model.TargetType == "Class" &&
            !model.TargetClassId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a target class.");
        }


        var parameters =
            new
            {
                p_school_id =
                    schoolId,

                p_event_name =
                    model.EventName.Trim(),

                p_description =
                    string.IsNullOrWhiteSpace(
                        model.Description)
                        ? null
                        : model.Description.Trim(),

                p_event_date =
                    model.EventDate.ToString(
                        "yyyy-MM-dd"),

                p_start_time =
                    model.StartTime.HasValue
                        ? model.StartTime.Value
                            .ToString(@"hh\:mm\:ss")
                        : null,

                p_end_time =
                    model.EndTime.HasValue
                        ? model.EndTime.Value
                            .ToString(@"hh\:mm\:ss")
                        : null,

                p_venue =
                    string.IsNullOrWhiteSpace(
                        model.Venue)
                        ? null
                        : model.Venue.Trim(),

                p_target_type =
                    model.TargetType,

                p_target_role =
                    model.TargetType == "Role"
                        ? model.TargetRole
                        : null,

                p_target_grade =
                    model.TargetType == "Grade"
                        ? model.TargetGrade
                        : null,

                p_target_class_id =
                    model.TargetType == "Class"
                        ? model.TargetClassId
                        : null
            };


        var created = await _supabase.Rpc(
            "create_school_event_with_notifications",
            parameters);

        // The RPC returns the created event UUID.
        return Guid.Parse((created.Content ?? "").Trim().Trim('"'));
    }

    public async Task<EventManagementViewModel>
    GetEventsAsync(
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // LOAD SCHOOL CLASSES
        // -------------------------------------------------

        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();


        var classes =
            classResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD SCHOOL EVENTS
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();


        var schoolEvents =
            eventResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .OrderByDescending(x =>
                    x.EventDate)
                .ThenByDescending(x =>
                    x.StartTime)
                .ToList();


        // -------------------------------------------------
        // BUILD EVENT VIEW MODELS
        // -------------------------------------------------

        var events =
            new List<CommunicationEventItemViewModel>();


        foreach (var schoolEvent in schoolEvents)
        {
            var targetDisplay =
                BuildEventTargetDisplay(
                    schoolEvent,
                    classes);


            events.Add(
                new CommunicationEventItemViewModel
                {
                    Id =
                        schoolEvent.Id,

                    EventName =
                        schoolEvent.EventName,

                    Description =
                        schoolEvent.Description ?? "",

                    EventDate =
                        schoolEvent.EventDate,

                    StartTime =
                        schoolEvent.StartTime,

                    EndTime =
                        schoolEvent.EndTime,

                    Venue =
                        schoolEvent.Venue ?? "",

                    TargetDisplay =
                        targetDisplay
                });
        }


        return new EventManagementViewModel
        {
            Events =
                events
        };
    }

    public async Task<CommunicationEventItemViewModel?>
    GetEventDetailsAsync(
        Guid eventId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // LOAD SCHOOL CLASSES
        // -------------------------------------------------

        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();


        var classes =
            classResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD EVENT
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();


        var schoolEvent =
            eventResponse.Models
                .FirstOrDefault(x =>
                    x.Id == eventId &&
                    x.SchoolId == schoolId);


        if (schoolEvent == null)
        {
            return null;
        }


        // -------------------------------------------------
        // BUILD TARGET DISPLAY
        // -------------------------------------------------

        var targetDisplay =
            BuildEventTargetDisplay(
                schoolEvent,
                classes);


        // -------------------------------------------------
        // RETURN EVENT DETAILS
        // -------------------------------------------------

        return new CommunicationEventItemViewModel
        {
            Id =
                schoolEvent.Id,

            EventName =
                schoolEvent.EventName,

            Description =
                schoolEvent.Description ?? "",

            EventDate =
                schoolEvent.EventDate,

            StartTime =
                schoolEvent.StartTime,

            EndTime =
                schoolEvent.EndTime,

            Venue =
                schoolEvent.Venue ?? "",

            TargetDisplay =
                targetDisplay
        };
    }

    public async Task<EditEventViewModel?>
    GetEditEventAsync(
        Guid eventId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // Load dropdown options
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        // Load event
        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();


        var schoolEvent =
            eventResponse.Models
                .FirstOrDefault(x =>
                    x.Id == eventId &&
                    x.SchoolId == schoolId);


        if (schoolEvent == null)
        {
            return null;
        }


        return new EditEventViewModel
        {
            Id =
                schoolEvent.Id,

            EventName =
                schoolEvent.EventName,

            Description =
                schoolEvent.Description,

            EventDate =
                schoolEvent.EventDate,

            StartTime =
                schoolEvent.StartTime,

            EndTime =
                schoolEvent.EndTime,

            Venue =
                schoolEvent.Venue,

            TargetType =
                schoolEvent.TargetType,

            TargetRole =
                schoolEvent.TargetRole,

            TargetGrade =
                schoolEvent.TargetGrade,

            TargetClassId =
                schoolEvent.TargetClassId,

            Classes =
                options.Classes,

            Grades =
                options.Grades
        };
    }


    public async Task PopulateEditEventOptionsAsync(
        EditEventViewModel model,
        Guid schoolId)
    {
        var options =
            await GetCreateAnnouncementAsync(
                schoolId);


        model.Classes =
            options.Classes;

        model.Grades =
            options.Grades;
    }


    public async Task UpdateEventAsync(
        EditEventViewModel model,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var validTargets =
            new[]
            {
            "Whole School",
            "Role",
            "Grade",
            "Class"
            };


        var validRoles =
            new[]
            {
            "School Admin",
            "Teacher",
            "Parent"
            };


        if (model.Id == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A valid event is required.");
        }


        if (string.IsNullOrWhiteSpace(
                model.EventName))
        {
            throw new InvalidOperationException(
                "Event name is required.");
        }


        if (!validTargets.Contains(
                model.TargetType))
        {
            throw new InvalidOperationException(
                "Invalid target audience.");
        }


        if (model.StartTime.HasValue &&
            model.EndTime.HasValue &&
            model.EndTime.Value <=
                model.StartTime.Value)
        {
            throw new InvalidOperationException(
                "End time must be later than start time.");
        }


        if (model.TargetType == "Role")
        {
            if (string.IsNullOrWhiteSpace(
                    model.TargetRole) ||
                !validRoles.Contains(
                    model.TargetRole))
            {
                throw new InvalidOperationException(
                    "Please select a valid target role.");
            }
        }


        if (model.TargetType == "Grade" &&
            string.IsNullOrWhiteSpace(
                model.TargetGrade))
        {
            throw new InvalidOperationException(
                "Please select a target grade.");
        }


        if (model.TargetType == "Class" &&
            !model.TargetClassId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a target class.");
        }


        var parameters =
            new
            {
                p_event_id =
                    model.Id,

                p_school_id =
                    schoolId,

                p_event_name =
                    model.EventName.Trim(),

                p_description =
                    string.IsNullOrWhiteSpace(
                        model.Description)
                        ? null
                        : model.Description.Trim(),

                p_event_date =
                    model.EventDate.ToString(
                        "yyyy-MM-dd"),

                p_start_time =
                    model.StartTime.HasValue
                        ? model.StartTime.Value
                            .ToString(@"hh\:mm\:ss")
                        : null,

                p_end_time =
                    model.EndTime.HasValue
                        ? model.EndTime.Value
                            .ToString(@"hh\:mm\:ss")
                        : null,

                p_venue =
                    string.IsNullOrWhiteSpace(
                        model.Venue)
                        ? null
                        : model.Venue.Trim(),

                p_target_type =
                    model.TargetType,

                p_target_role =
                    model.TargetType == "Role"
                        ? model.TargetRole
                        : null,

                p_target_grade =
                    model.TargetType == "Grade"
                        ? model.TargetGrade
                        : null,

                p_target_class_id =
                    model.TargetType == "Class"
                        ? model.TargetClassId
                        : null
            };


        await _supabase.Rpc(
            "update_school_event_with_notifications",
            parameters);
    }

    public async Task DeleteEventAsync(
    Guid eventId,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        if (eventId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A valid event is required.");
        }


        var parameters =
            new
            {
                p_event_id =
                    eventId,

                p_school_id =
                    schoolId
            };


        await _supabase.Rpc(
            "delete_school_event",
            parameters);
    }


    private static string BuildEventTargetDisplay(
        EventRecord schoolEvent,
        List<ClassRecord> classes)
    {
        return schoolEvent.TargetType switch
        {
            "Whole School" =>
                "Whole School",

            "Role" =>
                string.IsNullOrWhiteSpace(
                    schoolEvent.TargetRole)
                    ? "Role"
                    : $"Role: {schoolEvent.TargetRole}",

            "Grade" =>
                string.IsNullOrWhiteSpace(
                    schoolEvent.TargetGrade)
                    ? "Grade"
                    : schoolEvent.TargetGrade,

            "Class" =>
                BuildClassTarget(
                    schoolEvent.TargetClassId,
                    classes),

            _ =>
                schoolEvent.TargetType
        };
    }


    private static string BuildClassTarget(
        Guid? classId,
        List<ClassRecord> classes)
    {
        if (!classId.HasValue)
        {
            return "Class";
        }


        var schoolClass =
            classes.FirstOrDefault(x =>
                x.Id == classId.Value);


        if (schoolClass == null)
        {
            return "Class";
        }


        return string.IsNullOrWhiteSpace(
                schoolClass.Code)
            ? schoolClass.Name
            : $"{schoolClass.Name} ({schoolClass.Code})";
    }
}