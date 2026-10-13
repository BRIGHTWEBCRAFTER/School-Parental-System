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
}