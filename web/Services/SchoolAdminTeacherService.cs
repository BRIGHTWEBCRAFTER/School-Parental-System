using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminTeacherService
// Purpose          : Encapsulates school admin teacher operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminTeacherService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;


    //
    // Method Name      : SchoolAdminTeacherService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminTeacherService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    //
    // Method Name      : GetTeachersAsync
    // Purpose          : Retrieves teachers.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId
    // Output Type      : Task<TeachersPageViewModel>.
    //

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


    //
    // Method Name      : AddTeacherAsync
    // Purpose          : Invite a Teacher through Supabase
    //                    using a temporary email PIN.
    // Re-use           : School Admin Add Teacher page.
    // Input Parameters : AddTeacherViewModel, Guid schoolId.
    // Output Type      : Task.
    //

    public async Task AddTeacherAsync(
        AddTeacherViewModel model,
        Guid schoolId)
    {
        //
        // STEP 1: RESTORE SCHOOL ADMIN SESSION
        //

        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException(
                "Your session has expired.");
        }

        var accessToken =
            _supabase.Auth.CurrentSession?.AccessToken;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new UnauthorizedAccessException(
                "A valid School Admin session is required.");
        }

        //
        // STEP 2: CREATE INVITATION REQUEST
        //

        var body = new Dictionary<string, object>
        {
            ["full_name"] =
                model.FullName.Trim(),

            ["email"] =
                model.Email.Trim().ToLowerInvariant(),

            ["role"] =
                "Teacher",

            ["school_id"] =
                schoolId.ToString()
        };

        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            body["phone"] = model.Phone.Trim();
        }

        if (!string.IsNullOrWhiteSpace(model.Specialization))
        {
            body["specialization"] =
                model.Specialization.Trim();
        }

        if (model.JoiningDate.HasValue)
        {
            body["joining_date"] =
                model.JoiningDate.Value.ToString("yyyy-MM-dd");
        }

        if (model.DateOfBirth.HasValue)
        {
            body["date_of_birth"] =
                model.DateOfBirth.Value.ToString("yyyy-MM-dd");
        }

        if (!string.IsNullOrWhiteSpace(model.Gender))
        {
            body["gender"] = model.Gender.Trim();
        }

        if (!string.IsNullOrWhiteSpace(model.AlternativePhone))
        {
            body["alternative_phone"] =
                model.AlternativePhone.Trim();
        }

        if (!string.IsNullOrWhiteSpace(model.Address))
        {
            body["address"] = model.Address.Trim();
        }

        //
        // STEP 3: INVOKE SUPABASE EDGE FUNCTION
        //

        var options =
            new Supabase.Functions.Client.InvokeFunctionOptions
            {
                Headers = new Dictionary<string, string>
                {
                    ["Authorization"] =
                        $"Bearer {accessToken}"
                },

                Body = body
            };

        var responseJson =
            await _supabase.Functions.Invoke(
                "invite-school-user",
                options: options);

        //
        // STEP 4: VERIFY INVITATION RESULT
        //

        using var document =
            System.Text.Json.JsonDocument.Parse(responseJson);

        var result = document.RootElement;

        var success =
            result.TryGetProperty("success", out var successValue) &&
            successValue.ValueKind ==
                System.Text.Json.JsonValueKind.True;

        var invitationSent =
            result.TryGetProperty(
                "invitation_sent",
                out var sentValue) &&
            sentValue.ValueKind ==
                System.Text.Json.JsonValueKind.True;

        if (!success || !invitationSent)
        {
            throw new InvalidOperationException(
                "Supabase could not confirm that " +
                "the temporary PIN email was requested.");
        }

    } // end method


    //
    // Method Name      : GetTeacherDetailsAsync
    // Purpose          : Retrieves teacher Details.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid teacherId
    // Output Type      : Task<TeacherDetailsViewModel?>.
    //

    public async Task<TeacherDetailsViewModel?>
    GetTeacherDetailsAsync(
        Guid schoolId,
        Guid teacherId)
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
                .Where(x =>
                    x.Id == teacherId &&
                    x.SchoolId == schoolId)
                .Get();


        var teacher =
            teacherResponse.Models
                .FirstOrDefault();


        if (teacher == null)
        {
            return null;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == teacher.UserId)
                .Get();


        var user =
            userResponse.Models
                .FirstOrDefault();


        if (user == null)
        {
            return null;
        }


        return new TeacherDetailsViewModel
        {
            Id =
                teacher.Id,

            EmployeeId =
                teacher.EmployeeId,

            FullName =
                user.FullName ?? "",

            Email =
                user.Email ?? "",

            Phone =
                user.Phone ?? "",

            DateOfBirth =
                teacher.DateOfBirth,

            Gender =
                teacher.Gender ?? "",

            Specialization =
                teacher.Specialization ?? "",

            JoiningDate =
                teacher.JoiningDate,

            AlternativePhone =
                teacher.AlternativePhone ?? "",

            Address =
                teacher.Address ?? "",

            IsActive =
                teacher.IsActive &&
                user.IsActive
        };
    }

    //
    // Method Name      : GetTeacherForEditAsync
    // Purpose          : Retrieves teacher For Edit.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid teacherId
    // Output Type      : Task<EditTeacherViewModel?>.
    //

    public async Task<EditTeacherViewModel?>
    GetTeacherForEditAsync(
        Guid schoolId,
        Guid teacherId)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException();
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.Id == teacherId &&
                    x.SchoolId == schoolId)
                .Get();

        var teacher =
            teacherResponse.Models.FirstOrDefault();

        if (teacher == null)
        {
            return null;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == teacher.UserId &&
                    x.SchoolId == schoolId)
                .Get();

        var user =
            userResponse.Models.FirstOrDefault();

        if (user == null)
        {
            return null;
        }


        return new EditTeacherViewModel
        {
            Id = teacher.Id,
            EmployeeId = teacher.EmployeeId,
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            Phone = user.Phone,
            Specialization = teacher.Specialization,
            JoiningDate = teacher.JoiningDate,
            DateOfBirth = teacher.DateOfBirth,
            Gender = teacher.Gender,
            AlternativePhone = teacher.AlternativePhone,
            Address = teacher.Address
        };
    }

    //
    // Method Name      : UpdateTeacherAsync
    // Purpose          : Updates teacher.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid teacherId;
    //                    EditTeacherViewModel model
    // Output Type      : Task<bool>.
    //

    public async Task<bool> UpdateTeacherAsync(
    Guid schoolId,
    Guid teacherId,
    EditTeacherViewModel model)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException();
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.Id == teacherId &&
                    x.SchoolId == schoolId)
                .Get();

        var teacher =
            teacherResponse.Models.FirstOrDefault();

        if (teacher == null)
        {
            return false;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == teacher.UserId &&
                    x.SchoolId == schoolId)
                .Get();

        var user =
            userResponse.Models.FirstOrDefault();

        if (user == null)
        {
            return false;
        }


        await _supabase
            .From<UserProfile>()
            .Where(x => x.Id == teacher.UserId)
            .Set(
                x => x.FullName,
                model.FullName.Trim())
            .Set(
                x => x.Phone,
                string.IsNullOrWhiteSpace(model.Phone)
                    ? null
                    : model.Phone.Trim())
            .Update();


        await _supabase
            .From<TeacherRecord>()
            .Where(x => x.Id == teacherId)
            .Set(
                x => x.Specialization,
                string.IsNullOrWhiteSpace(
                    model.Specialization)
                    ? null
                    : model.Specialization.Trim())
            .Set(
                x => x.JoiningDate,
                model.JoiningDate)
            .Set(
                x => x.DateOfBirth,
                model.DateOfBirth)
            .Set(
                x => x.Gender,
                string.IsNullOrWhiteSpace(model.Gender)
                    ? null
                    : model.Gender.Trim())
            .Set(
                x => x.AlternativePhone,
                string.IsNullOrWhiteSpace(
                    model.AlternativePhone)
                    ? null
                    : model.AlternativePhone.Trim())
            .Set(
                x => x.Address,
                string.IsNullOrWhiteSpace(model.Address)
                    ? null
                    : model.Address.Trim())
            .Update();


        return true;
    }

    //
    // Method Name      : SetTeacherActiveStatusAsync
    // Purpose          : Sets teacher Active Status.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid teacherId; bool isActive
    // Output Type      : Task<bool>.
    //

    public async Task<bool> SetTeacherActiveStatusAsync(
    Guid schoolId,
    Guid teacherId,
    bool isActive)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException();
        }


        var teacherResponse =
            await _supabase
                .From<TeacherRecord>()
                .Where(x =>
                    x.Id == teacherId &&
                    x.SchoolId == schoolId)
                .Get();

        var teacher =
            teacherResponse.Models.FirstOrDefault();

        if (teacher == null)
        {
            return false;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == teacher.UserId &&
                    x.SchoolId == schoolId)
                .Get();

        var user =
            userResponse.Models.FirstOrDefault();

        if (user == null)
        {
            return false;
        }


        await _supabase
            .From<UserProfile>()
            .Where(x => x.Id == teacher.UserId)
            .Set(x => x.IsActive, isActive)
            .Update();


        try
        {
            await _supabase
                .From<TeacherRecord>()
                .Where(x => x.Id == teacherId)
                .Set(x => x.IsActive, isActive)
                .Update();
        }
        catch
        {
            await _supabase
                .From<UserProfile>()
                .Where(x => x.Id == teacher.UserId)
                .Set(x => x.IsActive, !isActive)
                .Update();

            throw;
        }


        return true;
    }
}