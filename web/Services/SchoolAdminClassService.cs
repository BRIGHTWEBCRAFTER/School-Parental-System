using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminClassService
// Purpose          : Encapsulates school admin class operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminClassService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SchoolAdminClassService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminClassService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    //
    // Method Name      : GetClassesAsync
    // Purpose          : Retrieves classes.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId
    // Output Type      : Task<ClassesPageViewModel>.
    //

    public async Task<ClassesPageViewModel>
        GetClassesAsync(Guid schoolId)
    {
        var restored =
            await _sessionService.RestoreAsync();


        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var classes =
            classResponse.Models;


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var teachers =
            teacherResponse.Models;


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var users =
            userResponse.Models;


        var items =
            new List<ClassListItemViewModel>();


        foreach (var schoolClass in classes)
        {
            var teacherName =
                "Not assigned";


            if (schoolClass.ClassTeacherId.HasValue)
            {
                var teacher =
                    teachers.FirstOrDefault(
                        x =>
                            x.Id ==
                            schoolClass.ClassTeacherId.Value);


                if (teacher != null)
                {
                    var teacherUser =
                        users.FirstOrDefault(
                            x =>
                                x.Id ==
                                teacher.UserId);


                    if (teacherUser != null &&
                        !string.IsNullOrWhiteSpace(
                            teacherUser.FullName))
                    {
                        teacherName =
                            teacherUser.FullName;
                    }
                }
            }


            items.Add(
                new ClassListItemViewModel
                {
                    Id =
                        schoolClass.Id,

                    Code =
                        schoolClass.Code,

                    Name =
                        schoolClass.Name,

                    Grade =
                        schoolClass.Grade,

                    Section =
                        schoolClass.Section ?? "",

                    ClassTeacherName =
                        teacherName,

                    RoomNumber =
                        schoolClass.RoomNumber ?? "",

                    Capacity =
                        schoolClass.Capacity,

                    AcademicYear =
                        schoolClass.AcademicYear,

                    IsActive =
                        schoolClass.IsActive
                });
        }


        items =
            items
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Section)
                .ThenBy(x => x.Name)
                .ToList();


        return new ClassesPageViewModel
        {
            TotalClasses =
                items.Count,

            ActiveClasses =
                items.Count(x =>
                    x.IsActive),

            InactiveClasses =
                items.Count(x =>
                    !x.IsActive),

            Classes =
                items
        };
    }

    //
    // Method Name      : GetAddClassModelAsync
    // Purpose          : Retrieves add Class Model.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; int academicYear
    // Output Type      : Task<AddClassViewModel>.
    //

    public async Task<AddClassViewModel>
    GetAddClassModelAsync(
        Guid schoolId,
        int academicYear)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.SchoolId == schoolId &&
                    x.IsActive == true)
                .Get();


        var teachers =
            teacherResponse.Models;


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var users =
            userResponse.Models;


        var teacherOptions =
            new List<ClassTeacherOptionViewModel>();


        foreach (var teacher in teachers)
        {
            var user =
                users.FirstOrDefault(
                    x =>
                        x.Id == teacher.UserId &&
                        x.IsActive == true);


            if (user == null)
            {
                continue;
            }


            teacherOptions.Add(
                new ClassTeacherOptionViewModel
                {
                    Id =
                        teacher.Id,

                    DisplayName =
                        $"{user.FullName} ({teacher.EmployeeId})"
                });
        }


        return new AddClassViewModel
        {
            AcademicYear =
                academicYear,

            Teachers =
                teacherOptions
                    .OrderBy(x => x.DisplayName)
                    .ToList()
        };
    }

    //
    // Method Name      : AddClassAsync
    // Purpose          : Adds class.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; AddClassViewModel model
    // Output Type      : Task<Guid>.
    //

    public async Task<Guid> AddClassAsync(
    Guid schoolId,
    AddClassViewModel model)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // If a teacher was selected, verify that the
        // teacher belongs to this school and is active.
        if (model.ClassTeacherId.HasValue)
        {
            var teacherResponse =
                await _supabase
                    .From<TeacherRecord>()
                    .Where(x =>
                        x.Id ==
                            model.ClassTeacherId.Value &&
                        x.SchoolId ==
                            schoolId &&
                        x.IsActive == true)
                    .Get();


            if (!teacherResponse.Models.Any())
            {
                throw new InvalidOperationException(
                    "The selected teacher is not available.");
            }
        }

        var now =
            DateTime.UtcNow;


        var newClass =
            new SchoolClassRecord
            {
                Name =
                    model.Name.Trim(),

                Code =
                    model.Code.Trim()
                        .ToUpperInvariant(),

                SchoolId =
                    schoolId,

                Grade =
                    model.Grade.Trim(),

                Section =
                    string.IsNullOrWhiteSpace(
                        model.Section)
                        ? null
                        : model.Section.Trim(),

                ClassTeacherId =
                    model.ClassTeacherId,

                RoomNumber =
                    string.IsNullOrWhiteSpace(
                        model.RoomNumber)
                        ? null
                        : model.RoomNumber.Trim(),

                Capacity =
                    model.Capacity,

                AcademicYear =
                    model.AcademicYear,

                IsActive =
                    true,

                CreatedAt =
                    now,

                UpdatedAt =
                    now
            };


        var response =
            await _supabase
                .From<SchoolClassRecord>()
                .Insert(newClass);


        var created =
            response.Models.FirstOrDefault();


        if (created == null)
        {
            throw new InvalidOperationException(
                "The class could not be created.");
        }


        return created.Id;
    }

    //
    // Method Name      : GetClassDetailsAsync
    // Purpose          : Retrieves class Details.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid classId
    // Output Type      : Task<ClassDetailsViewModel?>.
    //

    public async Task<ClassDetailsViewModel?>
    GetClassDetailsAsync(
        Guid schoolId,
        Guid classId)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.Id == classId &&
                    x.SchoolId == schoolId)
                .Get();


        var schoolClass =
            classResponse.Models
                .FirstOrDefault();


        if (schoolClass == null)
        {
            return null;
        }


        // -----------------------------------------------------
        // CLASS TEACHER
        // -----------------------------------------------------

        var teacherName =
            "Not assigned";

        var teacherEmployeeId =
            "";


        if (schoolClass.ClassTeacherId.HasValue)
        {
            var teacherResponse =
                await _supabase
                    .From<TeacherRecord>()
                    .Where(x =>
                        x.Id ==
                            schoolClass.ClassTeacherId.Value &&
                        x.SchoolId ==
                            schoolId)
                    .Get();


            var teacher =
                teacherResponse.Models
                    .FirstOrDefault();


            if (teacher != null)
            {
                teacherEmployeeId =
                    teacher.EmployeeId ?? "";


                var userResponse =
                    await _supabase
                        .From<UserProfile>()
                        .Where(x =>
                            x.Id ==
                                teacher.UserId &&
                            x.SchoolId ==
                                schoolId)
                        .Get();


                var user =
                    userResponse.Models
                        .FirstOrDefault();


                if (user != null &&
                    !string.IsNullOrWhiteSpace(
                        user.FullName))
                {
                    teacherName =
                        user.FullName;
                }
            }
        }


        // -----------------------------------------------------
        // ENROLLED LEARNERS
        // -----------------------------------------------------

        var enrollmentResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Where(x =>
                    x.ClassId == classId &&
                    x.IsActive == true)
                .Get();


        var enrollments =
            enrollmentResponse.Models
                .Where(x =>
                    x.AcademicYear ==
                        schoolClass.AcademicYear)
                .ToList();


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var learners =
            learnerResponse.Models;


        var enrolledLearners =
            new List<ClassEnrolledLearnerViewModel>();


        foreach (var enrollment in enrollments)
        {
            var learner =
                learners.FirstOrDefault(
                    x =>
                        x.Id ==
                        enrollment.LearnerId);


            if (learner == null)
            {
                continue;
            }


            enrolledLearners.Add(
                new ClassEnrolledLearnerViewModel
                {
                    LearnerId =
                        learner.Id,

                    AdmissionNumber =
                        learner.AdmissionNumber,

                    FullName =
                        learner.FullName,

                    Grade =
                        learner.Grade,

                    Gender =
                        learner.Gender ?? "",

                    EnrollmentDate =
                        enrollment.EnrollmentDate,

                    IsActive =
                        learner.IsActive
                });
        }


        enrolledLearners =
            enrolledLearners
                .OrderBy(x =>
                    x.FullName)
                .ToList();


        // -----------------------------------------------------
        // RETURN VIEW MODEL
        // -----------------------------------------------------

        return new ClassDetailsViewModel
        {
            Id =
                schoolClass.Id,

            Name =
                schoolClass.Name,

            Code =
                schoolClass.Code,

            Grade =
                schoolClass.Grade,

            Section =
                schoolClass.Section ?? "",

            RoomNumber =
                schoolClass.RoomNumber ?? "",

            Capacity =
                schoolClass.Capacity,

            AcademicYear =
                schoolClass.AcademicYear,

            IsActive =
                schoolClass.IsActive,

            ClassTeacherName =
                teacherName,

            ClassTeacherEmployeeId =
                teacherEmployeeId,

            CreatedAt =
                schoolClass.CreatedAt,

            UpdatedAt =
                schoolClass.UpdatedAt,

            EnrolledLearners =
                enrolledLearners
        };
    }

    //
    // Method Name      : GetClassForEditAsync
    // Purpose          : Retrieves class For Edit.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid classId
    // Output Type      : Task<EditClassViewModel?>.
    //

    public async Task<EditClassViewModel?>
    GetClassForEditAsync(
        Guid schoolId,
        Guid classId)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.Id == classId &&
                    x.SchoolId == schoolId)
                .Get();


        var schoolClass =
            classResponse.Models
                .FirstOrDefault();


        if (schoolClass == null)
        {
            return null;
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var teachers =
            teacherResponse.Models;


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var users =
            userResponse.Models;


        var teacherOptions =
            new List<ClassTeacherOptionViewModel>();


        foreach (var teacher in teachers)
        {
            var user =
                users.FirstOrDefault(
                    x =>
                        x.Id == teacher.UserId);


            if (user == null)
            {
                continue;
            }


            var isCurrentTeacher =
                schoolClass.ClassTeacherId.HasValue &&
                schoolClass.ClassTeacherId.Value ==
                    teacher.Id;


            /*
             * Normally only active teachers are selectable.
             * But if the currently assigned teacher later
             * became inactive, still show them so opening
             * Edit Class does not silently remove them.
             */
            if (!teacher.IsActive ||
                !user.IsActive)
            {
                if (!isCurrentTeacher)
                {
                    continue;
                }
            }


            var statusSuffix =
                teacher.IsActive &&
                user.IsActive
                    ? ""
                    : " - Inactive";


            teacherOptions.Add(
                new ClassTeacherOptionViewModel
                {
                    Id =
                        teacher.Id,

                    DisplayName =
                        $"{user.FullName} ({teacher.EmployeeId}){statusSuffix}"
                });
        }


        return new EditClassViewModel
        {
            Id =
                schoolClass.Id,

            Name =
                schoolClass.Name,

            Code =
                schoolClass.Code,

            Grade =
                schoolClass.Grade,

            Section =
                schoolClass.Section,

            ClassTeacherId =
                schoolClass.ClassTeacherId,

            RoomNumber =
                schoolClass.RoomNumber,

            Capacity =
                schoolClass.Capacity,

            AcademicYear =
                schoolClass.AcademicYear,

            Teachers =
                teacherOptions
                    .OrderBy(x =>
                        x.DisplayName)
                    .ToList()
        };
    }

    //
    // Method Name      : UpdateClassAsync
    // Purpose          : Updates class.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; EditClassViewModel model
    // Output Type      : Task<bool>.
    //

    public async Task<bool> UpdateClassAsync(
    Guid schoolId,
    EditClassViewModel model)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.Id == model.Id &&
                    x.SchoolId == schoolId)
                .Get();


        var existingClass =
            classResponse.Models
                .FirstOrDefault();


        if (existingClass == null)
        {
            return false;
        }


        /*
         * Validate teacher selection.
         *
         * A newly selected teacher must belong to this
         * school and must be active.
         *
         * The only exception is the teacher who is
         * already assigned to this class, because they
         * may have become inactive after assignment.
         */
        if (model.ClassTeacherId.HasValue)
        {
            var selectedTeacherId =
                model.ClassTeacherId.Value;


            if (existingClass.ClassTeacherId !=
                selectedTeacherId)
            {
                var teacherResponse =
                    await _supabase
                        .From<TeacherRecord>()
                        .Where(x =>
                            x.Id ==
                                selectedTeacherId &&
                            x.SchoolId ==
                                schoolId &&
                            x.IsActive == true)
                        .Get();


                var teacher =
                    teacherResponse.Models
                        .FirstOrDefault();


                if (teacher == null)
                {
                    throw new InvalidOperationException(
                        "The selected teacher is not available.");
                }


                var userResponse =
                    await _supabase
                        .From<UserProfile>()
                        .Where(x =>
                            x.Id ==
                                teacher.UserId &&
                            x.SchoolId ==
                                schoolId &&
                            x.IsActive == true)
                        .Get();


                if (!userResponse.Models.Any())
                {
                    throw new InvalidOperationException(
                        "The selected teacher account is inactive.");
                }
            }
        }


        await _supabase
            .From<SchoolClassRecord>()
            .Where(x =>
                x.Id == model.Id &&
                x.SchoolId == schoolId)

            .Set(
                x => x.Name,
                model.Name.Trim())

            .Set(
                x => x.Code,
                model.Code
                    .Trim()
                    .ToUpperInvariant())

            .Set(
                x => x.Grade,
                model.Grade.Trim())

            .Set(
                x => x.Section,
                string.IsNullOrWhiteSpace(
                    model.Section)
                    ? null
                    : model.Section.Trim())

            .Set(
                x => x.ClassTeacherId,
                model.ClassTeacherId)

            .Set(
                x => x.RoomNumber,
                string.IsNullOrWhiteSpace(
                    model.RoomNumber)
                    ? null
                    : model.RoomNumber.Trim())

            .Set(
                x => x.Capacity,
                model.Capacity)

            .Set(
                x => x.AcademicYear,
                model.AcademicYear)

            .Set(
                x => x.UpdatedAt,
                DateTime.UtcNow)

            .Update();


        return true;
    }

    //
    // Method Name      : DeactivateClassAsync
    // Purpose          : Deactivates class.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid classId
    // Output Type      : Task<bool>.
    //

    public async Task<bool> DeactivateClassAsync(
    Guid schoolId,
    Guid classId)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.Id == classId &&
                    x.SchoolId == schoolId)
                .Get();


        var schoolClass =
            classResponse.Models
                .FirstOrDefault();


        if (schoolClass == null)
        {
            return false;
        }


        if (!schoolClass.IsActive)
        {
            return true;
        }


        await _supabase
            .From<SchoolClassRecord>()
            .Where(x =>
                x.Id == classId &&
                x.SchoolId == schoolId)

            .Set(
                x => x.IsActive,
                false)

            .Set(
                x => x.UpdatedAt,
                DateTime.UtcNow)

            .Update();


        return true;
    }


    //
    // Method Name      : ReactivateClassAsync
    // Purpose          : Reactivates class.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid classId
    // Output Type      : Task<bool>.
    //

    public async Task<bool> ReactivateClassAsync(
        Guid schoolId,
        Guid classId)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var classResponse =
            await _supabase
                .From<SchoolClassRecord>()
                .Where(x =>
                    x.Id == classId &&
                    x.SchoolId == schoolId)
                .Get();


        var schoolClass =
            classResponse.Models
                .FirstOrDefault();


        if (schoolClass == null)
        {
            return false;
        }


        if (schoolClass.IsActive)
        {
            return true;
        }


        await _supabase
            .From<SchoolClassRecord>()
            .Where(x =>
                x.Id == classId &&
                x.SchoolId == schoolId)

            .Set(
                x => x.IsActive,
                true)

            .Set(
                x => x.UpdatedAt,
                DateTime.UtcNow)

            .Update();


        return true;
    }
}