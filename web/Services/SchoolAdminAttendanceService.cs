using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminAttendanceService
// Purpose          : Encapsulates school admin attendance operations for
//                    the School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminAttendanceService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SchoolAdminAttendanceService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminAttendanceService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;

        _sessionService = sessionService;
    }


    //
    // Method Name      : GetAttendanceAsync
    // Purpose          : Retrieves attendance.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; DateTime attendanceDate; Guid?
    //                    classId; Guid? subjectId; string? searchTerm
    // Output Type      : Task<AttendanceManagementViewModel>.
    //

    public async Task<AttendanceManagementViewModel>
        GetAttendanceAsync(
            Guid schoolId,
            DateTime attendanceDate,
            Guid? classId = null,
            Guid? subjectId = null,
            string? searchTerm = null)
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
                    x.SchoolId == schoolId &&
                    x.IsActive)
                .OrderBy(x =>
                    x.Grade)
                .ThenBy(x =>
                    x.Name)
                .ToList();


        // -------------------------------------------------
        // LOAD SCHOOL SUBJECTS
        // -------------------------------------------------

        var subjectResponse =
            await _supabase
                .From<SubjectRecord>()
                .Get();


        var allSchoolSubjects =
            subjectResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        var activeSchoolSubjects =
            allSchoolSubjects
                .Where(x =>
                    x.IsActive)
                .OrderBy(x =>
                    x.Name)
                .ToList();


        // -------------------------------------------------
        // LOAD TEACHING ASSIGNMENTS
        // -------------------------------------------------

        var assignmentResponse =
            await _supabase
                .From<TeachingAssignmentRecord>()
                .Get();


        var teachingAssignments =
            assignmentResponse.Models
                .Where(x =>
                    x.IsActive &&
                    classes.Any(c =>
                        c.Id == x.ClassId))
                .ToList();


        // -------------------------------------------------
        // VALIDATE CLASS FILTER
        // -------------------------------------------------

        if (classId.HasValue &&
            !classes.Any(x =>
                x.Id == classId.Value))
        {
            classId = null;
        }


        // -------------------------------------------------
        // FILTER SUBJECTS BY SELECTED CLASS
        // -------------------------------------------------

        var subjects =
            activeSchoolSubjects;


        if (classId.HasValue)
        {
            var assignedSubjectIds =
                teachingAssignments
                    .Where(x =>
                        x.ClassId == classId.Value)
                    .Select(x =>
                        x.SubjectId)
                    .Distinct()
                    .ToHashSet();


            subjects =
                activeSchoolSubjects
                    .Where(x =>
                        assignedSubjectIds.Contains(
                            x.Id))
                    .OrderBy(x =>
                        x.Name)
                    .ToList();
        }


        // -------------------------------------------------
        // VALIDATE SUBJECT FILTER
        // -------------------------------------------------

        if (subjectId.HasValue &&
            !subjects.Any(x =>
                x.Id == subjectId.Value))
        {
            subjectId = null;
        }


        // -------------------------------------------------
        // LOAD LEARNERS
        // -------------------------------------------------

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Get();


        var learners =
            learnerResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD TEACHERS
        // -------------------------------------------------

        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Get();


        var teachers =
            teacherResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD USER PROFILES
        // -------------------------------------------------

        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Get();


        var users =
            userResponse.Models
                .Where(x =>
                    x.SchoolId == schoolId)
                .ToList();


        // -------------------------------------------------
        // LOAD ATTENDANCE FOR SELECTED DATE
        // -------------------------------------------------

        var attendanceResponse =
            await _supabase
                .From<AttendanceRecord>()
                .Where(x =>
                    x.AttendanceDate ==
                        attendanceDate.Date)
                .Get();


        var attendanceRecords =
            attendanceResponse.Models
                .Where(x =>
                    classes.Any(c =>
                        c.Id == x.ClassId))
                .ToList();


        // -------------------------------------------------
        // APPLY CLASS FILTER
        // -------------------------------------------------

        if (classId.HasValue)
        {
            attendanceRecords =
                attendanceRecords
                    .Where(x =>
                        x.ClassId ==
                            classId.Value)
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY SUBJECT FILTER
        // -------------------------------------------------

        if (subjectId.HasValue)
        {
            attendanceRecords =
                attendanceRecords
                    .Where(x =>
                        x.SubjectId ==
                            subjectId.Value)
                    .ToList();
        }


        // -------------------------------------------------
        // BUILD ATTENDANCE LIST
        // -------------------------------------------------

        var records =
            new List<AttendanceListItemViewModel>();


        foreach (var attendance
            in attendanceRecords)
        {
            var learner =
                learners.FirstOrDefault(x =>
                    x.Id ==
                        attendance.LearnerId);


            var schoolClass =
                classes.FirstOrDefault(x =>
                    x.Id ==
                        attendance.ClassId);


            if (learner == null ||
                schoolClass == null)
            {
                continue;
            }


            SubjectRecord? subject = null;

            if (attendance.SubjectId.HasValue)
            {
                subject =
                    allSchoolSubjects.FirstOrDefault(x =>
                        x.Id ==
                            attendance.SubjectId.Value);
            }


            var teacher =
                teachers.FirstOrDefault(x =>
                    x.Id ==
                        attendance.MarkedBy);


            UserProfile? teacherUser = null;

            if (teacher != null)
            {
                teacherUser =
                    users.FirstOrDefault(x =>
                        x.Id ==
                            teacher.UserId);
            }


            var markedByName =
                teacherUser?.FullName;


            if (string.IsNullOrWhiteSpace(
                    markedByName))
            {
                markedByName =
                    teacher?.EmployeeId ??
                    "Unknown teacher";
            }


            records.Add(
                new AttendanceListItemViewModel
                {
                    Id =
                        attendance.Id,

                    LearnerId =
                        learner.Id,

                    AdmissionNumber =
                        learner.AdmissionNumber,

                    LearnerName =
                        learner.FullName,

                    ClassName =
                        schoolClass.Name,

                    ClassCode =
                        schoolClass.Code,

                    SubjectName =
                        subject?.Name ??
                        "Not specified",

                    AttendanceDate =
                        attendance.AttendanceDate,

                    Status =
                        attendance.Status,

                    CheckInTime =
                        attendance.CheckInTime,

                    Remarks =
                        attendance.Remarks ?? "",

                    MarkedByName =
                        markedByName,

                    UpdatedAt =
                        attendance.UpdatedAt
                });
        }


        // -------------------------------------------------
        // APPLY LEARNER SEARCH
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                searchTerm))
        {
            var search =
                searchTerm.Trim();


            records =
                records
                    .Where(x =>
                        x.LearnerName.Contains(
                            search,
                            StringComparison
                                .OrdinalIgnoreCase) ||

                        x.AdmissionNumber.Contains(
                            search,
                            StringComparison
                                .OrdinalIgnoreCase))
                    .ToList();
        }


        records =
            records
                .OrderBy(x =>
                    x.LearnerName)
                .ToList();


        // -------------------------------------------------
        // RETURN VIEW MODEL
        // -------------------------------------------------

        return new AttendanceManagementViewModel
        {
            SearchTerm =
                searchTerm ?? "",

            SelectedClassId =
                classId,

            SelectedSubjectId =
                subjectId,

            AttendanceDate =
                attendanceDate.Date,

            Classes =
                classes
                    .Select(x =>
                        new AttendanceClassOptionViewModel
                        {
                            Id =
                                x.Id,

                            Name =
                                x.Name,

                            Code =
                                x.Code
                        })
                    .ToList(),

            Subjects =
                subjects
                    .Select(x =>
                        new AttendanceSubjectOptionViewModel
                        {
                            Id =
                                x.Id,

                            Name =
                                x.Name
                        })
                    .ToList(),

            Records =
                records
        };
    }
}