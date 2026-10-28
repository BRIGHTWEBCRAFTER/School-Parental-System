
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels.Teacher;

namespace SchoolParentalSystem.Web.Services.Teacher;

//
// Class Name       : TeacherDashboardService
// Purpose          : Encapsulates teacher dashboard operations for the
//                    School Parental System.
// Re-use           : Used by Teacher controllers and dashboard actions.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class TeacherDashboardService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService _sessionService;

    private readonly TeacherAssignedClassesService
        _classesService;

    private readonly ILogger<TeacherDashboardService>
        _logger;


    //
    // Method Name      : TeacherDashboardService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService; TeacherAssignedClassesService
    //                    classesService; ILogger<TeacherDashboardService>
    //                    logger
    // Output Type      : Constructor.
    //

    public TeacherDashboardService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        TeacherAssignedClassesService classesService,
        ILogger<TeacherDashboardService> logger)
    {
        _supabase = supabase;
        _sessionService = sessionService;
        _classesService = classesService;
        _logger = logger;
    }


    // ==========================================
    // LOAD DASHBOARD
    // ==========================================

    //
    // Method Name      : GetDashboardAsync
    // Purpose          : Retrieves dashboard.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<TeacherDashboardViewModel>.
    //

    public async Task<TeacherDashboardViewModel>
        GetDashboardAsync(Guid userId, Guid schoolId)
    {
        if (userId == Guid.Empty ||
            schoolId == Guid.Empty ||
            !await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }


        // ======================================
        // GET AUTHORIZED CLASSES
        // ======================================

        var assigned = await _classesService
            .GetAssignedClassesAsync(userId, schoolId);


        var model = new TeacherDashboardViewModel
        {
            HasTeacherProfile =
                assigned.HasTeacherProfile,

            AssignedClassCount =
                assigned.HomeroomClassCount,

            ClassesTaughtCount =
                assigned.ClassesTaughtCount,

            AssignedClassSummary =
                assigned.AssignedClass?.ClassName
                ?? "No class-teacher assignment yet",

            ClassesTaughtSummary =
                assigned.ClassesTaughtCount == 1
                    ? "Active teaching class"
                    : "Active teaching classes"
        };


        if (!assigned.HasTeacherProfile)
        {
            return model;
        }


        // CLASSES ACCESSIBLE TO THIS TEACHER

        var allowedClassIds = assigned.Classes
            .Select(c => c.Id)
            .ToHashSet();


        var namesById = assigned.Classes
            .GroupBy(c => c.Id)
            .ToDictionary(
                g => g.Key,
                g => g.First().Name);



        // ==========================================
        // TEACHING CLASSES WITH LIVE TIMETABLE
        // ==========================================

        model.TeachingClasses = assigned.ClassesITeach
            .OrderBy(c => c.ClassName)
            .ThenBy(c => c.SubjectName)
            .Take(6)
            .Select(c => new TeacherDashboardClassItem
            {
                ClassId = c.ClassId,

                ClassName = c.ClassName,

                SubjectName = c.SubjectName,

                RoomNumber = c.RoomNumber,

                // Populated by TeacherAssignedClassesService
                Schedule = c.Schedule
            })
            .ToList();


        var today = DateTime.Today;


        // ======================================
        // TODAY'S ATTENDANCE
        // ======================================

        try
        {
            var response = await _supabase
                .From<AttendanceRecord>()
                .Where(a => a.AttendanceDate == today)
                .Get();


            // Count only assigned-class records.
            // These may include daily and
            // subject attendance entries.

            var records = response.Models
                .Where(a =>
                    allowedClassIds.Contains(a.ClassId) &&
                    a.AttendanceDate.Date == today)
                .ToList();


            model.AttendanceTodayCount =
                records.Count;


            model.PresentTodayCount =
                records.Count(a =>
                    IsStatus(a.Status, "Present") ||
                    IsStatus(a.Status, "Late"));


            model.AbsentTodayCount =
                records.Count(a =>
                    IsStatus(a.Status, "Absent"));


            // BUILD CLASS ATTENDANCE SUMMARIES

            model.AttendanceByClass = namesById
                .OrderBy(p => p.Value)
                .Take(5)
                .Select(pair =>
                {
                    var rows = records
                        .Where(a => a.ClassId == pair.Key)
                        .ToList();


                    return new TeacherDashboardAttendanceItem
                    {
                        ClassId = pair.Key,

                        ClassName = pair.Value,

                        Total = rows.Count,

                        Present = rows.Count(a =>
                            IsStatus(a.Status, "Present")),

                        Late = rows.Count(a =>
                            IsStatus(a.Status, "Late")),

                        Absent = rows.Count(a =>
                            IsStatus(a.Status, "Absent")),

                        Excused = rows.Count(a =>
                            IsStatus(a.Status, "Excused"))
                    };
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not load Teacher dashboard attendance.");

            model.AttendanceLoadFailed = true;
        }


        // ======================================
        // ACTIVE ANNOUNCEMENTS
        // ======================================

        try
        {
            var response = await _supabase
                .From<TeacherClassAnnouncementRecord>()
                .Where(a => a.CreatedBy == userId)
                .Get();


            var active = response.Models
                .Where(a =>
                    a.SchoolId == schoolId &&
                    a.CreatedBy == userId &&
                    a.TargetClassId.HasValue &&
                    allowedClassIds.Contains(
                        a.TargetClassId.Value) &&
                    (
                        a.TargetType == "Class" ||
                        a.TargetType == "Specific Users"
                    ) &&
                    (
                        !a.ExpiresAt.HasValue ||
                        a.ExpiresAt.Value > DateTime.UtcNow
                    ))
                .OrderByDescending(a => a.CreatedAt)
                .ToList();


            model.ActiveAnnouncementsCount =
                active.Count;


            model.RecentAnnouncements = active
                .Take(4)
                .Select(a =>
                    new TeacherDashboardAnnouncementItem
                    {
                        Title = a.Title,

                        Priority = a.Priority,

                        ClassName = namesById[
                            a.TargetClassId!.Value],

                        CreatedAt = a.CreatedAt
                    })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not load Teacher dashboard announcements.");

            model.AnnouncementsLoadFailed = true;
        }


        // ======================================
        // UPCOMING EVENTS
        // ======================================

        try
        {
            var response = await _supabase
                .From<TeacherEventRecord>()
                .Where(e => e.CreatedBy == userId)
                .Get();


            model.UpcomingEvents = response.Models
                .Where(e =>
                    e.SchoolId == schoolId &&
                    e.CreatedBy == userId &&
                    e.EventDate.Date >= today &&
                    e.TargetClassId.HasValue &&
                    allowedClassIds.Contains(
                        e.TargetClassId.Value) &&
                    (
                        e.TargetType == "Class" ||
                        e.TargetType == "Specific Users"
                    ))
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.StartTime)
                .Take(4)
                .Select(e => new TeacherDashboardEventItem
                {
                    EventName = e.EventName,

                    ClassName = namesById[
                        e.TargetClassId!.Value],

                    Venue = e.Venue ??
                        "Venue to be confirmed",

                    EventDate = e.EventDate,

                    StartTime = e.StartTime
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not load Teacher dashboard events.");

            model.EventsLoadFailed = true;
        }


        return model;
    }


    //
    // Method Name      : IsStatus
    // Purpose          : Checks whether status.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : string? value; string expected
    // Output Type      : bool.
    //

    private static bool IsStatus(
        string? value,
        string expected)
    {
        return string.Equals(
            value,
            expected,
            StringComparison.OrdinalIgnoreCase);
    }
}
