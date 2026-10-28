
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Models.Teacher;
using SchoolParentalSystem.Web.ViewModels.Teacher;

namespace SchoolParentalSystem.Web.Services.Teacher;

//
// Class Name       : TeacherGradesService
// Purpose          : Encapsulates teacher grades operations for the School
//                    Parental System.
// Re-use           : Used by Teacher controllers and dashboard actions.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class TeacherGradesService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;
    private readonly TeacherAssignedClassesService _classesService;

    //
    // Method Name      : TeacherGradesService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService; TeacherAssignedClassesService
    //                    classesService
    // Output Type      : Constructor.
    //

    public TeacherGradesService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        TeacherAssignedClassesService classesService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
        _classesService = classesService;
    }


    //
    // Method Name      : GetGradesAsync
    // Purpose          : Retrieves grades.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId; Guid? classId; Guid?
    //                    subjectId; string? term; string? searchTerm;
    //                    string? editAssessment
    // Output Type      : Task<TeacherGradesViewModel>.
    //

    public async Task<TeacherGradesViewModel> GetGradesAsync(
        Guid userId,
        Guid schoolId,
        Guid? classId,
        Guid? subjectId,
        string? term,
        string? searchTerm,
        string? editAssessment = null)

    {
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        var model = new TeacherGradesViewModel();

        // ==========================================
        // LOAD AUTHORIZED TEACHER ASSIGNMENTS
        // ==========================================

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        model.HasTeacherProfile =
            assignedClasses.HasTeacherProfile;

        model.AcademicYear =
            assignedClasses.AcademicYear;

        // Only show classes where this teacher
        // has an assigned subject.
        model.Classes = assignedClasses.Classes
            .Where(c => c.SubjectOptions.Count > 0)
            .Select(c =>
                new TeacherGradeClassOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
            .ToList();

        if (!classId.HasValue)
        {
            return model;
        }

        // ==========================================
        // VALIDATE THE SELECTED CLASS
        // ==========================================

        var selectedClass = assignedClasses.Classes
            .FirstOrDefault(c =>
                c.Id == classId.Value &&
                c.SubjectOptions.Count > 0);

        if (selectedClass == null)
        {
            model.InvalidSelection = true;
            return model;
        }

        model.SelectedClassId = selectedClass.Id;

        // ==========================================
        // LOAD AUTHORIZED SUBJECTS
        // ==========================================

        model.Subjects = selectedClass.SubjectOptions
            .Select(s =>
                new TeacherGradeSubjectOptionViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
            .OrderBy(s => s.Name)
            .ToList();

        var authorizedSubjectIds = model.Subjects
            .Select(s => s.Id)
            .ToHashSet();

        if (subjectId.HasValue &&
            !authorizedSubjectIds.Contains(subjectId.Value))
        {
            model.InvalidSelection = true;
            return model;
        }

        model.SelectedSubjectId = subjectId;

        // ==========================================
        // VALIDATE TERM
        // ==========================================

        var selectedTerm = term?.Trim() ?? "";

        if (!string.IsNullOrWhiteSpace(selectedTerm) &&
            !model.Terms.Contains(selectedTerm))
        {
            model.InvalidSelection = true;
            return model;
        }

        model.SelectedTerm = selectedTerm;

        // ==========================================
        // VALIDATE SEARCH TEXT
        // ==========================================

        var search = searchTerm?.Trim() ?? "";

        if (search.Length > 100)
        {
            search = search[..100];
        }

        model.SearchTerm = search;


        // ==========================================
        // LOAD ENROLLED LEARNERS FOR GRADE ENTRY
        // ==========================================

        if (subjectId.HasValue)
        {
            // Only classes and subjects already
            // authorized above reach this section.

            var enrollmentResponse = await _supabase
                .From<TeacherLearnerSearchEnrollmentRecord>()
                .Where(e => e.ClassId == selectedClass.Id)
                .Where(e => e.IsActive)
                .Get();

            var enrolledLearnerIds = enrollmentResponse.Models
                .Where(e =>
                    e.AcademicYear == model.AcademicYear &&
                    (e.EndDate == null ||
                     e.EndDate.Value.Date >= DateTime.Today))
                .Select(e => e.LearnerId)
                .ToHashSet();

            if (enrolledLearnerIds.Count > 0)
            {
                var registerResponse = await _supabase
                    .From<TeacherLearnerSearchRecord>()
                    .Where(l => l.SchoolId == schoolId)
                    .Where(l => l.IsActive)
                    .Get();

                model.EntryLearners = registerResponse.Models
                    .Where(l =>
                        enrolledLearnerIds.Contains(l.Id))
                    .GroupBy(l => l.Id)
                    .Select(g => g.First())
                    .OrderBy(l => l.FullName)
                    .Select(l =>
                        new TeacherGradeEntryLearnerViewModel
                        {
                            LearnerId = l.Id,

                            FullName = l.FullName,

                            AdmissionNumber =
                                l.AdmissionNumber
                        })
                    .ToList();
            }
        }


        // ==========================================
        // LOAD RLS-PROTECTED GRADES
        // ==========================================


        var gradeResponse = await _supabase
            .From<TeacherGradeDetailRecord>()
            .Where(g => g.ClassId == selectedClass.Id)
            .Get();


        var grades = gradeResponse.Models
            .Where(g =>
                g.AcademicYear == model.AcademicYear &&
                authorizedSubjectIds.Contains(g.SubjectId))
            .ToList();

        if (subjectId.HasValue)
        {
            grades = grades
                .Where(g => g.SubjectId == subjectId.Value)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(selectedTerm))
        {
            grades = grades
                .Where(g => g.AcademicTerm == selectedTerm)
                .ToList();
        }

        if (grades.Count == 0)
        {
            return model;
        }


        // ==========================================
        // IDENTIFY THE LOGGED-IN TEACHER
        // ==========================================

        var teacherResponse = await _supabase
            .From<TeacherDashboardTeacherRecord>()
            .Where(t => t.UserId == userId)
            .Get();

        var currentTeacher = teacherResponse.Models
            .FirstOrDefault(t =>
                t.SchoolId == schoolId &&
                t.IsActive);

        if (currentTeacher == null)
        {
            throw new UnauthorizedAccessException(
                "Active Teacher profile not found.");
        }

        var currentTeacherId = currentTeacher.Id;


        // ==========================================
        // LOAD AUTHORIZED LEARNER DETAILS
        // ==========================================

        var learnerIds = grades
            .Select(g => g.LearnerId)
            .ToHashSet();

        var learnerResponse = await _supabase
            .From<TeacherLearnerSearchRecord>()
            .Where(l => l.SchoolId == schoolId)
            .Get();

        var learnersById = learnerResponse.Models
            .Where(l => learnerIds.Contains(l.Id))
            .GroupBy(l => l.Id)
            .ToDictionary(g => g.Key, g => g.First());

        var subjectsById = model.Subjects
            .ToDictionary(s => s.Id, s => s.Name);

        // ==========================================
        // BUILD GRADE RECORDS
        // ==========================================

        foreach (var grade in grades)
        {
            // Do not display inaccessible learners.
            if (!learnersById.TryGetValue(
                grade.LearnerId,
                out var learner))
            {
                continue;
            }

            if (!subjectsById.TryGetValue(
                grade.SubjectId,
                out var subjectName))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(search) &&
                !learner.FullName.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) &&
                !learner.AdmissionNumber.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            model.Records.Add(
                new TeacherGradeRecordViewModel
                {
                    Id = grade.Id,
                    LearnerId = learner.Id,
                    SubjectId = grade.SubjectId,

                    AdmissionNumber =
                        learner.AdmissionNumber,

                    LearnerName = learner.FullName,

                    SubjectName = subjectName,

                    AssessmentName = grade.AssessmentName,
                    AssessmentType = grade.AssessmentType,

                    Score = Convert.ToDecimal(grade.Score),
                    GradeLetter = grade.GradeLetter ?? "",

                    AcademicTerm = grade.AcademicTerm,
                    AcademicYear = grade.AcademicYear,


                    Remarks = grade.Remarks ?? "",

                    PublicationStatus = grade.PublicationStatus,

                    MarksObtained = grade.MarksObtained,

                    MaximumMarks = grade.MaximumMarks,

                    CanEditDraft =
                    grade.PublicationStatus == "Draft" &&
                    grade.TeacherId == currentTeacherId

                });
        }

        model.Records = model.Records
            .OrderBy(r => r.LearnerName)
            .ThenBy(r => r.SubjectName)
            .ThenBy(r => r.AssessmentName)
            .ToList();


        // ==========================================
        // LOAD AN EXISTING DRAFT FOR EDITING
        // ==========================================

        var editName = editAssessment?.Trim() ?? "";

        if (!string.IsNullOrWhiteSpace(editName))
        {
            if (editName.Length > 100 ||
                !subjectId.HasValue ||
                string.IsNullOrWhiteSpace(selectedTerm))
            {
                model.InvalidSelection = true;
                return model;
            }

            var draftRecords = grades
                .Where(g =>
                    g.SubjectId == subjectId.Value &&
                    g.AcademicTerm == selectedTerm &&
                    g.AssessmentName == editName)
                .ToList();

            if (draftRecords.Count == 0 ||
                draftRecords.Any(g =>
                    g.PublicationStatus != "Draft" ||
                    g.TeacherId != currentTeacherId))
            {
                model.InvalidSelection = true;
                return model;
            }

            var firstDraft = draftRecords[0];

            if (!firstDraft.MaximumMarks.HasValue ||
                firstDraft.MaximumMarks.Value <= 0 ||
                draftRecords.Any(g =>
                    g.MaximumMarks != firstDraft.MaximumMarks ||
                    g.AssessmentType != firstDraft.AssessmentType))
            {
                model.InvalidSelection = true;
                return model;
            }

            model.EditingAssessmentName =
                firstDraft.AssessmentName;

            model.EditingAssessmentType =
                firstDraft.AssessmentType;

            model.EditingTerm =
                firstDraft.AcademicTerm;

            model.EditingMaximumMarks =
                firstDraft.MaximumMarks;

            var existingMarks = draftRecords
                .GroupBy(g => g.LearnerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

            foreach (var learner in model.EntryLearners)
            {
                if (existingMarks.TryGetValue(
                    learner.LearnerId,
                    out var existing))
                {
                    learner.MarksObtained =
                        existing.MarksObtained;

                    learner.Remarks =
                        existing.Remarks ?? "";
                }
            }
        }


        return model;
    }


    /* ==========================================
       SAVE TEACHER GRADE DRAFT
       ========================================== */

    //
    // Method Name      : SaveDraftAsync
    // Purpose          : Saves draft.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId;
    //                    TeacherGradeDraftSaveViewModel request
    // Output Type      : Task.
    //

    public async Task SaveDraftAsync(
        Guid userId,
        Guid schoolId,
        TeacherGradeDraftSaveViewModel request)
    {
        // RESTORE SUPABASE SESSION

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        // VALIDATE CLASS AND SUBJECT

        if (request.ClassId == Guid.Empty ||
            request.SubjectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Select a valid class and subject.");
        }

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        var selectedClass = assignedClasses.Classes
            .FirstOrDefault(c => c.Id == request.ClassId);

        if (!assignedClasses.HasTeacherProfile ||
            selectedClass == null ||
            !selectedClass.SubjectOptions.Any(s =>
                s.Id == request.SubjectId))
        {
            throw new InvalidOperationException(
                "You are not assigned to this " +
                "class and subject.");
        }

        // VALIDATE ASSESSMENT INFORMATION

        var assessmentName =
            request.AssessmentName?.Trim() ?? "";

        var assessmentType =
            request.AssessmentType?.Trim() ?? "";

        var academicTerm =
            request.AcademicTerm?.Trim() ?? "";

        if (assessmentName.Length == 0 ||
            assessmentName.Length > 100)
        {
            throw new ArgumentException(
                "Assessment name must contain " +
                "1 to 100 characters.");
        }

        if (assessmentType != "Test" &&
            assessmentType != "Assignment" &&
            assessmentType != "Exam")
        {
            throw new ArgumentException(
                "Select a valid assessment type.");
        }

        if (academicTerm != "Term 1" &&
            academicTerm != "Term 2" &&
            academicTerm != "Term 3" &&
            academicTerm != "Term 4")
        {
            throw new ArgumentException(
                "Select a valid academic term.");
        }

        if (request.AcademicYear !=
            assignedClasses.AcademicYear)
        {
            throw new ArgumentException(
                "Invalid academic year.");
        }

        if (request.MaximumMarks <= 0 ||
            request.MaximumMarks > 10000)
        {
            throw new ArgumentException(
                "Enter valid maximum marks.");
        }

        // VALIDATE LEARNER ENTRIES

        if (request.Learners == null ||
            request.Learners.Count == 0 ||
            request.Learners.Count > 500)
        {
            throw new ArgumentException(
                "Invalid learner grade register.");
        }

        if (request.Learners.Any(l =>
                l.LearnerId == Guid.Empty) ||
            request.Learners
                .GroupBy(l => l.LearnerId)
                .Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                "Duplicate or invalid learner IDs.");
        }

        var entries = new List<object>();

        foreach (var learner in request.Learners)
        {
            // A learner without entered marks
            // is not saved as zero.
            if (!learner.MarksObtained.HasValue)
            {
                continue;
            }

            var marks = learner.MarksObtained.Value;

            if (marks < 0 ||
                marks > request.MaximumMarks)
            {
                throw new ArgumentException(
                    "Learner marks must be between " +
                    "zero and the maximum marks.");
            }

            var remarks = learner.Remarks?.Trim();

            if (remarks?.Length > 255)
            {
                throw new ArgumentException(
                    "Remarks cannot exceed 255 characters.");
            }

            entries.Add(new
            {
                learner_id = learner.LearnerId,
                marks_obtained = marks,
                remarks = string.IsNullOrWhiteSpace(remarks)
                    ? null
                    : remarks
            });
        }

        if (entries.Count == 0)
        {
            throw new ArgumentException(
                "Enter marks for at least one learner.");
        }

        // SAVE GRADES ATOMICALLY TO SUPABASE

        await _supabase.Rpc(
            "save_teacher_grade_draft",
            new Dictionary<string, object>
            {
                ["p_class_id"] = request.ClassId,
                ["p_subject_id"] = request.SubjectId,

                ["p_assessment_name"] =
                    assessmentName,

                ["p_assessment_type"] =
                    assessmentType,

                ["p_academic_term"] =
                    academicTerm,

                ["p_academic_year"] =
                    request.AcademicYear,

                ["p_maximum_marks"] =
                    request.MaximumMarks,

                ["p_entries"] = entries
            });
    }


    /* ==========================================
       PUBLISH TEACHER ASSESSMENT
       ========================================== */

    //
    // Method Name      : PublishAssessmentAsync
    // Purpose          : Publishes assessment.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId; Guid classId; Guid
    //                    subjectId; string assessmentName; string
    //                    academicTerm; int academicYear
    // Output Type      : Task.
    //

    public async Task PublishAssessmentAsync(
        Guid userId,
        Guid schoolId,
        Guid classId,
        Guid subjectId,
        string assessmentName,
        string academicTerm,
        int academicYear)
    {
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (classId == Guid.Empty ||
            subjectId == Guid.Empty ||
            string.IsNullOrWhiteSpace(assessmentName))
        {
            throw new ArgumentException(
                "Invalid assessment selection.");
        }

        assessmentName = assessmentName.Trim();
        academicTerm = academicTerm?.Trim() ?? "";

        if (assessmentName.Length > 100 ||
            !new[]
            {
            "Term 1",
            "Term 2",
            "Term 3",
            "Term 4"
            }.Contains(academicTerm))
        {
            throw new ArgumentException(
                "Invalid assessment details.");
        }

        // Verify the Teacher's assigned class and subject.

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        var selectedClass = assignedClasses.Classes
            .FirstOrDefault(c => c.Id == classId);

        if (!assignedClasses.HasTeacherProfile ||
            selectedClass == null ||
            !selectedClass.SubjectOptions.Any(s =>
                s.Id == subjectId))
        {
            throw new UnauthorizedAccessException(
                "You are not assigned to this subject.");
        }

        if (academicYear != assignedClasses.AcademicYear)
        {
            throw new ArgumentException(
                "Invalid academic year.");
        }

        // Publish through the secured database function.
        // The database checks assessment completeness
        // and publishes all learner grades atomically.

        await _supabase.Rpc(
            "publish_teacher_assessment",
            new Dictionary<string, object>
            {
                ["p_class_id"] = classId,
                ["p_subject_id"] = subjectId,

                ["p_assessment_name"] = assessmentName,
                ["p_academic_term"] = academicTerm,
                ["p_academic_year"] = academicYear
            });
    }

    /* ==========================================
       TEACHER DASHBOARD GRADE STATISTICS
       ========================================== */

    //
    // Method Name      : GetDashboardGradeStatsAsync
    // Purpose          : Retrieves dashboard Grade Stats.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<TeacherGradeDashboardStatsViewModel>.
    //

    public async Task<TeacherGradeDashboardStatsViewModel>
        GetDashboardGradeStatsAsync(
            Guid userId,
            Guid schoolId)
    {
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        var assigned = await _classesService
            .GetAssignedClassesAsync(userId, schoolId);

        var summary =
            new TeacherGradeDashboardStatsViewModel
            {
                AcademicYear = assigned.AcademicYear
            };

        if (!assigned.HasTeacherProfile)
        {
            return summary;
        }

        // Build authorized class/subject pairs.

        var authorizedPairs = assigned.Classes
            .SelectMany(c =>
                c.SubjectOptions.Select(s =>
                    (ClassId: c.Id, SubjectId: s.Id)))
            .ToHashSet();

        if (authorizedPairs.Count == 0)
        {
            return summary;
        }

        // Supabase RLS also restricts visible grades
        // to the Teacher's assigned subjects.

        var response = await _supabase
            .From<TeacherGradeDetailRecord>()
            .Where(g =>
                g.AcademicYear == assigned.AcademicYear)
            .Get();

        var grades = response.Models
            .Where(g =>
                authorizedPairs.Contains(
                    (g.ClassId, g.SubjectId)))
            .ToList();

        summary.TotalEntries = grades.Count;

        summary.PublishedEntries = grades
            .Count(g =>
                g.PublicationStatus == "Published");

        summary.DraftEntries = grades
            .Count(g =>
                g.PublicationStatus == "Draft");

        summary.AssessmentCount = grades
            .Select(g => new
            {
                g.ClassId,
                g.SubjectId,
                g.AssessmentName,
                g.AcademicTerm,
                g.AcademicYear
            })
            .Distinct()
            .Count();

        summary.AverageScore = grades.Count > 0
            ? Math.Round(grades.Average(g => g.Score), 1)
            : null;

        return summary;
    }


}
