using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminGradeService
// Purpose          : Encapsulates school admin grade operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminGradeService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;


    //
    // Method Name      : SchoolAdminGradeService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminGradeService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    //
    // Method Name      : GetGradesAsync
    // Purpose          : Retrieves grades.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid? classId; Guid? subjectId;
    //                    string? term; int? academicYear; string?
    //                    searchTerm
    // Output Type      : Task<GradeManagementViewModel>.
    //

    public async Task<GradeManagementViewModel>
        GetGradesAsync(
            Guid schoolId,
            Guid? classId = null,
            Guid? subjectId = null,
            string? term = null,
            int? academicYear = null,
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
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
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
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
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
                        x.ClassId ==
                        classId.Value)
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
        // LOAD GRADES
        // -------------------------------------------------

        var gradeResponse =
            await _supabase
                .From<GradeRecord>()
                .Get();


        var gradeRecords =
            gradeResponse.Models
                .Where(x =>
                    classes.Any(c =>
                        c.Id == x.ClassId))
                .ToList();


        // -------------------------------------------------
        // BUILD ACADEMIC YEAR OPTIONS
        // -------------------------------------------------

        var academicYears =
            gradeRecords
                .Select(x =>
                    x.AcademicYear)
                .Distinct()
                .OrderByDescending(x =>
                    x)
                .ToList();


        if (academicYear.HasValue &&
            !academicYears.Contains(
                academicYear.Value))
        {
            academicYear = null;
        }


        // -------------------------------------------------
        // VALIDATE TERM FILTER
        // -------------------------------------------------

        var terms =
            new List<string>
            {
                "Term 1",
                "Term 2",
                "Term 3",
                "Term 4"
            };


        if (!string.IsNullOrWhiteSpace(term) &&
            !terms.Contains(
                term,
                StringComparer.OrdinalIgnoreCase))
        {
            term = null;
        }


        // -------------------------------------------------
        // APPLY CLASS FILTER
        // -------------------------------------------------

        if (classId.HasValue)
        {
            gradeRecords =
                gradeRecords
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
            gradeRecords =
                gradeRecords
                    .Where(x =>
                        x.SubjectId ==
                        subjectId.Value)
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY TERM FILTER
        // -------------------------------------------------

        if (!string.IsNullOrWhiteSpace(term))
        {
            gradeRecords =
                gradeRecords
                    .Where(x =>
                        string.Equals(
                            x.AcademicTerm,
                            term,
                            StringComparison
                                .OrdinalIgnoreCase))
                    .ToList();
        }


        // -------------------------------------------------
        // APPLY ACADEMIC YEAR FILTER
        // -------------------------------------------------

        if (academicYear.HasValue)
        {
            gradeRecords =
                gradeRecords
                    .Where(x =>
                        x.AcademicYear ==
                        academicYear.Value)
                    .ToList();
        }


        // -------------------------------------------------
        // BUILD GRADE LIST
        // -------------------------------------------------

        var records =
            new List<GradeListItemViewModel>();


        foreach (var grade in gradeRecords)
        {
            var learner =
                learners.FirstOrDefault(x =>
                    x.Id ==
                        grade.LearnerId);


            var schoolClass =
                classes.FirstOrDefault(x =>
                    x.Id ==
                        grade.ClassId);


            var subject =
                allSchoolSubjects
                    .FirstOrDefault(x =>
                        x.Id ==
                            grade.SubjectId);


            if (learner == null ||
                schoolClass == null)
            {
                continue;
            }


            var teacher =
                teachers.FirstOrDefault(x =>
                    x.Id ==
                        grade.TeacherId);


            UserProfile? teacherUser = null;


            if (teacher != null)
            {
                teacherUser =
                    users.FirstOrDefault(x =>
                        x.Id ==
                            teacher.UserId);
            }


            var teacherName =
                teacherUser?.FullName;


            if (string.IsNullOrWhiteSpace(
                    teacherName))
            {
                teacherName =
                    teacher?.EmployeeId ??
                    "Unknown teacher";
            }


            records.Add(
                new GradeListItemViewModel
                {
                    Id =
                        grade.Id,

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
                        "Unknown subject",

                    AssessmentName =
                        grade.AssessmentName,

                    AssessmentType =
                        grade.AssessmentType,

                    Score =
                        grade.Score,

                    GradeLetter =
                        grade.GradeLetter ?? "",

                    AcademicTerm =
                        grade.AcademicTerm,

                    AcademicYear =
                        grade.AcademicYear,

                    TeacherName =
                        teacherName,

                    Remarks =
                        grade.Remarks ?? "",

                    UpdatedAt =
                        grade.UpdatedAt
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


        // -------------------------------------------------
        // ORDER RESULTS
        // -------------------------------------------------

        records =
            records
                .OrderBy(x =>
                    x.LearnerName)
                .ThenBy(x =>
                    x.SubjectName)
                .ThenBy(x =>
                    x.AssessmentName)
                .ToList();


        // -------------------------------------------------
        // RETURN VIEW MODEL
        // -------------------------------------------------

        return new GradeManagementViewModel
        {
            SearchTerm =
                searchTerm ?? "",

            SelectedClassId =
                classId,

            SelectedSubjectId =
                subjectId,

            SelectedTerm =
                term ?? "",

            SelectedAcademicYear =
                academicYear,

            Classes =
                classes
                    .Select(x =>
                        new GradeClassOptionViewModel
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
                        new GradeSubjectOptionViewModel
                        {
                            Id =
                                x.Id,

                            Name =
                                x.Name
                        })
                    .ToList(),

            Terms =
                terms,

            AcademicYears =
                academicYears,

            Records =
                records
        };
    }
}