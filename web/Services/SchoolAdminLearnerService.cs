using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;
using System.Text.Json;
using Supabase.Postgrest;

namespace SchoolParentalSystem.Web.Services;

public class SchoolAdminLearnerService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;

    public SchoolAdminLearnerService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }

    public async Task<LearnersPageViewModel> GetLearnersAsync()
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }

        // RLS automatically limits these queries to what
        // the logged-in School Admin is allowed to see.

        var learnersResponse =
            await _supabase
                .From<LearnerRecord>()
                .Get();

        var enrollmentsResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Get();

        var classesResponse =
            await _supabase
                .From<ClassRecord>()
                .Get();

        var parentLearnersResponse =
            await _supabase
                .From<ParentLearnerRecord>()
                .Get();

        var parentsResponse =
            await _supabase
                .From<ParentRecord>()
                .Get();

        var usersResponse =
            await _supabase
                .From<UserProfile>()
                .Get();


        var learners =
            learnersResponse.Models;

        var enrollments =
            enrollmentsResponse.Models;

        var classes =
            classesResponse.Models;

        var parentLearners =
            parentLearnersResponse.Models;

        var parents =
            parentsResponse.Models;

        var users =
            usersResponse.Models;


        var learnerItems =
            learners
                .OrderBy(x => x.FullName)
                .Select(learner =>
                {
                    var enrollment =
                        enrollments
                            .Where(x =>
                                x.LearnerId == learner.Id &&
                                x.IsActive)
                            .OrderByDescending(x =>
                                x.AcademicYear)
                            .FirstOrDefault();


                    var learnerClass =
                        enrollment == null
                            ? null
                            : classes.FirstOrDefault(
                                x => x.Id ==
                                     enrollment.ClassId);


                    var parentLink =
                        parentLearners
                            .Where(x =>
                                x.LearnerId ==
                                learner.Id &&
                                x.IsActive)
                            .OrderByDescending(x =>
                                x.IsPrimaryGuardian)
                            .FirstOrDefault();


                    ParentRecord? parent = null;
                    UserProfile? guardianUser = null;


                    if (parentLink != null)
                    {
                        parent =
                            parents.FirstOrDefault(
                                x => x.Id ==
                                     parentLink.ParentId);

                        if (parent != null)
                        {
                            guardianUser =
                                users.FirstOrDefault(
                                    x => x.Id ==
                                         parent.UserId);
                        }
                    }


                    return new LearnerListItemViewModel
                    {
                        Id =
                            learner.Id.ToString(),

                        AdmissionNumber =
                            learner.AdmissionNumber,

                        FullName =
                            learner.FullName,

                        Grade =
                            learner.Grade ??
                            "Not assigned",

                        ClassName =
                            learnerClass?.Name ??
                            "Not assigned",

                        Gender =
                            learner.Gender ??
                            "Not specified",

                        GuardianName =
                            guardianUser?.FullName ??
                            "Not linked",

                        Status =
                            learner.IsActive
                                ? "Active"
                                : "Inactive"
                    };
                })
                .ToList();


        return new LearnersPageViewModel
        {
            TotalLearners =
                learners.Count,

            ActiveLearners =
                learners.Count(x =>
                    x.IsActive),

            InactiveLearners =
                learners.Count(x =>
                    !x.IsActive),

            Learners =
                learnerItems
        };
    }

    public async Task<SchoolAdminDashboardViewModel>
    GetDashboardAsync(
        Guid schoolId,
        string adminName)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // SCHOOL
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<DashboardSchoolRecord>()
                .Where(x => x.Id == schoolId)
                .Get();

        var school =
            schoolResponse.Models.FirstOrDefault();

        if (school == null)
        {
            throw new InvalidOperationException(
                "The school could not be found.");
        }


        // -------------------------------------------------
        // LEARNERS
        // -------------------------------------------------

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();

        var learners =
            learnerResponse.Models;

        var learnerIds =
            learners
                .Select(x => x.Id)
                .ToHashSet();


        // -------------------------------------------------
        // TEACHERS
        // -------------------------------------------------

        var teacherResponse =
            await _supabase
                .From<DashboardTeacherRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();

        var teachers =
            teacherResponse.Models
                .Where(x => x.IsActive)
                .ToList();


        // -------------------------------------------------
        // CLASSES
        // -------------------------------------------------

        var classResponse =
            await _supabase
                .From<ClassRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();

        var classes =
            classResponse.Models
                .Where(x => x.IsActive)
                .ToList();

        var classIds =
            classes
                .Select(x => x.Id)
                .ToHashSet();


        // -------------------------------------------------
        // ATTENDANCE
        // -------------------------------------------------

        var attendanceResponse =
            await _supabase
                .From<DashboardAttendanceRecord>()
                .Get();

        var attendance =
            attendanceResponse.Models
                .Where(x =>
                    learnerIds.Contains(x.LearnerId))
                .ToList();


        var presentCount =
            attendance.Count(x =>
                string.Equals(
                    x.Status,
                    "Present",
                    StringComparison.OrdinalIgnoreCase));

        var attendanceRate =
            attendance.Count == 0
                ? 0m
                : Math.Round(
                    (decimal)presentCount /
                    attendance.Count * 100m,
                    1);


        var attendanceOverview =
            attendance
                .GroupBy(x =>
                    x.AttendanceDate.Date)
                .OrderByDescending(x => x.Key)
                .Take(5)
                .OrderBy(x => x.Key)
                .Select(group =>
                {
                    var total =
                        group.Count();

                    var present =
                        group.Count(x =>
                            string.Equals(
                                x.Status,
                                "Present",
                                StringComparison.OrdinalIgnoreCase));

                    var percentage =
                        total == 0
                            ? 0m
                            : Math.Round(
                                (decimal)present /
                                total * 100m,
                                1);

                    return new AttendanceOverviewItem
                    {
                        Date =
                            group.Key.ToString("MMM dd"),

                        Percentage =
                            percentage
                    };
                })
                .ToList();


        // -------------------------------------------------
        // ANNOUNCEMENTS
        // -------------------------------------------------

        var announcementResponse =
            await _supabase
                .From<DashboardAnnouncementRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();

        var announcements =
            announcementResponse.Models;

        var now =
            DateTime.Now;

        var activeAnnouncements =
            announcements.Count(x =>
                !x.ExpiresAt.HasValue ||
                x.ExpiresAt.Value >= now);


        var recentAnnouncements =
            announcements
                .OrderByDescending(x => x.CreatedAt)
                .Take(3)
                .Select(x =>
                    new AnnouncementItem
                    {
                        Title =
                            x.Title,

                        Description =
                            ShortenDashboardText(
                                x.Content,
                                90)
                    })
                .ToList();


        // -------------------------------------------------
        // EVENTS
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<DashboardEventRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();

        var events =
            eventResponse.Models;

        var today =
            DateTime.Today;


        var upcomingEvents =
            events
                .Where(x =>
                    x.EventDate.Date >= today)
                .OrderBy(x => x.EventDate)
                .Take(3)
                .Select(x =>
                    new EventItem
                    {
                        Month =
                            x.EventDate
                                .ToString("MMM")
                                .ToUpperInvariant(),

                        Day =
                            x.EventDate
                                .ToString("dd"),

                        Title =
                            x.EventName
                    })
                .ToList();


        // -------------------------------------------------
        // ENROLLMENTS
        // -------------------------------------------------

        var enrollmentResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Get();

        var enrollments =
            enrollmentResponse.Models
                .Where(x =>
                    x.IsActive &&
                    learnerIds.Contains(x.LearnerId) &&
                    classIds.Contains(x.ClassId))
                .ToList();


        // -------------------------------------------------
        // GRADES
        // -------------------------------------------------

        var gradeResponse =
            await _supabase
                .From<DashboardGradeRecord>()
                .Get();

        var grades =
            gradeResponse.Models
                .Where(x =>
                    learnerIds.Contains(x.LearnerId) &&
                    classIds.Contains(x.ClassId))
                .ToList();


        var classPerformance =
            classes
                .Select(schoolClass =>
                {
                    var classEnrollments =
                        enrollments
                            .Where(x =>
                                x.ClassId == schoolClass.Id)
                            .ToList();

                    var classGrades =
                        grades
                            .Where(x =>
                                x.ClassId == schoolClass.Id)
                            .ToList();

                    var average =
                        classGrades.Count == 0
                            ? 0m
                            : Math.Round(
                                classGrades.Average(x => x.Score),
                                1);


                    string status;

                    if (classGrades.Count == 0)
                    {
                        status =
                            "No grade data";
                    }
                    else if (average >= 90)
                    {
                        status =
                            "Excellent";
                    }
                    else if (average >= 75)
                    {
                        status =
                            "Good";
                    }
                    else
                    {
                        status =
                            "Needs attention";
                    }


                    return new ClassPerformanceItem
                    {
                        ClassName =
                            schoolClass.Name,

                        LearnerCount =
                            classEnrollments
                                .Select(x => x.LearnerId)
                                .Distinct()
                                .Count(),

                        AverageGrade =
                            average,

                        Status =
                            status
                    };
                })
                .ToList();


        // -------------------------------------------------
        // RECENT ACTIVITY
        // -------------------------------------------------

        var learnerActivityResponse =
            await _supabase
                .From<DashboardLearnerActivityRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();


        var recentActivityData =
            new List<(DateTime Date,
                      string Description,
                      string Type)>();


        recentActivityData.AddRange(
            learnerActivityResponse.Models
                .Select(x =>
                    (
                        x.CreatedAt,
                        $"{x.FullName} was registered as a learner.",
                        "learner"
                    )));


        recentActivityData.AddRange(
            attendance
                .Select(x =>
                    (
                        x.CreatedAt,
                        "Learner attendance was updated.",
                        "attendance"
                    )));


        recentActivityData.AddRange(
            grades
                .Select(x =>
                    (
                        x.CreatedAt,
                        "A learner grade was recorded.",
                        "grade"
                    )));


        recentActivityData.AddRange(
            announcements
                .Select(x =>
                    (
                        x.CreatedAt,
                        $"Announcement posted: {x.Title}",
                        "announcement"
                    )));


        recentActivityData.AddRange(
            events
                .Select(x =>
                    (
                        x.CreatedAt,
                        $"Event created: {x.EventName}",
                        "event"
                    )));


        var recentActivities =
            recentActivityData
                .OrderByDescending(x => x.Date)
                .Take(5)
                .Select(x =>
                    new ActivityItem
                    {
                        Description =
                            x.Description,

                        Type =
                            x.Type,

                        TimeAgo =
                            FormatDashboardTimeAgo(
                                x.Date)
                    })
                .ToList();


        // -------------------------------------------------
        // FINAL VIEW MODEL
        // -------------------------------------------------

        return new SchoolAdminDashboardViewModel
        {
            AdminName =
                string.IsNullOrWhiteSpace(adminName)
                    ? "School Admin"
                    : adminName,

            SchoolName =
                school.Name,

            TotalLearners =
                learners.Count,

            TotalTeachers =
                teachers.Count,

            TotalClasses =
                classes.Count,

            AttendanceRate =
                attendanceRate,

            ActiveAnnouncements =
                activeAnnouncements,

            AttendanceOverview =
                attendanceOverview,

            RecentAnnouncements =
                recentAnnouncements,

            UpcomingEvents =
                upcomingEvents,

            RecentActivities =
                recentActivities,

            ClassPerformance =
                classPerformance
        };
    }

    private static string ShortenDashboardText(
    string? value,
    int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "No description provided.";
        }

        var text =
            value.Trim();

        if (text.Length <= maximumLength)
        {
            return text;
        }

        return text[..maximumLength].TrimEnd() + "...";
    }


    private static string FormatDashboardTimeAgo(
        DateTime date)
    {
        var elapsed =
            DateTime.Now - date;

        if (elapsed.TotalMinutes < 1)
        {
            return "Just now";
        }

        if (elapsed.TotalMinutes < 60)
        {
            return $"{(int)elapsed.TotalMinutes}m ago";
        }

        if (elapsed.TotalHours < 24)
        {
            return $"{(int)elapsed.TotalHours}h ago";
        }

        if (elapsed.TotalDays < 7)
        {
            return $"{(int)elapsed.TotalDays}d ago";
        }

        return date.ToString("dd MMM yyyy");
    }

    public async Task<List<ClassOptionViewModel>>
    GetAvailableClassesAsync(Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }

        var response =
            await _supabase
                .From<ClassRecord>()
                .Get();

        return response.Models
            .Where(x =>
                x.SchoolId == schoolId &&
                x.IsActive)
            .OrderBy(x => x.Grade)
            .ThenBy(x => x.Name)
            .Select(x =>
                new ClassOptionViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Grade = x.Grade ?? ""
                })
            .ToList();
    }

    public async Task<Guid> AddLearnerAsync(
    AddLearnerViewModel model,
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
        // VALIDATE CLASS
        // -------------------------------------------------

        ClassRecord? selectedClass = null;

        if (model.ClassId.HasValue)
        {
            var classResponse =
                await _supabase
                    .From<ClassRecord>()
                    .Where(x =>
                        x.Id == model.ClassId.Value)
                    .Get();

            selectedClass =
                classResponse.Models
                    .FirstOrDefault();

            if (selectedClass == null ||
                selectedClass.SchoolId != schoolId ||
                !selectedClass.IsActive)
            {
                throw new InvalidOperationException(
                    "The selected class is not valid.");
            }

            if (!string.Equals(
                    selectedClass.Grade,
                    model.Grade,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The selected class does not match the learner's grade.");
            }
        }


        // -------------------------------------------------
        // VALIDATE PARENT / GUARDIAN
        // -------------------------------------------------

        if (!model.ParentId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a parent or guardian.");
        }

        var parentResponse =
            await _supabase
                .From<ParentRecord>()
                .Where(x =>
                    x.Id == model.ParentId.Value)
                .Get();

        var selectedParent =
            parentResponse.Models
                .FirstOrDefault();

        if (selectedParent == null ||
            !selectedParent.IsActive)
        {
            throw new InvalidOperationException(
                "The selected parent or guardian is not valid.");
        }


        var parentUserResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == selectedParent.UserId)
                .Get();

        var parentUser =
            parentUserResponse.Models
                .FirstOrDefault();

        if (parentUser == null ||
            !parentUser.IsActive ||
            parentUser.Role != "Parent" ||
            parentUser.SchoolId != schoolId)
        {
            throw new InvalidOperationException(
                "The selected parent or guardian does not belong to this school.");
        }


        var allowedRelationships =
            new[]
            {
            "Mother",
            "Father",
            "Guardian",
            "Grandparent",
            "Other"
            };

        if (string.IsNullOrWhiteSpace(
                model.Relationship) ||
            !allowedRelationships.Contains(
                model.Relationship))
        {
            throw new InvalidOperationException(
                "Please select a valid relationship.");
        }


        // -------------------------------------------------
        // INSERT LEARNER
        // -------------------------------------------------

        var learner =
            new LearnerRecord
            {
                // Do NOT generate Id here.
                // PostgreSQL generates it.

                AdmissionNumber = "",

                FullName =
                    model.FullName.Trim(),

                DateOfBirth =
                    model.DateOfBirth,

                Gender =
                    string.IsNullOrWhiteSpace(
                        model.Gender)
                        ? null
                        : model.Gender.Trim(),

                SchoolId =
                    schoolId,

                Grade =
                    model.Grade.Trim(),

                Address =
                    string.IsNullOrWhiteSpace(
                        model.Address)
                        ? null
                        : model.Address.Trim(),

                IsActive =
                    true
            };


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Insert(
                    learner,
                    new QueryOptions
                    {
                        Returning =
                            QueryOptions
                                .ReturnType
                                .Representation
                    });


        var insertedLearner =
            learnerResponse.Models
                .FirstOrDefault();


        if (insertedLearner == null ||
            insertedLearner.Id == Guid.Empty)
        {
            throw new InvalidOperationException(
                "The learner was created, but the generated learner ID could not be retrieved.");
        }


        // THIS is the real PostgreSQL-generated ID.
        var learnerId =
            insertedLearner.Id;


        // -------------------------------------------------
        // CREATE CLASS ENROLLMENT
        // -------------------------------------------------

        if (selectedClass != null)
        {
            var enrollment =
                new EnrollmentRecord
                {
                    LearnerId =
                        learnerId,

                    ClassId =
                        selectedClass.Id,

                    EnrollmentDate =
                        DateTime.Today,

                    AcademicYear =
                        selectedClass.AcademicYear,

                    EndDate =
                        null,

                    IsActive =
                        true
                };


            await _supabase
                .From<EnrollmentRecord>()
                .Insert(enrollment);
        }


        // -------------------------------------------------
        // LINK REGISTERED PARENT / GUARDIAN
        // -------------------------------------------------

        var parentLearner =
            new ParentLearnerRecord
            {
                ParentId =
                    selectedParent.Id,

                LearnerId =
                    learnerId,

                Relationship =
                    model.Relationship.Trim(),

                IsPrimaryGuardian =
                    true,

                IsActive =
                    true
            };


        await _supabase
            .From<ParentLearnerRecord>()
            .Insert(parentLearner);


        return learnerId;
    }

    public async Task<List<ParentSearchResultViewModel>>
    SearchParentsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) ||
            query.Trim().Length < 2)
        {
            return [];
        }

        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }

        var response =
            await _supabase.Rpc(
                "search_school_parents",
                new Dictionary<string, object>
                {
                    ["p_query"] = query.Trim(),
                    ["p_limit"] = 8
                });

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            return [];
        }

        return JsonSerializer.Deserialize<
                   List<ParentSearchResultViewModel>>(
                   response.Content)
               ?? [];
    }
    public async Task<ParentSearchResultViewModel?>
    GetParentSelectionAsync(
        Guid parentId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var parentResponse =
            await _supabase
                .From<ParentRecord>()
                .Where(x => x.Id == parentId)
                .Get();

        var parent =
            parentResponse.Models
                .FirstOrDefault();

        if (parent == null ||
            !parent.IsActive)
        {
            return null;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x => x.Id == parent.UserId)
                .Get();

        var user =
            userResponse.Models
                .FirstOrDefault();

        if (user == null ||
            !user.IsActive ||
            user.Role != "Parent" ||
            user.SchoolId != schoolId)
        {
            return null;
        }


        return new ParentSearchResultViewModel
        {
            ParentId =
                parent.Id,

            GuardianNumber =
                parent.GuardianNumber,

            FullName =
                user.FullName,

            Phone =
                user.Phone,

            Email =
                user.Email
        };
    }

    public async Task<LearnerDetailsViewModel?>
    GetLearnerDetailsAsync(
        Guid learnerId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // ---------------------------------------------
        // LEARNER
        // ---------------------------------------------

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x => x.Id == learnerId)
                .Get();

        var learner =
            learnerResponse.Models
                .FirstOrDefault();

        if (learner == null)
        {
            return null;
        }

        // Extra school check in addition to RLS.
        if (learner.SchoolId != schoolId)
        {
            return null;
        }


        var model =
            new LearnerDetailsViewModel
            {
                Id =
                    learner.Id,

                AdmissionNumber =
                    learner.AdmissionNumber,

                FullName =
                    learner.FullName,

                DateOfBirth =
                    learner.DateOfBirth,

                Gender =
                    learner.Gender ?? "",

                Grade =
                    learner.Grade,

                Address =
                    learner.Address ?? "",

                IsActive =
                    learner.IsActive
            };


        // ---------------------------------------------
        // ACTIVE CLASS ENROLLMENT
        // ---------------------------------------------

        var enrollmentResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Where(x =>
                    x.LearnerId == learnerId &&
                    x.IsActive)
                .Get();

        var enrollment =
            enrollmentResponse.Models
                .FirstOrDefault();


        if (enrollment != null)
        {
            var classResponse =
                await _supabase
                    .From<ClassRecord>()
                    .Where(x =>
                        x.Id == enrollment.ClassId)
                    .Get();

            var learnerClass =
                classResponse.Models
                    .FirstOrDefault();

            if (learnerClass != null &&
                learnerClass.SchoolId == schoolId)
            {
                model.ClassName =
                    learnerClass.Name;
            }
        }


        // ---------------------------------------------
        // REGISTERED PARENT / GUARDIAN
        // ---------------------------------------------

        var linkResponse =
            await _supabase
                .From<ParentLearnerRecord>()
                .Where(x =>
                    x.LearnerId == learnerId &&
                    x.IsActive)
                .Get();

        var parentLink =
            linkResponse.Models
                .FirstOrDefault();


        if (parentLink != null)
        {
            var parentResponse =
                await _supabase
                    .From<ParentRecord>()
                    .Where(x =>
                        x.Id == parentLink.ParentId)
                    .Get();

            var parent =
                parentResponse.Models
                    .FirstOrDefault();


            if (parent != null)
            {
                var userResponse =
                    await _supabase
                        .From<UserProfile>()
                        .Where(x =>
                            x.Id == parent.UserId)
                        .Get();

                var parentUser =
                    userResponse.Models
                        .FirstOrDefault();


                if (parentUser != null &&
                    parentUser.SchoolId == schoolId)
                {
                    model.GuardianName =
                        parentUser.FullName;

                    model.GuardianNumber =
                        parent.GuardianNumber;

                    model.GuardianPhone =
                        parentUser.Phone ?? "";

                    model.GuardianEmail =
                        parentUser.Email ?? "";

                    model.Relationship =
                        parentLink.Relationship;
                }
            }
        }


        return model;
    }

    public async Task<EditLearnerViewModel?>
    GetEditLearnerAsync(
        Guid learnerId,
        Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x => x.Id == learnerId)
                .Get();

        var learner =
            learnerResponse.Models
                .FirstOrDefault();

        if (learner == null ||
            learner.SchoolId != schoolId)
        {
            return null;
        }


        var model =
            new EditLearnerViewModel
            {
                Id =
                    learner.Id,

                AdmissionNumber =
                    learner.AdmissionNumber,

                FullName =
                    learner.FullName,

                DateOfBirth =
                    learner.DateOfBirth,

                Gender =
                    learner.Gender ?? "",

                Grade =
                    learner.Grade,

                Address =
                    learner.Address,

                IsActive =
                    learner.IsActive,

                AvailableClasses =
                    await GetAvailableClassesAsync(
                        schoolId)
            };


        // Current class
        var enrollmentResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Where(x =>
                    x.LearnerId == learnerId &&
                    x.IsActive)
                .Get();

        var enrollment =
            enrollmentResponse.Models
                .FirstOrDefault();

        if (enrollment != null)
        {
            model.ClassId =
                enrollment.ClassId;
        }


        // Current registered parent
        var linkResponse =
            await _supabase
                .From<ParentLearnerRecord>()
                .Where(x =>
                    x.LearnerId == learnerId &&
                    x.IsActive)
                .Get();

        var link =
            linkResponse.Models
                .FirstOrDefault();


        if (link != null)
        {
            model.ParentId =
                link.ParentId;

            model.Relationship =
                link.Relationship;

            var parent =
                await GetParentSelectionAsync(
                    link.ParentId,
                    schoolId);

            if (parent != null)
            {
                model.SelectedParentName =
                    parent.FullName;

                model.SelectedGuardianNumber =
                    parent.GuardianNumber;

                model.SelectedParentPhone =
                    parent.Phone ?? "";

                model.SelectedParentEmail =
                    parent.Email ?? "";
            }
        }


        return model;
    }
    public async Task UpdateLearnerAsync(
    EditLearnerViewModel model,
    Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // ---------------------------------------------
        // VALIDATE LEARNER
        // ---------------------------------------------

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x => x.Id == model.Id)
                .Get();

        var learner =
            learnerResponse.Models
                .FirstOrDefault();

        if (learner == null ||
            learner.SchoolId != schoolId)
        {
            throw new InvalidOperationException(
                "The learner could not be found.");
        }


        // ---------------------------------------------
        // VALIDATE CLASS
        // ---------------------------------------------

        ClassRecord? selectedClass = null;

        if (model.ClassId.HasValue)
        {
            var classResponse =
                await _supabase
                    .From<ClassRecord>()
                    .Where(x =>
                        x.Id == model.ClassId.Value)
                    .Get();

            selectedClass =
                classResponse.Models
                    .FirstOrDefault();

            if (selectedClass == null ||
                !selectedClass.IsActive ||
                selectedClass.SchoolId != schoolId)
            {
                throw new InvalidOperationException(
                    "The selected class is not valid.");
            }

            if (!string.Equals(
                    selectedClass.Grade,
                    model.Grade,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The selected class does not match the learner's grade.");
            }
        }


        // ---------------------------------------------
        // VALIDATE PARENT
        // ---------------------------------------------

        if (!model.ParentId.HasValue)
        {
            throw new InvalidOperationException(
                "Please select a parent or guardian.");
        }

        var selectedParent =
            await GetParentSelectionAsync(
                model.ParentId.Value,
                schoolId);

        if (selectedParent == null)
        {
            throw new InvalidOperationException(
                "The selected parent or guardian is not valid.");
        }


        var allowedRelationships =
            new[]
            {
            "Mother",
            "Father",
            "Guardian",
            "Grandparent",
            "Other"
            };

        if (string.IsNullOrWhiteSpace(
                model.Relationship) ||
            !allowedRelationships.Contains(
                model.Relationship))
        {
            throw new InvalidOperationException(
                "Please select a valid relationship.");
        }


        // ---------------------------------------------
        // UPDATE LEARNER
        // ---------------------------------------------

        await _supabase
            .From<LearnerRecord>()
            .Where(x => x.Id == model.Id)

            .Set(
                x => x.FullName,
                model.FullName.Trim())

            .Set(
                x => x.DateOfBirth,
                model.DateOfBirth)

            .Set(
                x => x.Gender,
                model.Gender.Trim())

            .Set(
                x => x.Grade,
                model.Grade.Trim())

            .Set(
                x => x.Address,
                string.IsNullOrWhiteSpace(model.Address)
                    ? null
                    : model.Address.Trim())

            .Update();


        // ---------------------------------------------
        // UPDATE CLASS ENROLLMENT
        // ---------------------------------------------

        var enrollmentResponse =
            await _supabase
                .From<EnrollmentRecord>()
                .Where(x =>
                    x.LearnerId == model.Id &&
                    x.IsActive)
                .Get();

        var enrollment =
            enrollmentResponse.Models
                .FirstOrDefault();


        if (selectedClass != null)
        {
            if (enrollment != null)
            {
                await _supabase
                    .From<EnrollmentRecord>()
                    .Where(x =>
                        x.Id == enrollment.Id)

                    .Set(
                        x => x.ClassId,
                        selectedClass.Id)

                    .Set(
                        x => x.AcademicYear,
                        selectedClass.AcademicYear)

                    .Set(
                        x => x.IsActive,
                        true)

                    .Set(
                        x => x.EndDate,
                        (DateTime?)null)

                    .Update();
            }
            else
            {
                var newEnrollment =
                    new EnrollmentRecord
                    {
                        LearnerId =
                            model.Id,

                        ClassId =
                            selectedClass.Id,

                        EnrollmentDate =
                            DateTime.Today,

                        AcademicYear =
                            selectedClass.AcademicYear,

                        EndDate =
                            null,

                        IsActive =
                            true
                    };

                await _supabase
                    .From<EnrollmentRecord>()
                    .Insert(newEnrollment);
            }
        }
        else if (enrollment != null)
        {
            await _supabase
                .From<EnrollmentRecord>()
                .Where(x =>
                    x.Id == enrollment.Id)

                .Set(
                    x => x.IsActive,
                    false)

                .Set(
                    x => x.EndDate,
                    DateTime.Today)

                .Update();
        }


        // ---------------------------------------------
        // UPDATE PARENT LINK
        // ---------------------------------------------

        // Query ANY link, including an inactive one.
        // learner_id is unique in parent_learners.
        var linkResponse =
            await _supabase
                .From<ParentLearnerRecord>()
                .Where(x =>
                    x.LearnerId == model.Id)
                .Get();

        var existingLink =
            linkResponse.Models
                .FirstOrDefault();


        if (existingLink != null)
        {
            await _supabase
                .From<ParentLearnerRecord>()
                .Where(x =>
                    x.Id == existingLink.Id)

                .Set(
                    x => x.ParentId,
                    model.ParentId.Value)

                .Set(
                    x => x.Relationship,
                    model.Relationship.Trim())

                .Set(
                    x => x.IsPrimaryGuardian,
                    true)

                .Set(
                    x => x.IsActive,
                    true)

                .Update();
        }
        else
        {
            var newLink =
                new ParentLearnerRecord
                {
                    ParentId =
                        model.ParentId.Value,

                    LearnerId =
                        model.Id,

                    Relationship =
                        model.Relationship.Trim(),

                    IsPrimaryGuardian =
                        true,

                    IsActive =
                        true
                };

            await _supabase
                .From<ParentLearnerRecord>()
                .Insert(newLink);
        }
    }

    public async Task SetLearnerActiveStatusAsync(
    Guid learnerId,
    Guid schoolId,
    bool isActive)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }

        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x => x.Id == learnerId)
                .Get();

        var learner =
            learnerResponse.Models
                .FirstOrDefault();

        if (learner == null ||
            learner.SchoolId != schoolId)
        {
            throw new InvalidOperationException(
                "The learner could not be found.");
        }

        await _supabase
            .From<LearnerRecord>()
            .Where(x => x.Id == learnerId)
            .Set(
                x => x.IsActive,
                isActive)
            .Update();
    }


}