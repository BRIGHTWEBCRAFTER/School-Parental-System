
using SchoolParentalSystem.Web.ViewModels.Teacher;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolParentalSystem.Web.Services.Teacher;

public class TeacherCommunicationService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;
    private readonly TeacherAssignedClassesService _classesService;

    public TeacherCommunicationService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        TeacherAssignedClassesService classesService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
        _classesService = classesService;
    }

    /* ==========================================
       TEACHER COMMUNICATION MANAGEMENT
       ========================================== */

    public async Task<TeacherCommunicationViewModel>
        GetCommunicationAsync(
            Guid userId,
            Guid schoolId,
            string? searchTerm = null,
            string? priority = null,
            string? targetType = null,
            string? status = null)
    {
        // RESTORE SESSION

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        // LOAD ASSIGNED CLASSES

        var assignments = await _classesService
            .GetAssignedClassesAsync(userId, schoolId);

        var model = new TeacherCommunicationViewModel
        {
            HasTeacherProfile = assignments.HasTeacherProfile
        };

        if (!model.HasTeacherProfile)
        {
            return model;
        }

        model.Classes = assignments.Classes
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .Select(c =>
                new TeacherCommunicationClassOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
            .OrderBy(c => c.Name)
            .ToList();

        var assignedClassIds = model.Classes
            .Select(c => c.Id)
            .ToHashSet();

        var classNames = model.Classes
            .ToDictionary(c => c.Id, c => c.Name);

        // LOAD ELIGIBLE PARENTS
        //
        // The RPC restricts results to parents
        // of learners in assigned classes.

        var parentsResponse = await _supabase.Rpc(
            "list_teacher_contactable_parents",
            new Dictionary<string, object>());

        model.Parents = JsonSerializer
            .Deserialize<List<TeacherEligibleParentViewModel>>(
                parentsResponse.Content ?? "[]")
            ?.Where(p =>
                assignedClassIds.Contains(p.ClassId))
            .OrderBy(p => p.ParentName)
            .ThenBy(p => p.LearnerNames)
            .ToList() ?? new();

        // LOAD ANNOUNCEMENTS
        //
        // Only announcements created by this
        // Teacher are displayed.
        //
        // Both Class and Specific Users are allowed.

        var announcementResponse = await _supabase
            .From<TeacherClassAnnouncementRecord>()
            .Where(a => a.CreatedBy == userId)
            .Get();

        var teacherAnnouncements =
            announcementResponse.Models
                .Where(a =>
                    a.SchoolId == schoolId &&
                    (
                        a.TargetType == "Class" ||
                        a.TargetType == "Specific Users"
                    ) &&
                    a.TargetClassId.HasValue &&
                    assignedClassIds.Contains(
                        a.TargetClassId.Value))
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

        // BUILD ANNOUNCEMENT LIST

        model.Announcements = teacherAnnouncements
            .Select(a =>
                new TeacherCommunicationAnnouncementViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Priority = a.Priority,

                    TargetType = a.TargetType,

                    ClassName = classNames[
                        a.TargetClassId!.Value],

                    TargetClassId =
                        a.TargetClassId.Value,

                    CreatedAt = a.CreatedAt,
                    ExpiresAt = a.ExpiresAt,

                    AcknowledgementRequired =
                        a.AcknowledgementRequired
                })
            .ToList();

        // LOAD RECIPIENT ENGAGEMENT
        //
        // Read and acknowledgement information
        // is summarized for announcements
        // created by this Teacher.

        if (model.Announcements.Count > 0)
        {
            var announcementIds = model.Announcements
                .Select(a => a.Id)
                .ToHashSet();

            var receiptResponse = await _supabase
                .From<TeacherAnnouncementReceiptRecord>()
                .Get();

            var receiptGroups = receiptResponse.Models
                .Where(r =>
                    announcementIds.Contains(
                        r.AnnouncementId))
                .GroupBy(r => r.AnnouncementId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            foreach (var announcement in model.Announcements)
            {
                if (!receiptGroups.TryGetValue(
                    announcement.Id,
                    out var receipts))
                {
                    continue;
                }

                announcement.RecipientCount =
                    receipts.Count;

                announcement.ReadCount =
                    receipts.Count(r => r.IsRead);

                announcement.AcknowledgedCount =
                    receipts.Count(r => r.IsAcknowledged);
            }
        }

        // VALIDATE FILTER VALUES

        var priorities = new[]
        {
            "Normal",
            "Important",
            "Urgent"
        };

        var targets = new[]
        {
            "Class",
            "Specific Users"
        };

        var statuses = new[]
        {
            "Active",
            "Expired"
        };

        priority = priorities.FirstOrDefault(x =>
            string.Equals(
                x,
                priority,
                StringComparison.OrdinalIgnoreCase));

        targetType = targets.FirstOrDefault(x =>
            string.Equals(
                x,
                targetType,
                StringComparison.OrdinalIgnoreCase));

        status = statuses.FirstOrDefault(x =>
            string.Equals(
                x,
                status,
                StringComparison.OrdinalIgnoreCase));

        searchTerm = searchTerm?.Trim();

        // APPLY SEARCH FILTER

        IEnumerable<TeacherCommunicationAnnouncementViewModel>
            filteredAnnouncements = model.Announcements;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredAnnouncements = filteredAnnouncements
                .Where(a =>
                    a.Title.Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase) ||
                    a.Content.Contains(
                        searchTerm,
                        StringComparison.OrdinalIgnoreCase));
        }

        // APPLY PRIORITY FILTER

        if (!string.IsNullOrWhiteSpace(priority))
        {
            filteredAnnouncements = filteredAnnouncements
                .Where(a =>
                    string.Equals(
                        a.Priority,
                        priority,
                        StringComparison.OrdinalIgnoreCase));
        }

        // APPLY TARGET FILTER

        if (!string.IsNullOrWhiteSpace(targetType))
        {
            filteredAnnouncements = filteredAnnouncements
                .Where(a =>
                    string.Equals(
                        a.TargetType,
                        targetType,
                        StringComparison.OrdinalIgnoreCase));
        }

        // APPLY STATUS FILTER

        if (!string.IsNullOrWhiteSpace(status))
        {
            filteredAnnouncements = filteredAnnouncements
                .Where(a =>
                    string.Equals(
                        a.Status,
                        status,
                        StringComparison.OrdinalIgnoreCase));
        }

        // RETURN FILTERED ANNOUNCEMENTS

        model.Announcements = filteredAnnouncements
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        model.SearchTerm = searchTerm ?? "";
        model.SelectedPriority = priority ?? "";
        model.SelectedTarget = targetType ?? "";
        model.SelectedStatus = status ?? "";

        return model;
    }

    /* ==========================================
       SEND TEACHER ANNOUNCEMENT
       CLASS OR SPECIFIC PARENT
       ========================================== */

    public async Task<Guid> SendClassAnnouncementAsync(
        Guid userId,
        Guid schoolId,
        TeacherCreateClassAnnouncementViewModel request)
    {
        // RESTORE SESSION

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        // VALIDATE CLASS

        if (!request.ClassId.HasValue ||
            request.ClassId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Select a valid class.");
        }

        // VALIDATE DETAILS

        var title = (request.Title ?? "").Trim();
        var content = (request.Content ?? "").Trim();
        var priority = (request.Priority ?? "").Trim();
        var targetType = (request.TargetType ?? "").Trim();

        if (title.Length is < 1 or > 150 ||
            content.Length is < 1 or > 5000)
        {
            throw new ArgumentException(
                "Provide a title and message within " +
                "the allowed limits.");
        }

        if (priority != "Normal" &&
            priority != "Important" &&
            priority != "Urgent")
        {
            throw new ArgumentException(
                "Select a valid priority.");
        }

        if (request.AcknowledgementRequired &&
            priority == "Normal")
        {
            throw new ArgumentException(
                "Acknowledgements require Important " +
                "or Urgent priority.");
        }

        if (targetType != "Class" &&
            targetType != "Specific Users")
        {
            throw new ArgumentException(
                "Select a valid target type.");
        }

        // VALIDATE TEACHER ASSIGNMENT

        var assignments = await _classesService
            .GetAssignedClassesAsync(userId, schoolId);

        if (!assignments.HasTeacherProfile ||
            !assignments.Classes.Any(
                c => c.Id == request.ClassId.Value))
        {
            throw new UnauthorizedAccessException(
                "You are not assigned to this class.");
        }

        // SEND TO ONE SPECIFIC PARENT

        if (targetType == "Specific Users")
        {
            if (!request.ParentUserId.HasValue ||
                request.ParentUserId.Value == Guid.Empty)
            {
                throw new ArgumentException(
                    "Please select a parent.");
            }

            // The RPC independently checks the
            // Teacher's role, assigned class and
            // the parent's active learner link.

            var created = await _supabase.Rpc(
                "create_teacher_specific_parent_announcement",
                new Dictionary<string, object>
                {
                    ["p_class_id"] =
                        request.ClassId.Value,

                    ["p_parent_user_id"] =
                        request.ParentUserId.Value,

                    ["p_title"] = title,

                    ["p_content"] = content,

                    ["p_priority"] = priority,

                    ["p_acknowledgement_required"] =
                        request.AcknowledgementRequired
                });

            return Guid.Parse((created.Content ?? "").Trim().Trim('"'));
        }

        // SEND TO ALL ELIGIBLE PARENTS IN CLASS

        else
        {
            var created = await _supabase.Rpc(
                "create_teacher_class_announcement",
                new Dictionary<string, object>
                {
                    ["p_class_id"] =
                        request.ClassId.Value,

                    ["p_title"] = title,

                    ["p_content"] = content,

                    ["p_priority"] = priority,

                    ["p_acknowledgement_required"] =
                        request.AcknowledgementRequired
                });

            return Guid.Parse((created.Content ?? "").Trim().Trim('"'));
        }
    }


    /* ==========================================
       LOAD ANNOUNCEMENT FOR EDITING
       ========================================== */

    public async Task<TeacherEditAnnouncementViewModel?>
        GetAnnouncementForEditingAsync(
            Guid userId,
            Guid schoolId,
            Guid announcementId)
    {
        if (announcementId == Guid.Empty)
        {
            return null;
        }

        // Existing service verifies the session,
        // Teacher profile and assigned classes.

        var data = await GetCommunicationAsync(
            userId,
            schoolId);

        if (data.DataLoadFailed)
        {
            throw new InvalidOperationException(
                "Unable to load Teacher communication.");
        }

        if (!data.HasTeacherProfile)
        {
            throw new UnauthorizedAccessException(
                "Active Teacher profile required.");
        }

        // Only Teacher-created, authorized announcements
        // are returned by GetCommunicationAsync.


        var announcement = data.Announcements
            .FirstOrDefault(a => a.Id == announcementId);

        if (announcement == null ||
            announcement.TargetClassId == Guid.Empty ||
            !data.Classes.Any(c =>
                c.Id == announcement.TargetClassId))
        {
            return null;
        }


        var model = new TeacherEditAnnouncementViewModel
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            Priority = announcement.Priority,
            ExpiresAt = announcement.ExpiresAt,
            TargetType = announcement.TargetType,
            TargetClassId = announcement.TargetClassId,
            AcknowledgementRequired =
                announcement.AcknowledgementRequired,

            Classes = data.Classes,
            Parents = data.Parents
        };

        // Specific Users announcements store the
        // selected parent in announcement_recipients.
        // The RPC independently verifies ownership
        // and assigned-class access.

        if (announcement.TargetType == "Specific Users")
        {
            var response = await _supabase.Rpc(
                "get_teacher_announcement_selected_parent",
                new Dictionary<string, object>
                {
                    ["p_announcement_id"] = announcementId
                });

            using var json =
                System.Text.Json.JsonDocument.Parse(
                    response.Content ?? "null");

            if (json.RootElement.ValueKind !=
                    System.Text.Json.JsonValueKind.String ||
                !Guid.TryParse(
                    json.RootElement.GetString(),
                    out var parentId))
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the selected parent.");
            }

            model.ParentUserId = parentId;
        }

        return model;
    }


    /* ==========================================
       UPDATE TEACHER ANNOUNCEMENT
       ========================================== */

    public async Task UpdateAnnouncementAsync(
        Guid userId,
        Guid schoolId,
        TeacherEditAnnouncementViewModel request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }

        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Invalid announcement ID.");
        }

        // ==========================================
        // 1. VALIDATE ANNOUNCEMENT
        // ==========================================

        var title = (request.Title ?? "").Trim();
        var content = (request.Content ?? "").Trim();
        var priority = (request.Priority ?? "").Trim();
        var targetType = (request.TargetType ?? "").Trim();

        if (title.Length is < 1 or > 150)
        {
            throw new ArgumentException(
                "Title must be between 1 and 150 characters.");
        }

        if (content.Length is < 1 or > 5000)
        {
            throw new ArgumentException(
                "Message must be between 1 and 5000 characters.");
        }

        if (priority != "Normal" &&
            priority != "Important" &&
            priority != "Urgent")
        {
            throw new ArgumentException(
                "Invalid announcement priority.");
        }

        if (request.AcknowledgementRequired &&
            priority == "Normal")
        {
            throw new ArgumentException(
                "Only Important or Urgent announcements " +
                "can require acknowledgement.");
        }

        if (targetType != "Class" &&
            targetType != "Specific Users")
        {
            throw new ArgumentException(
                "Invalid announcement target type.");
        }

        if (!request.TargetClassId.HasValue ||
            request.TargetClassId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Select an assigned class.");
        }

        // ==========================================
        // 2. VALIDATE EXPIRY
        // ==========================================

        if (request.ExpiresAt.HasValue &&
            request.ExpiresAt.Value <= DateTime.Now)
        {
            throw new ArgumentException(
                "Expiry date must be in the future.");
        }

        // ==========================================
        // 3. VALIDATE SPECIFIC PARENT
        // ==========================================

        if (targetType == "Specific Users" &&
            (!request.ParentUserId.HasValue ||
             request.ParentUserId.Value == Guid.Empty))
        {
            throw new ArgumentException(
                "Select one eligible parent.");
        }

        if (targetType == "Class" &&
            request.ParentUserId.HasValue)
        {
            throw new ArgumentException(
                "A Class announcement cannot target " +
                "an individual parent.");
        }

        // ==========================================
        // 4. VERIFY TEACHER ACCESS
        // ==========================================

        var data = await GetCommunicationAsync(
            userId,
            schoolId);

        if (data.DataLoadFailed)
        {
            throw new InvalidOperationException(
                "Unable to verify Teacher communication.");
        }

        if (!data.HasTeacherProfile)
        {
            throw new UnauthorizedAccessException(
                "Active Teacher profile required.");
        }

        var existingAnnouncement = data.Announcements
            .FirstOrDefault(a => a.Id == request.Id);

        if (existingAnnouncement == null)
        {
            throw new UnauthorizedAccessException(
                "You cannot edit this announcement.");
        }

        if (!data.Classes.Any(c =>
            c.Id == request.TargetClassId.Value))
        {
            throw new UnauthorizedAccessException(
                "This class is not assigned to you.");
        }

        // ==========================================
        // 5. PREPARE SUPABASE PARAMETERS
        // ==========================================

        var parameters = new Dictionary<string, object>
        {
            ["p_announcement_id"] = request.Id,
            ["p_class_id"] = request.TargetClassId.Value,
            ["p_target_type"] = targetType,
            ["p_title"] = title,
            ["p_content"] = content,
            ["p_priority"] = priority,
            ["p_acknowledgement_required"] =
                request.AcknowledgementRequired
        };

        // The form's datetime-local value represents
        // South African local time (UTC+02:00).
        // Include the offset explicitly when sending it
        // to a timestamptz database parameter.
        //
        // Send null explicitly if the Teacher clears
        // the expiry date.

        parameters["p_expires_at"] =
            request.ExpiresAt.HasValue
                ? new DateTimeOffset(
                    DateTime.SpecifyKind(
                        request.ExpiresAt.Value,
                        DateTimeKind.Unspecified),
                    TimeSpan.FromHours(2)
                  ).ToString("o")
                : null!;

        if (targetType == "Specific Users")
        {
            parameters["p_parent_user_id"] =
                request.ParentUserId!.Value;
        }

        // ==========================================
        // 6. UPDATE IN SUPABASE
        // ==========================================

        // The database independently verifies:
        //
        // - Authenticated active Teacher
        // - Original announcement ownership
        // - Original assigned class
        // - Newly selected assigned class
        // - Eligible parent linkage
        //
        // Existing recipients retain read and
        // acknowledgement history.
        //
        // New recipients receive notifications.
        // Removed recipients lose their access.
        //
        // School Admin notifications are preserved.

        await _supabase.Rpc(
            "update_teacher_announcement_with_recipients",
            parameters);
    }


    /* ==========================================
       DELETE TEACHER ANNOUNCEMENT
       ========================================== */

    public async Task DeleteAnnouncementAsync(
        Guid userId,
        Guid schoolId,
        Guid announcementId)
    {
        // ==========================================
        // 1. RESTORE SESSION
        // ==========================================

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        // ==========================================
        // 2. VALIDATE ANNOUNCEMENT ID
        // ==========================================

        if (announcementId == Guid.Empty)
        {
            throw new ArgumentException(
                "Invalid announcement ID.");
        }

        // ==========================================
        // 3. VERIFY TEACHER ACCESS
        // ==========================================

        var communication = await GetCommunicationAsync(
            userId,
            schoolId);

        if (communication.DataLoadFailed)
        {
            throw new InvalidOperationException(
                "Unable to verify Teacher communication.");
        }

        if (!communication.HasTeacherProfile)
        {
            throw new UnauthorizedAccessException(
                "Active Teacher profile required.");
        }

        // Only announcements created by the Teacher
        // for assigned classes are included.

        var announcement = communication.Announcements
            .FirstOrDefault(a => a.Id == announcementId);

        if (announcement == null)
        {
            throw new UnauthorizedAccessException(
                "You cannot delete this announcement.");
        }

        // ==========================================
        // 4. DELETE THROUGH SUPABASE
        // ==========================================

        // The database independently checks:
        //
        // - Active authenticated Teacher
        // - Announcement ownership
        // - School membership
        // - Assigned class permissions
        //
        // Related recipient records and notifications
        // are removed using database cascade rules.

        await _supabase.Rpc(
            "delete_teacher_announcement",
            new Dictionary<string, object>
            {
                ["p_announcement_id"] = announcementId
            });
    }


}

/* ==========================================
   ANNOUNCEMENT DATABASE MODEL
   ========================================== */

[Table("announcements")]
public class TeacherClassAnnouncementRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("content")]
    public string Content { get; set; } = "";

    [Column("priority")]
    public string Priority { get; set; } = "Normal";

    [Column("target_type")]
    public string TargetType { get; set; } = "";

    [Column("target_class_id")]
    public Guid? TargetClassId { get; set; }

    [Column("acknowledgement_required")]
    public bool AcknowledgementRequired { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }
}

/* ==========================================
   RECIPIENT DATABASE MODEL
   ========================================== */

// Only aggregate receipt counts are returned
// to the Teacher's Communication dashboard.

[Table("announcement_recipients")]
public class TeacherAnnouncementReceiptRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("announcement_id")]
    public Guid AnnouncementId { get; set; }

    [Column("is_read")]
    public bool IsRead { get; set; }

    [Column("is_acknowledged")]
    public bool IsAcknowledged { get; set; }
}
