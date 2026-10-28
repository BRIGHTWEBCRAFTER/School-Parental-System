
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SchoolParentalSystem.Web.ViewModels.Teacher;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Services.Teacher;

/* ==========================================
   TEACHER EVENTS SERVICE
   ========================================== */

public class TeacherEventsService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService _sessionService;

    private readonly TeacherAssignedClassesService _classesService;


    public TeacherEventsService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        TeacherAssignedClassesService classesService)
    {
        _supabase = supabase;

        _sessionService = sessionService;

        _classesService = classesService;
    }


    /* ==========================================
       LOAD TEACHER EVENTS
       ========================================== */

    public async Task<TeacherEventManagementViewModel>
        GetEventsAsync(
            Guid userId,
            Guid schoolId)
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
        // 2. LOAD ASSIGNED CLASSES
        // ==========================================

        var assignments = await _classesService
            .GetAssignedClassesAsync(
                userId,
                schoolId);


        var model = new TeacherEventManagementViewModel
        {
            HasTeacherProfile =
                assignments.HasTeacherProfile
        };


        if (!model.HasTeacherProfile)
        {
            return model;
        }


        // ==========================================
        // 3. BUILD AUTHORIZED CLASS LOOKUP
        // ==========================================

        var assignedClasses = assignments.Classes
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .ToDictionary(
                c => c.Id,
                c => c.Name);


        if (assignedClasses.Count == 0)
        {
            return model;
        }


        // ==========================================
        // 4. LOAD TEACHER-CREATED EVENTS
        // ==========================================

        // The authenticated Supabase session
        // applies database RLS.
        //
        // Only events created by this Teacher
        // are requested.

        var response = await _supabase
            .From<TeacherEventRecord>()
            .Where(e => e.CreatedBy == userId)
            .Get();


        // ==========================================
        // 5. VERIFY EVENT ACCESS
        // ==========================================

        // Keep events within:
        //
        // - The current school
        // - The current Teacher
        // - The Teacher's assigned classes
        // - Class or Specific Users target types
        //
        // These checks are additional to RLS.

        var permittedEvents = response.Models
            .Where(e =>
                e.SchoolId == schoolId &&
                e.CreatedBy == userId &&
                (
                    e.TargetType == "Class" ||
                    e.TargetType == "Specific Users"
                ) &&
                e.TargetClassId.HasValue &&
                assignedClasses.ContainsKey(
                    e.TargetClassId.Value))
            .ToList();


        // ==========================================
        // 6. MAP EVENTS TO VIEWMODEL
        // ==========================================

        model.Events = permittedEvents
            .Select(e => new TeacherEventItemViewModel
            {
                Id = e.Id,

                EventName = e.EventName,

                Description = e.Description ?? "",

                EventDate = e.EventDate,

                StartTime = e.StartTime,

                EndTime = e.EndTime,

                Venue = e.Venue ?? "",

                TargetType = e.TargetType,

                TargetClassId = e.TargetClassId,

                ClassName = assignedClasses[
                    e.TargetClassId!.Value]
            })
            .OrderBy(e =>
                e.EventDate.Date < DateTime.Today)
            .ThenBy(e => e.EventDate)
            .ThenBy(e => e.StartTime)
            .ToList();


        return model;
    }


    /* ==========================================
       LOAD ONE TEACHER EVENT
       ========================================== */

    public async Task<TeacherEventItemViewModel?>
        GetEventDetailsAsync(
            Guid userId,
            Guid schoolId,
            Guid eventId)
    {
        if (eventId == Guid.Empty)
        {
            return null;
        }


        // Reuse the authorized event list.
        // The Teacher cannot use an arbitrary
        // event ID to bypass class restrictions.

        var model = await GetEventsAsync(
            userId,
            schoolId);


        return model.Events
            .FirstOrDefault(e => e.Id == eventId);
    }

    /* ==========================================
       CREATE TEACHER EVENT
       CLASS OR SPECIFIC PARENT
       ========================================== */

    public async Task<Guid> CreateEventAsync(
        Guid userId,
        Guid schoolId,
        TeacherCreateEventViewModel request)
    {
        // 1. RESTORE SESSION

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // 2. VALIDATE EVENT DETAILS

        var eventName = (request.EventName ?? "").Trim();
        var description = (request.Description ?? "").Trim();
        var venue = (request.Venue ?? "").Trim();
        var targetType = (request.TargetType ?? "").Trim();

        if (eventName.Length is < 1 or > 150)
        {
            throw new ArgumentException(
                "Event name must be between 1 and 150 characters.");
        }

        if (description.Length > 500)
        {
            throw new ArgumentException(
                "Description cannot exceed 500 characters.");
        }

        if (venue.Length > 150)
        {
            throw new ArgumentException(
                "Venue cannot exceed 150 characters.");
        }

        if (!request.EventDate.HasValue ||
            request.EventDate.Value.Date < DateTime.Today)
        {
            throw new ArgumentException(
                "Select a valid current or future event date.");
        }

        if (request.StartTime.HasValue &&
            request.EndTime.HasValue &&
            request.EndTime.Value <= request.StartTime.Value)
        {
            throw new ArgumentException(
                "End time must be later than start time.");
        }

        // 3. VALIDATE TARGET TYPE

        if (targetType != "Class" &&
            targetType != "Specific Users")
        {
            throw new ArgumentException(
                "Select a valid target type.");
        }

        if (!request.TargetClassId.HasValue ||
            request.TargetClassId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Select an assigned class.");
        }

        // 4. VERIFY TEACHER ASSIGNMENT

        var assignments = await _classesService
            .GetAssignedClassesAsync(userId, schoolId);

        if (!assignments.HasTeacherProfile ||
            !assignments.Classes.Any(c =>
                c.Id == request.TargetClassId.Value))
        {
            throw new UnauthorizedAccessException(
                "You are not assigned to this class.");
        }

        // 5. VALIDATE SPECIFIC PARENT

        if (targetType == "Specific Users" &&
            (!request.ParentUserId.HasValue ||
             request.ParentUserId.Value == Guid.Empty))
        {
            throw new ArgumentException(
                "Select one eligible parent.");
        }

        // 6. PREPARE DATABASE PARAMETERS
        //
        // The dedicated RPC must independently
        // verify the Teacher, class and parent.
        //
        // It must create the event and recipient
        // notifications in one transaction.

        var parameters = new Dictionary<string, object>
        {
            ["p_class_id"] = request.TargetClassId.Value,
            ["p_target_type"] = targetType,
            ["p_event_name"] = eventName,
            ["p_description"] = description,
            ["p_event_date"] =
                request.EventDate.Value.ToString("yyyy-MM-dd"),
            ["p_venue"] = venue
        };

        if (request.StartTime.HasValue)
        {
            parameters["p_start_time"] =
                request.StartTime.Value.ToString(@"hh\:mm\:ss");
        }

        if (request.EndTime.HasValue)
        {
            parameters["p_end_time"] =
                request.EndTime.Value.ToString(@"hh\:mm\:ss");
        }

        if (targetType == "Specific Users")
        {
            parameters["p_parent_user_id"] =
                request.ParentUserId!.Value;
        }

        // 7. CREATE EVENT
        //
        // This RPC will be created in the
        // next database step.
        //
        // Do not replace it with the School Admin
        // event-creation RPC; that function has
        // different authorization rules.

        var created = await _supabase.Rpc(
            "create_teacher_event_with_notifications",
            parameters);

        return Guid.Parse((created.Content ?? "").Trim().Trim('"'));
    }


    /* ==========================================
       LOAD TEACHER EVENT FOR EDITING
       ========================================== */

    public async Task<TeacherEditEventViewModel?>
        GetEventForEditingAsync(
            Guid userId,
            Guid schoolId,
            Guid eventId)
    {
        if (eventId == Guid.Empty)
        {
            return null;
        }

        // Reuse existing authorization checks.

        var schoolEvent = await GetEventDetailsAsync(
            userId,
            schoolId,
            eventId);

        if (schoolEvent == null)
        {
            return null;
        }

        var model = new TeacherEditEventViewModel
        {
            Id = schoolEvent.Id,

            EventName = schoolEvent.EventName,

            Description = schoolEvent.Description,

            EventDate = schoolEvent.EventDate,

            StartTime = schoolEvent.StartTime,

            EndTime = schoolEvent.EndTime,

            Venue = schoolEvent.Venue,

            TargetType = schoolEvent.TargetType,

            TargetClassId = schoolEvent.TargetClassId
        };

        // Specific Users events store their recipient
        // in notifications, not directly in events.
        //
        // This RPC verifies Teacher ownership
        // before returning the selected parent.

        if (schoolEvent.TargetType == "Specific Users")
        {
            var response = await _supabase.Rpc(
                "get_teacher_event_selected_parent",
                new Dictionary<string, object>
                {
                    ["p_event_id"] = eventId
                });

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                throw new InvalidOperationException(
                    "Unable to retrieve the event recipient.");
            }

            using var json =
                System.Text.Json.JsonDocument.Parse(
                    response.Content);

            if (json.RootElement.ValueKind !=
                    System.Text.Json.JsonValueKind.String ||
                !Guid.TryParse(
                    json.RootElement.GetString(),
                    out var parentId))
            {
                throw new InvalidOperationException(
                    "The selected parent could not be determined.");
            }

            model.ParentUserId = parentId;
        }

        return model;
    }


    /* ==========================================
       UPDATE TEACHER EVENT
       ========================================== */

    public async Task UpdateEventAsync(
        Guid userId,
        Guid schoolId,
        TeacherEditEventViewModel request)
    {
        // ==========================================
        // 1. RESTORE SESSION
        // ==========================================

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (request == null)
        {
            throw new ArgumentNullException(
                nameof(request));
        }

        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Invalid event ID.");
        }

        // ==========================================
        // 2. CHECK EVENT OWNERSHIP
        // ==========================================

        var existingEvent = await GetEventDetailsAsync(
            userId,
            schoolId,
            request.Id);

        if (existingEvent == null)
        {
            throw new UnauthorizedAccessException(
                "You cannot edit this event.");
        }

        // ==========================================
        // 3. VALIDATE EVENT DETAILS
        // ==========================================

        var eventName =
            (request.EventName ?? "").Trim();

        var description =
            (request.Description ?? "").Trim();

        var venue =
            (request.Venue ?? "").Trim();

        var targetType =
            (request.TargetType ?? "").Trim();

        if (eventName.Length is < 1 or > 150)
        {
            throw new ArgumentException(
                "Event name must contain between " +
                "1 and 150 characters.");
        }

        if (description.Length > 500)
        {
            throw new ArgumentException(
                "Description cannot exceed 500 characters.");
        }

        if (venue.Length > 150)
        {
            throw new ArgumentException(
                "Venue cannot exceed 150 characters.");
        }

        if (!request.EventDate.HasValue)
        {
            throw new ArgumentException(
                "Event date is required.");
        }

        if (request.StartTime.HasValue &&
            request.EndTime.HasValue &&
            request.EndTime.Value <= request.StartTime.Value)
        {
            throw new ArgumentException(
                "End time must be later than start time.");
        }

        // ==========================================
        // 4. VALIDATE TARGET AUDIENCE
        // ==========================================

        if (targetType != "Class" &&
            targetType != "Specific Users")
        {
            throw new ArgumentException(
                "Invalid target type.");
        }

        if (!request.TargetClassId.HasValue ||
            request.TargetClassId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Select an assigned class.");
        }

        // ==========================================
        // 5. VERIFY ASSIGNED CLASS
        // ==========================================

        var assignments = await _classesService
            .GetAssignedClassesAsync(
                userId,
                schoolId);

        if (!assignments.HasTeacherProfile ||
            !assignments.Classes.Any(c =>
                c.Id == request.TargetClassId.Value))
        {
            throw new UnauthorizedAccessException(
                "This class is not assigned to you.");
        }

        // ==========================================
        // 6. VALIDATE SPECIFIC PARENT
        // ==========================================

        if (targetType == "Specific Users" &&
            (!request.ParentUserId.HasValue ||
             request.ParentUserId.Value == Guid.Empty))
        {
            throw new ArgumentException(
                "Select one eligible parent.");
        }

        // ==========================================
        // 7. PREPARE SUPABASE RPC PARAMETERS
        // ==========================================

        var parameters = new Dictionary<string, object>
        {
            ["p_event_id"] = request.Id,

            ["p_class_id"] =
                request.TargetClassId.Value,

            ["p_target_type"] = targetType,

            ["p_event_name"] = eventName,

            ["p_description"] = description,

            ["p_event_date"] =
                request.EventDate.Value
                    .ToString("yyyy-MM-dd"),

            ["p_venue"] = venue
        };

        if (request.StartTime.HasValue)
        {
            parameters["p_start_time"] =
                request.StartTime.Value
                    .ToString(@"hh\:mm\:ss");
        }

        if (request.EndTime.HasValue)
        {
            parameters["p_end_time"] =
                request.EndTime.Value
                    .ToString(@"hh\:mm\:ss");
        }

        if (targetType == "Specific Users")
        {
            parameters["p_parent_user_id"] =
                request.ParentUserId!.Value;
        }

        // ==========================================
        // 8. UPDATE EVENT IN SUPABASE
        // ==========================================

        // Database independently verifies:
        //
        // - Authenticated active Teacher
        // - Event ownership
        // - Existing assigned class
        // - New assigned class
        // - Eligible parent
        //
        // Event and recipient changes are atomic.

        await _supabase.Rpc(
            "update_teacher_event_with_notifications",
            parameters);
    }


    /* ==========================================
       DELETE TEACHER EVENT
       ========================================== */

    public async Task DeleteEventAsync(
        Guid userId,
        Guid schoolId,
        Guid eventId)
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
        // 2. VALIDATE EVENT ID
        // ==========================================

        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Invalid event ID.");
        }

        // ==========================================
        // 3. VERIFY TEACHER ACCESS
        // ==========================================

        var schoolEvent = await GetEventDetailsAsync(
            userId,
            schoolId,
            eventId);

        if (schoolEvent == null)
        {
            throw new UnauthorizedAccessException(
                "You cannot delete this event.");
        }

        // ==========================================
        // 4. DELETE EVENT THROUGH SUPABASE
        // ==========================================

        // The Supabase function independently checks:
        //
        // - Active authenticated Teacher
        // - Event ownership
        // - School membership
        // - Original assigned class
        //
        // Related event notifications are removed
        // automatically by ON DELETE CASCADE.
        //
        // Unauthorized and missing events are rejected.

        await _supabase.Rpc(
            "delete_teacher_event",
            new Dictionary<string, object>
            {
                ["p_event_id"] = eventId
            });
    }


}


/* ==========================================
   SUPABASE EVENTS TABLE MAPPING
   ========================================== */

[Table("events")]
public class TeacherEventRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }


    [Column("school_id")]
    public Guid SchoolId { get; set; }


    [Column("created_by")]
    public Guid CreatedBy { get; set; }


    [Column("event_name")]
    public string EventName { get; set; } = "";


    [Column("description")]
    public string? Description { get; set; }


    [Column("event_date")]
    public DateTime EventDate { get; set; }


    [Column("start_time")]
    public TimeSpan? StartTime { get; set; }


    [Column("end_time")]
    public TimeSpan? EndTime { get; set; }


    [Column("venue")]
    public string? Venue { get; set; }


    [Column("target_type")]
    public string TargetType { get; set; } = "";


    [Column("target_class_id")]
    public Guid? TargetClassId { get; set; }


    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
