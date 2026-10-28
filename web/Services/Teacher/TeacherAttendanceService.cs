
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Models.Teacher;
using SchoolParentalSystem.Web.ViewModels.Teacher;

namespace SchoolParentalSystem.Web.Services.Teacher;

//
// Class Name       : TeacherAttendanceService
// Purpose          : Encapsulates teacher attendance operations for the
//                    School Parental System.
// Re-use           : Used by Teacher controllers and dashboard actions.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class TeacherAttendanceService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;

    private readonly TeacherAssignedClassesService
        _classesService;

    //
    // Method Name      : TeacherAttendanceService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService; TeacherAssignedClassesService
    //                    classesService
    // Output Type      : Constructor.
    //

    public TeacherAttendanceService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        TeacherAssignedClassesService classesService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
        _classesService = classesService;
    }


    //
    // Method Name      : GetAttendanceAsync
    // Purpose          : Retrieves attendance.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId; Guid? classId;
    //                    DateTime attendanceDate; Guid? subjectId
    // Output Type      : Task<TeacherAttendanceViewModel>.
    //

    public async Task<TeacherAttendanceViewModel>
        GetAttendanceAsync(
            Guid userId,
            Guid schoolId,
            Guid? classId,
            DateTime attendanceDate,
            Guid? subjectId = null)

    {
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        var model = new TeacherAttendanceViewModel
        {
            AttendanceDate = attendanceDate.Date
        };

        // =========================================
        // LOAD AUTHORIZED TEACHER CLASSES
        // =========================================

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        model.HasTeacherProfile =
            assignedClasses.HasTeacherProfile;

        model.Classes = assignedClasses.Classes
            .Select(c =>
                new TeacherAttendanceClassOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsClassTeacher = c.IsClassTeacher
                })
            .ToList();

        // =========================================
        // VALIDATE SELECTED CLASS
        // =========================================

        if (!classId.HasValue)
        {
            return model;
        }

        var selectedClass = assignedClasses.Classes
            .FirstOrDefault(c => c.Id == classId.Value);

        if (selectedClass == null)
        {
            // Never query an unauthorized class.
            return model;
        }


        model.SelectedClassId = selectedClass.Id;

        model.IsClassTeacher =
            selectedClass.IsClassTeacher;

        // =========================================
        // LOAD AUTHORIZED SUBJECTS FOR THIS CLASS
        // =========================================

        model.SubjectOptions = selectedClass.SubjectOptions
            .Select(s =>
                new TeacherAssignedSubjectOptionViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
            .ToList();

        // =========================================
        // SELECT DAILY OR SUBJECT ATTENDANCE
        // =========================================

        if (subjectId.HasValue)
        {
            // Never load an unauthorized subject.
            var authorizedSubject = model.SubjectOptions
                .FirstOrDefault(s => s.Id == subjectId.Value);

            if (authorizedSubject == null)
            {
                model.InvalidSubjectSelection = true;
                return model;
            }

            model.SelectedSubjectId = authorizedSubject.Id;
        }
        else
        {
            // Only the designated class teacher
            // can access the Daily Attendance register.
            if (!selectedClass.IsClassTeacher)
            {
                return model;
            }
        }

        model.CanLoadRegister = true;


        // =========================================
        // LOAD ACTIVE ENROLLMENTS
        // =========================================

        var enrollmentResponse = await _supabase
            .From<TeacherLearnerSearchEnrollmentRecord>()
            .Where(e => e.ClassId == selectedClass.Id)
            .Where(e => e.IsActive)
            .Get();

        var enrollments = enrollmentResponse.Models
            .Where(e =>
                e.AcademicYear == assignedClasses.AcademicYear &&
                (e.EndDate == null ||
                 e.EndDate.Value.Date >=
                    attendanceDate.Date))
            .ToList();

        if (enrollments.Count == 0)
        {
            return model;
        }

        var enrolledLearnerIds = enrollments
            .Select(e => e.LearnerId)
            .ToHashSet();

        // =========================================
        // LOAD AUTHORIZED LEARNERS
        // =========================================

        var learnerResponse = await _supabase
            .From<TeacherLearnerSearchRecord>()
            .Where(l => l.SchoolId == schoolId)
            .Where(l => l.IsActive)
            .Get();

        var learners = learnerResponse.Models
            .Where(l => enrolledLearnerIds.Contains(l.Id))
            .OrderBy(l => l.FullName)
            .ToList();

        // =========================================
        // LOAD EXISTING DAILY ATTENDANCE
        // =========================================

        var attendanceResponse = await _supabase
            .From<AttendanceRecord>()
            .Where(a => a.ClassId == selectedClass.Id)
            .Where(a =>
                a.AttendanceDate == attendanceDate.Date)
            .Get();


        // Daily Attendance: subject_id IS NULL.
        // Subject Attendance: match the selected subject ID.

        var selectedAttendance = attendanceResponse.Models
            .Where(a =>
                a.SubjectId == model.SelectedSubjectId)
            .GroupBy(a => a.LearnerId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(
                    a => a.UpdatedAt).First());


        // =========================================
        // BUILD COMPLETE LEARNER REGISTER
        // =========================================

        foreach (var learner in learners)
        {

            selectedAttendance.TryGetValue(
                learner.Id,
                out var existingAttendance);


            model.Learners.Add(
                new TeacherAttendanceLearnerViewModel
                {
                    LearnerId = learner.Id,

                    FullName = learner.FullName,

                    AdmissionNumber =
                        learner.AdmissionNumber,

                    Status = existingAttendance?.Status
                             ?? "Not marked",

                    Remarks = existingAttendance?.Remarks
                              ?? "",

                    AttendanceRecordId =
                        existingAttendance?.Id
                });
        }

        return model;
    }


    //
    // Method Name      : SaveDailyAttendanceAsync
    // Purpose          : Saves daily Attendance.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId;
    //                    TeacherAttendanceSaveViewModel request
    // Output Type      : Task.
    //

    public async Task SaveDailyAttendanceAsync(
        Guid userId,
        Guid schoolId,
        TeacherAttendanceSaveViewModel request)
    {
        // =========================================
        // RESTORE AUTHENTICATED SESSION
        // =========================================

        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (request == null ||
            request.ClassId == Guid.Empty)
        {
            throw new ArgumentException(
                "Select a valid class.");
        }

        // =========================================
        // VALIDATE TEACHER ASSIGNMENT
        // =========================================

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        var selectedClass =
            assignedClasses.Classes.FirstOrDefault(
                c => c.Id == request.ClassId);

        if (!assignedClasses.HasTeacherProfile ||
            selectedClass == null ||
            !selectedClass.IsClassTeacher)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to mark " +
                "daily attendance for this class.");
        }

        // =========================================
        // VALIDATE DATE AND SUBMISSION
        // =========================================

        var attendanceDate =
            request.AttendanceDate.Date;

        if (attendanceDate == DateTime.MinValue)
        {
            throw new ArgumentException(
                "Select a valid attendance date.");
        }

        if (request.Learners == null ||
            request.Learners.Count == 0 ||
            request.Learners.Count > 500)
        {
            throw new ArgumentException(
                "Invalid attendance register.");
        }

        if (request.Learners.Any(x =>
                x.LearnerId == Guid.Empty) ||
            request.Learners
                .GroupBy(x => x.LearnerId)
                .Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                "Duplicate or invalid learner IDs.");
        }

        // =========================================
        // PREPARE VALID ATTENDANCE ENTRIES
        // =========================================

        var entries = new List<object>();

        foreach (var learner in request.Learners)
        {
            var status = learner.Status?.Trim() ?? "";

            // Not marked is not an attendance record.
            if (string.IsNullOrWhiteSpace(status) ||
                status.Equals(
                    "Not marked",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (status != "Present" &&
                status != "Absent" &&
                status != "Late" &&
                status != "Excused")
            {
                throw new ArgumentException(
                    "Invalid attendance status.");
            }

            var remarks = learner.Remarks?.Trim();

            if (remarks?.Length > 255)
            {
                throw new ArgumentException(
                    "Attendance remarks must not " +
                    "exceed 255 characters.");
            }

            entries.Add(new
            {
                learner_id = learner.LearnerId,
                status,
                remarks = string.IsNullOrWhiteSpace(remarks)
                    ? null
                    : remarks
            });
        }

        if (entries.Count == 0)
        {
            throw new ArgumentException(
                "Mark at least one learner before saving.");
        }

        // =========================================
        // SAVE ATOMICALLY THROUGH SUPABASE
        // =========================================

        await _supabase.Rpc(
            "save_teacher_daily_attendance",
            new Dictionary<string, object>
            {
                ["p_class_id"] = request.ClassId,

                ["p_attendance_date"] =
                    attendanceDate.ToString(
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture),

                ["p_entries"] = entries
            });
    }


    // =========================================
    // SAVE ATTENDANCE - SELECT CORRECT REGISTER
    // =========================================

    //
    // Method Name      : SaveAttendanceAsync
    // Purpose          : Saves attendance.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId;
    //                    TeacherAttendanceSaveViewModel request
    // Output Type      : Task.
    //

    public async Task SaveAttendanceAsync(
        Guid userId,
        Guid schoolId,
        TeacherAttendanceSaveViewModel request)
    {
        if (request == null)
        {
            throw new ArgumentException(
                "Attendance information is required.");
        }

        if (request.SubjectId.HasValue)
        {
            await SaveSubjectAttendanceAsync(
                userId,
                schoolId,
                request);

            return;
        }

        // Preserve the working Daily Attendance logic.
        await SaveDailyAttendanceAsync(
            userId,
            schoolId,
            request);
    }


    // =========================================
    // SAVE SUBJECT ATTENDANCE
    // =========================================

    //
    // Method Name      : SaveSubjectAttendanceAsync
    // Purpose          : Saves subject Attendance.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId;
    //                    TeacherAttendanceSaveViewModel request
    // Output Type      : Task.
    //

    public async Task SaveSubjectAttendanceAsync(
        Guid userId,
        Guid schoolId,
        TeacherAttendanceSaveViewModel request)
    {
        // Restore authenticated Supabase session.
        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }

        if (request.ClassId == Guid.Empty ||
            !request.SubjectId.HasValue ||
            request.SubjectId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "Select a valid class and subject.");
        }

        // =====================================
        // CHECK AUTHORIZED SUBJECT ASSIGNMENT
        // =====================================

        var assignedClasses =
            await _classesService.GetAssignedClassesAsync(
                userId,
                schoolId);

        var selectedClass = assignedClasses.Classes
            .FirstOrDefault(c =>
                c.Id == request.ClassId);

        if (!assignedClasses.HasTeacherProfile ||
            selectedClass == null ||
            !selectedClass.SubjectOptions.Any(s =>
                s.Id == request.SubjectId.Value))
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to mark " +
                "attendance for this subject.");
        }

        // =====================================
        // VALIDATE SUBMITTED DATE AND LEARNERS
        // =====================================

        var attendanceDate =
            request.AttendanceDate.Date;

        if (attendanceDate == DateTime.MinValue ||
            attendanceDate.Year !=
                assignedClasses.AcademicYear)
        {
            throw new ArgumentException(
                "Invalid attendance date.");
        }

        if (request.Learners == null ||
            request.Learners.Count == 0 ||
            request.Learners.Count > 500)
        {
            throw new ArgumentException(
                "Invalid attendance register.");
        }

        if (request.Learners.Any(x =>
                x.LearnerId == Guid.Empty) ||
            request.Learners
                .GroupBy(x => x.LearnerId)
                .Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                "Duplicate or invalid learner IDs.");
        }

        // =====================================
        // VALIDATE ATTENDANCE ENTRIES
        // =====================================

        var entries = new List<object>();

        foreach (var learner in request.Learners)
        {
            var status =
                learner.Status?.Trim() ?? "";

            // Leave new unmarked learners unsaved.
            if (string.IsNullOrWhiteSpace(status) ||
                status.Equals(
                    "Not marked",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (status != "Present" &&
                status != "Absent" &&
                status != "Late" &&
                status != "Excused")
            {
                throw new ArgumentException(
                    "Invalid attendance status.");
            }

            var remarks =
                learner.Remarks?.Trim();

            if (remarks?.Length > 255)
            {
                throw new ArgumentException(
                    "Remarks must not exceed 255 characters.");
            }

            entries.Add(new
            {
                learner_id = learner.LearnerId,
                status,
                remarks = string.IsNullOrWhiteSpace(remarks)
                    ? null
                    : remarks
            });
        }

        if (entries.Count == 0)
        {
            throw new ArgumentException(
                "Mark at least one learner before saving.");
        }

        // =====================================
        // ATOMIC SUPABASE SAVE
        // =====================================

        await _supabase.Rpc(
            "save_teacher_subject_attendance",
            new Dictionary<string, object>
            {
                ["p_class_id"] = request.ClassId,

                ["p_subject_id"] =
                    request.SubjectId.Value,

                ["p_attendance_date"] =
                    attendanceDate.ToString(
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture),

                ["p_entries"] = entries
            });
    }


}
