
using System;
using System.Linq;
using System.Threading.Tasks;

using SchoolParentalSystem.Web.Models.Teacher;
using SchoolParentalSystem.Web.ViewModels.Teacher;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Services.Teacher;

//
// Class Name       : TeacherAssignedClassesService
// Purpose          : Encapsulates teacher assigned classes operations for
//                    the School Parental System.
// Re-use           : Used by Teacher controllers and dashboard actions.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class TeacherAssignedClassesService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService _sessionService;

    //
    // Method Name      : TeacherAssignedClassesService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public TeacherAssignedClassesService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }

    //
    // Method Name      : GetAssignedClassesAsync
    // Purpose          : Retrieves assigned Classes.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<TeacherAssignedClassesViewModel>.
    //

    public async Task<TeacherAssignedClassesViewModel>
        GetAssignedClassesAsync(
            Guid userId,
            Guid schoolId)
    {
        // ======================================
        // RESTORE AUTHENTICATED SESSION
        // ======================================

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        var model =
            new TeacherAssignedClassesViewModel();

        // ======================================
        // GET LOGGED-IN TEACHER
        // ======================================

        var teachersResult = await _supabase
            .From<TeacherDashboardTeacherRecord>()
            .Where(t => t.UserId == userId)
            .Get();

        var teacher = teachersResult.Models
            .FirstOrDefault(t =>
                t.SchoolId == schoolId &&
                t.IsActive);

        if (teacher == null)
        {
            return model;
        }

        model.HasTeacherProfile = true;

        // ======================================
        // GET VISIBLE CLASSES
        // ======================================

        var classesResult = await _supabase
            .From<TeacherAssignedClassRecord>()
            .Where(c => c.SchoolId == schoolId)
            .Get();

        var assignmentResult = await _supabase
            .From<TeacherDashboardTeachingAssignmentRecord>()
            .Where(a => a.TeacherId == teacher.Id)
            .Get();

        var activeClasses = classesResult.Models
            .Where(c =>
                c.IsActive &&
                c.AcademicYear == model.AcademicYear)
            .ToList();

        var activeAssignments = assignmentResult.Models
            .Where(a => a.IsActive)
            .ToList();

        var classIds = activeClasses
            .Select(c => c.Id)
            .ToHashSet();

        activeAssignments = activeAssignments
            .Where(a => classIds.Contains(a.ClassId))
            .ToList();

        var taughtClassIds = activeAssignments
            .Select(a => a.ClassId)
            .ToHashSet();

        // Restrict results to this teacher's
        // class-teacher or subject-teaching classes.
        var visibleClasses = activeClasses
            .Where(c =>
                c.ClassTeacherId == teacher.Id ||
                taughtClassIds.Contains(c.Id))
            .OrderBy(c => c.Grade)
            .ThenBy(c => c.Name)
            .ToList();

        // ======================================
        // CLASS COUNTS
        // ======================================

        model.HomeroomClassCount =
            visibleClasses.Count(c =>
                c.ClassTeacherId == teacher.Id);

        model.ClassesTaughtCount =
            visibleClasses.Count(c =>
                taughtClassIds.Contains(c.Id));

        // ======================================
        // GET SUBJECT NAMES
        // ======================================

        var subjectsResult = await _supabase
            .From<TeacherAssignedSubjectRecord>()
            .Where(s => s.SchoolId == schoolId)
            .Get();

        var subjectsById = subjectsResult.Models
            .Where(s => s.IsActive)
            .GroupBy(s => s.Id)
            .ToDictionary(
                g => g.Key,
                g => g.First().Name);

        model.SubjectCount = activeAssignments
            .Where(a =>
                subjectsById.ContainsKey(a.SubjectId))
            .Select(a => a.SubjectId)
            .Distinct()
            .Count();

        // ======================================
        // LOAD REAL WEEKLY TIMETABLE
        // ======================================
        // Supabase RLS allows only this Teacher's
        // assigned class + subject lessons.
        // Keep the school/teacher checks here too.

        var scheduleResult = await _supabase
            .From<TeacherAssignedScheduleRecord>()
            .Get();

        var timetable = scheduleResult.Models
            .Where(s =>
                classIds.Contains(s.ClassId) &&
                (!s.SchoolId.HasValue ||
                 s.SchoolId.Value == schoolId) &&
                (!s.TeacherId.HasValue ||
                 s.TeacherId.Value == teacher.Id) &&
                s.DayOfWeek >= 1 &&
                s.DayOfWeek <= 5 &&
                s.StartTime < s.EndTime)
            .ToList();

        // ======================================
        // BUILD ORIGINAL CLASS RECORDS
        // ======================================

        foreach (var schoolClass in visibleClasses)
        {
            var subjectNames = activeAssignments
                .Where(a =>
                    a.ClassId == schoolClass.Id &&
                    subjectsById.ContainsKey(a.SubjectId))
                .Select(a => subjectsById[a.SubjectId])
                .Distinct()
                .OrderBy(name => name)
                .ToList();


            // ======================================
            // BUILD SUBJECT OPTIONS WITH REAL IDS
            // ======================================

            var subjectOptions = activeAssignments
                .Where(a =>
                    a.ClassId == schoolClass.Id &&
                    subjectsById.ContainsKey(a.SubjectId))
                .Select(a =>
                    new TeacherAssignedSubjectOptionViewModel
                    {
                        Id = a.SubjectId,
                        Name = subjectsById[a.SubjectId]
                    })
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .OrderBy(s => s.Name)
                .ToList();


            model.Classes.Add(
                new TeacherAssignedClassItemViewModel
                {
                    Id = schoolClass.Id,

                    Name = schoolClass.Name,

                    Code = schoolClass.Code,

                    Grade = schoolClass.Grade,

                    RoomNumber = schoolClass.RoomNumber,

                    IsClassTeacher =
                        schoolClass.ClassTeacherId ==
                        teacher.Id,

                    TeachesSubjects =
                        taughtClassIds.Contains(
                            schoolClass.Id),

                    Subjects = subjectNames,
                    SubjectOptions = subjectOptions
                });
        }

        // ======================================
        // FIGMA: MY ASSIGNED CLASS
        // ======================================

        var homeroomClass = visibleClasses
            .FirstOrDefault(c =>
                c.ClassTeacherId == teacher.Id);

        if (homeroomClass != null)
        {
            model.AssignedClass =
                new TeacherAssignedClassSummaryViewModel
                {
                    ClassName = homeroomClass.Name,

                    ClassTeacherName = "You",

                    // Learner totals will come from
                    // enrollment records later.
                    LearnerCount = null,

                    Description =
                        "As the class teacher, you can " +
                        "monitor attendance, communicate " +
                        "with parents and guardians, " +
                        "and view learner progress."
                };
        }

        // ======================================
        // FIGMA: CLASSES I TEACH TABLE
        // ======================================

        foreach (var schoolClass in visibleClasses)
        {
            if (!taughtClassIds.Contains(schoolClass.Id))
            {
                continue;
            }

            // Use real SubjectId values, not names,
            // so similarly named subjects cannot
            // accidentally share timetable periods.
            var subjectAssignments = activeAssignments
                .Where(a =>
                    a.ClassId == schoolClass.Id &&
                    subjectsById.ContainsKey(a.SubjectId))
                .GroupBy(a => a.SubjectId)
                .Select(g => g.First())
                .OrderBy(a => subjectsById[a.SubjectId]);

            foreach (var assignment in subjectAssignments)
            {
                var lessonTimes = timetable
                    .Where(s =>
                        s.ClassId == schoolClass.Id &&
                        s.SubjectId == assignment.SubjectId)
                    .OrderBy(s => s.DayOfWeek)
                    .ThenBy(s => s.StartTime)
                    .Select(s =>
                        $"{ShortDay(s.DayOfWeek)} " +
                        $"{s.StartTime.ToString(@"hh\:mm")}" +
                        "–" +
                        $"{s.EndTime.ToString(@"hh\:mm")}")
                    .Distinct()
                    .ToList();

                model.ClassesITeach.Add(
                    new TeacherClassListItemViewModel
                    {
                        ClassId = schoolClass.Id,
                        ClassName = schoolClass.Name,
                        RoomNumber = schoolClass.RoomNumber
                            ?? "Not assigned",
                        SubjectName =
                            subjectsById[assignment.SubjectId],
                        StudentCount = null,
                        Schedule = lessonTimes.Count == 0
                            ? "Not scheduled"
                            : string.Join(" · ", lessonTimes)
                    });
            }
        }

        return model;
    }


    //
    // Method Name      : ShortDay
    // Purpose          : Handles short Day operations.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : int day
    // Output Type      : string.
    //

    private static string ShortDay(int day) => day switch
    {
        1 => "Mon",
        2 => "Tue",
        3 => "Wed",
        4 => "Thu",
        5 => "Fri",
        _ => "?"
    };


    //
    // Method Name      : PopulateLearnerMatchesAsync
    // Purpose          : Populates learner Matches.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : TeacherAssignedClassesViewModel model; Guid
    //                    schoolId; string? query
    // Output Type      : Task.
    //

    public async Task PopulateLearnerMatchesAsync(
        TeacherAssignedClassesViewModel model,
        Guid schoolId,
        string? query)
    {
        var searchText = query?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(searchText))
            return;

        if (searchText.Length > 100)
            searchText = searchText[..100];

        model.SearchQuery = searchText;

        // Restore authenticated Supabase session.
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (!model.HasTeacherProfile)
            return;

        // Only classes already authorized for this teacher.
        var allowedClasses = model.Classes
            .ToDictionary(
                c => c.Id,
                c => c.Name);

        if (allowedClasses.Count == 0)
            return;

        // Retrieve RLS-protected enrollments.
        var enrollmentResponse = await _supabase
            .From<TeacherLearnerSearchEnrollmentRecord>()
            .Where(e => e.IsActive)
            .Get();

        var permittedEnrollments = enrollmentResponse.Models
            .Where(e =>
                e.AcademicYear == model.AcademicYear &&
                allowedClasses.ContainsKey(e.ClassId) &&
                (e.EndDate == null ||
                 e.EndDate.Value.Date >= DateTime.Today))
            .ToList();

        if (permittedEnrollments.Count == 0)
            return;

        var learnerIds = permittedEnrollments
            .Select(e => e.LearnerId)
            .ToHashSet();

        // Retrieve RLS-protected learners.
        var learnerResponse = await _supabase
            .From<TeacherLearnerSearchRecord>()
            .Where(l =>
                l.SchoolId == schoolId &&
                l.IsActive)
            .Get();

        var matchingLearners = learnerResponse.Models
            .Where(l =>
                learnerIds.Contains(l.Id) &&
                (
                    l.FullName.Contains(
                        searchText,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    l.AdmissionNumber.Contains(
                        searchText,
                        StringComparison.OrdinalIgnoreCase)
                ))
            .ToList();

        model.LearnerMatches =
            (from enrollment in permittedEnrollments
             join learner in matchingLearners
                 on enrollment.LearnerId equals learner.Id
             select new TeacherLearnerSearchResultViewModel
             {
                 LearnerId = learner.Id,
                 ClassId = enrollment.ClassId,
                 FullName = learner.FullName,
                 AdmissionNumber = learner.AdmissionNumber,
                 ClassName =
                     allowedClasses[enrollment.ClassId]
             })
            .GroupBy(x => new
            {
                x.LearnerId,
                x.ClassId
            })
            .Select(g => g.First())
            .OrderBy(x => x.FullName)
            .Take(30)
            .ToList();
    }


}


// ==========================================
// READ-ONLY MAP: public.class_schedules
// ==========================================

//
// Class Name       : TeacherAssignedScheduleRecord
// Purpose          : Holds teacher assigned schedule record data used by
//                    the application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("class_schedules")]
public class TeacherAssignedScheduleRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("subject_id")]
    public Guid SubjectId { get; set; }

    [Column("school_id")]
    public Guid? SchoolId { get; set; }

    [Column("teacher_id")]
    public Guid? TeacherId { get; set; }

    [Column("day_of_week")]
    public int DayOfWeek { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }
}
