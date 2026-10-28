using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

namespace SchoolParentalSystem.Web.Services.SuperAdmin;

//
// Class Name       : SuperAdminSchoolService
// Purpose          : Encapsulates super admin school operations for the
//                    School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminSchoolService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SuperAdminSchoolService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminSchoolService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    // -----------------------------------------------------
    // ADD SCHOOL
    // -----------------------------------------------------

    //
    // Method Name      : AddSchoolAsync
    // Purpose          : Adds school.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : AddSchoolViewModel model
    // Output Type      : Task.
    //

    public async Task AddSchoolAsync(
        AddSchoolViewModel model)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        if (string.IsNullOrWhiteSpace(
                model.SchoolName))
        {
            throw new InvalidOperationException(
                "School name is required.");
        }


        if (string.IsNullOrWhiteSpace(
                model.Address))
        {
            throw new InvalidOperationException(
                "School address is required.");
        }


        var school =
            new SuperAdminSchoolRecord
            {
                Id =
                    Guid.NewGuid(),

                Name =
                    model.SchoolName.Trim(),

                Address =
                    model.Address.Trim(),

                Phone =
                    string.IsNullOrWhiteSpace(
                        model.Phone)
                        ? null
                        : model.Phone.Trim(),

                Email =
                    string.IsNullOrWhiteSpace(
                        model.Email)
                        ? null
                        : model.Email
                            .Trim()
                            .ToLowerInvariant(),

                SchoolCode =
                    string.IsNullOrWhiteSpace(
                        model.SchoolCode)
                        ? null
                        : model.SchoolCode
                            .Trim()
                            .ToUpperInvariant(),

                AdmissionPrefix =
                    string.IsNullOrWhiteSpace(
                        model.AdmissionPrefix)
                        ? null
                        : model.AdmissionPrefix
                            .Trim()
                            .ToUpperInvariant(),

                SchoolType =
                    string.IsNullOrWhiteSpace(
                        model.SchoolType)
                        ? null
                        : model.SchoolType,

                Province =
                    string.IsNullOrWhiteSpace(
                        model.Province)
                        ? null
                        : model.Province,

                TownCity =
                    string.IsNullOrWhiteSpace(
                        model.TownCity)
                        ? null
                        : model.TownCity.Trim(),

                PrincipalName =
                    string.IsNullOrWhiteSpace(
                        model.PrincipalName)
                        ? null
                        : model.PrincipalName.Trim(),

                MaxLearners =
                    model.MaxLearners,

                Status =
                    string.IsNullOrWhiteSpace(
                        model.Status)
                        ? "Active"
                        : model.Status,

                Notes =
                    string.IsNullOrWhiteSpace(
                        model.Notes)
                        ? null
                        : model.Notes.Trim(),

                CreatedAt =
                    DateTime.UtcNow,

                UpdatedAt =
                    DateTime.UtcNow
            };


        await _supabase
            .From<SuperAdminSchoolRecord>()
            .Insert(school);
    }


    // -----------------------------------------------------
    // GET ALL SCHOOLS
    // -----------------------------------------------------

    //
    // Method Name      : GetSchoolsAsync
    // Purpose          : Retrieves schools.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<SchoolManagementViewModel>.
    //

    public async Task<SchoolManagementViewModel>
        GetSchoolsAsync()
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();


        var schools =
            schoolResponse.Models;


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Get();


        var learners =
            learnerResponse.Models;


        var model =
            new SchoolManagementViewModel
            {
                Schools =
                    schools
                        .OrderByDescending(x =>
                            x.CreatedAt)
                        .Select(x =>
                            new SuperAdminSchoolItemViewModel
                            {
                                Id =
                                    x.Id,

                                SchoolName =
                                    x.Name,

                                SchoolCode =
                                    x.SchoolCode ?? "",

                                SchoolType =
                                    x.SchoolType ?? "",

                                Province =
                                    x.Province ?? "",

                                TownCity =
                                    x.TownCity ?? "",

                                PrincipalName =
                                    x.PrincipalName ?? "",

                                Phone =
                                    x.Phone ?? "",

                                Email =
                                    x.Email ?? "",

                                LearnerCount =
                                    learners.Count(learner =>
                                        learner.IsActive &&
                                        learner.SchoolId ==
                                            x.Id),

                                Status =
                                    string.IsNullOrWhiteSpace(
                                        x.Status)
                                        ? "Pending"
                                        : x.Status,

                                CreatedAt =
                                    x.CreatedAt
                            })
                        .ToList()
            };


        return model;
    }


    // -----------------------------------------------------
    // SCHOOL DETAILS
    // -----------------------------------------------------

    //
    // Method Name      : GetSchoolDetailsAsync
    // Purpose          : Retrieves school Details.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id
    // Output Type      : Task<SchoolDetailsViewModel?>.
    //

    public async Task<SchoolDetailsViewModel?>
        GetSchoolDetailsAsync(
            Guid id)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Where(x =>
                    x.Id == id)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        if (school == null)
        {
            return null;
        }


        var learnerResponse =
            await _supabase
                .From<LearnerRecord>()
                .Where(x =>
                    x.SchoolId == id)
                .Get();


        var learnerCount =
            learnerResponse.Models
                .Count(x =>
                    x.IsActive);


        return new SchoolDetailsViewModel
        {
            Id =
                school.Id,

            SchoolName =
                school.Name,

            Address =
                school.Address,

            SchoolCode =
                school.SchoolCode ?? "",

            AdmissionPrefix =
                school.AdmissionPrefix ?? "",

            SchoolType =
                school.SchoolType ?? "",

            Province =
                school.Province ?? "",

            TownCity =
                school.TownCity ?? "",

            PrincipalName =
                school.PrincipalName ?? "",

            Phone =
                school.Phone ?? "",

            Email =
                school.Email ?? "",

            MaxLearners =
                school.MaxLearners,

            LearnerCount =
                learnerCount,

            Status =
                string.IsNullOrWhiteSpace(
                    school.Status)
                    ? "Pending"
                    : school.Status,

            Notes =
                school.Notes ?? "",

            CreatedAt =
                school.CreatedAt
        };
    }


    // -----------------------------------------------------
    // EDIT SCHOOL - LOAD
    // -----------------------------------------------------

    //
    // Method Name      : GetEditSchoolAsync
    // Purpose          : Retrieves edit School.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id
    // Output Type      : Task<EditSchoolViewModel?>.
    //

    public async Task<EditSchoolViewModel?>
        GetEditSchoolAsync(
            Guid id)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Where(x =>
                    x.Id == id)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        if (school == null)
        {
            return null;
        }


        return new EditSchoolViewModel
        {
            Id =
                school.Id,

            SchoolName =
                school.Name,

            Address =
                school.Address,

            SchoolCode =
                school.SchoolCode,

            AdmissionPrefix =
                school.AdmissionPrefix,

            SchoolType =
                school.SchoolType,

            Province =
                school.Province,

            TownCity =
                school.TownCity,

            PrincipalName =
                school.PrincipalName,

            Phone =
                school.Phone,

            Email =
                school.Email,

            MaxLearners =
                school.MaxLearners,

            Status =
                string.IsNullOrWhiteSpace(
                    school.Status)
                    ? "Active"
                    : school.Status,

            Notes =
                school.Notes
        };
    }
    // -----------------------------------------------------
    // EDIT SCHOOL - SAVE
    // -----------------------------------------------------

    //
    // Method Name      : UpdateSchoolAsync
    // Purpose          : Updates school.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : EditSchoolViewModel model
    // Output Type      : Task.
    //

    public async Task UpdateSchoolAsync(
        EditSchoolViewModel model)
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
                .From<SuperAdminSchoolRecord>()
                .Where(x => x.Id == model.Id)
                .Get();


        var school =
            response.Models.FirstOrDefault();


        if (school == null)
        {
            throw new InvalidOperationException(
                "School could not be found.");
        }


        school.Name =
            model.SchoolName.Trim();

        school.Address =
            model.Address.Trim();

        school.SchoolCode =
            string.IsNullOrWhiteSpace(model.SchoolCode)
                ? null
                : model.SchoolCode
                    .Trim()
                    .ToUpperInvariant();

        school.AdmissionPrefix =
            string.IsNullOrWhiteSpace(model.AdmissionPrefix)
                ? null
                : model.AdmissionPrefix
                    .Trim()
                    .ToUpperInvariant();

        school.SchoolType =
            string.IsNullOrWhiteSpace(model.SchoolType)
                ? null
                : model.SchoolType;

        school.Province =
            string.IsNullOrWhiteSpace(model.Province)
                ? null
                : model.Province;

        school.TownCity =
            string.IsNullOrWhiteSpace(model.TownCity)
                ? null
                : model.TownCity.Trim();

        school.PrincipalName =
            string.IsNullOrWhiteSpace(model.PrincipalName)
                ? null
                : model.PrincipalName.Trim();

        school.Phone =
            string.IsNullOrWhiteSpace(model.Phone)
                ? null
                : model.Phone.Trim();

        school.Email =
            string.IsNullOrWhiteSpace(model.Email)
                ? null
                : model.Email
                    .Trim()
                    .ToLowerInvariant();

        school.MaxLearners =
            model.MaxLearners;

        school.Status =
            string.IsNullOrWhiteSpace(model.Status)
                ? "Active"
                : model.Status;

        school.Notes =
            string.IsNullOrWhiteSpace(model.Notes)
                ? null
                : model.Notes.Trim();

        school.UpdatedAt =
            DateTime.UtcNow;


        await _supabase
            .From<SuperAdminSchoolRecord>()
            .Update(school);
    }
}