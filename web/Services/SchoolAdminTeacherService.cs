using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

public class SchoolAdminTeacherService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;


    public SchoolAdminTeacherService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    public async Task<TeachersPageViewModel>
        GetTeachersAsync(Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();

        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get();


        var teachers =
            teacherResponse.Models;


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x => x.SchoolId == schoolId)
                .Get();


        var users =
            userResponse.Models
                .Where(x => x.Role == "Teacher")
                .ToDictionary(
                    x => x.Id,
                    x => x);


        var items =
            teachers
                .OrderBy(x => x.EmployeeId)
                .Select(teacher =>
                {
                    users.TryGetValue(
                        teacher.UserId,
                        out var user);

                    return new TeacherListItemViewModel
                    {
                        Id =
                            teacher.Id,

                        EmployeeId =
                            teacher.EmployeeId,

                        FullName =
                            user?.FullName ??
                            "Unknown Teacher",

                        Email =
                            user?.Email ??
                            "",

                        Phone =
                            user?.Phone ??
                            teacher.AlternativePhone ??
                            "",

                        Specialization =
                            teacher.Specialization ??
                            "Not specified",

                        JoiningDate =
                            teacher.JoiningDate,

                        IsActive =
                            teacher.IsActive &&
                            (user?.IsActive ?? false)
                    };
                })
                .ToList();


        return new TeachersPageViewModel
        {
            TotalTeachers =
                items.Count,

            ActiveTeachers =
                items.Count(x => x.IsActive),

            InactiveTeachers =
                items.Count(x => !x.IsActive),

            Teachers =
                items
        };
    }

    public async Task AddTeacherAsync(
    AddTeacherViewModel model,
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
        // GET CURRENT SCHOOL ADMIN ACCESS TOKEN
        // -------------------------------------------------

        var accessToken =
            _supabase.Auth.CurrentSession?.AccessToken;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new UnauthorizedAccessException(
                "Your Supabase session has expired.");
        }


        // -------------------------------------------------
        // BUILD EDGE FUNCTION REQUEST
        // -------------------------------------------------

        var body =
    new Dictionary<string, object>
    {
        ["full_name"] =
            model.FullName.Trim(),

        ["email"] =
            model.Email.Trim().ToLowerInvariant(),

        ["role"] =
            "Teacher"
    };

        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            body["phone"] =
                model.Phone.Trim();
        }


        if (!string.IsNullOrWhiteSpace(model.Specialization))
        {
            body["specialization"] =
                model.Specialization.Trim();
        }


        if (model.JoiningDate.HasValue)
        {
            body["joining_date"] =
                model.JoiningDate.Value
                    .ToString("yyyy-MM-dd");
        }


        if (model.DateOfBirth.HasValue)
        {
            body["date_of_birth"] =
                model.DateOfBirth.Value
                    .ToString("yyyy-MM-dd");
        }


        if (!string.IsNullOrWhiteSpace(model.Gender))
        {
            body["gender"] =
                model.Gender.Trim();
        }


        if (!string.IsNullOrWhiteSpace(
                model.AlternativePhone))
        {
            body["alternative_phone"] =
                model.AlternativePhone.Trim();
        }


        if (!string.IsNullOrWhiteSpace(model.Address))
        {
            body["address"] =
                model.Address.Trim();
        }


        var options =
            new Supabase.Functions.Client.InvokeFunctionOptions
            {
                Headers =
                    new Dictionary<string, string>
                    {
                        ["Authorization"] =
                            $"Bearer {accessToken}"
                    },

                Body =
                    body
            };


        // -------------------------------------------------
        // CREATE AUTH USER + USERS PROFILE + TEACHER PROFILE
        // -------------------------------------------------

        await _supabase.Functions.Invoke(
            "invite-school-user",
            options: options);
    }
}