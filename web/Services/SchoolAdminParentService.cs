using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;
using static Supabase.Functions.Client;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminParentService
// Purpose          : Encapsulates school admin parent operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminParentService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SchoolAdminParentService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminParentService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    //
    // Method Name      : GetParentsAsync
    // Purpose          : Retrieves parents.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId
    // Output Type      : Task<ParentsPageViewModel>.
    //

    public async Task<ParentsPageViewModel>
        GetParentsAsync(Guid schoolId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        /*
         * Load parent application users for this school.
         */
        var usersResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.SchoolId == schoolId)
                .Get();


        var parentUsers =
            usersResponse.Models
                .Where(x =>
                    string.Equals(
                        x.Role,
                        "Parent",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();


        /*
         * Parent-specific records.
         *
         * RLS already limits what the logged-in
         * School Admin is allowed to access.
         */
        var parentsResponse =
            await _supabase
                .From<ParentRecord>()
                .Get();


        var parents =
            parentsResponse.Models;


        var items =
            new List<ParentListItemViewModel>();


        foreach (var parentUser in parentUsers)
        {
            var parent =
                parents.FirstOrDefault(
                    x => x.UserId == parentUser.Id);


            if (parent == null)
            {
                continue;
            }


            items.Add(
                new ParentListItemViewModel
                {
                    Id =
                        parent.Id,

                    GuardianNumber =
                        parent.GuardianNumber ?? "",

                    FullName =
                        parentUser.FullName ?? "",

                    Email =
                        parentUser.Email ?? "",

                    Phone =
                        parentUser.Phone ?? "",

                    Gender =
                        parent.Gender ?? "",

                    IsActive =
                        parent.IsActive &&
                        parentUser.IsActive
                });
        }


        items =
            items
                .OrderBy(x => x.FullName)
                .ToList();


        return new ParentsPageViewModel
        {
            TotalParents =
                items.Count,

            ActiveParents =
                items.Count(x => x.IsActive),

            InactiveParents =
                items.Count(x => !x.IsActive),

            Parents =
                items
        };
    }

    //
    // Method Name      : AddParentAsync
    // Purpose          : Adds parent.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; AddParentViewModel model
    // Output Type      : Task.
    //

    public async Task AddParentAsync(
    Guid schoolId,
    AddParentViewModel model)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var accessToken =
            _supabase.Auth.CurrentSession?.AccessToken;


        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new UnauthorizedAccessException(
                "A valid Supabase session is required.");
        }


        var body =
            new Dictionary<string, object>
            {
                ["full_name"] =
                    model.FullName.Trim(),

                ["email"] =
                    model.Email.Trim().ToLowerInvariant(),

                ["role"] =
                    "Parent",

                ["school_id"] =
                    schoolId.ToString()
            };


        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            body["phone"] =
                model.Phone.Trim();
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

        Body = body
    };



        var responseJson =
            await _supabase.Functions.Invoke(
                "invite-school-user",
                options: options);

        // Verify that Supabase accepted the invitation.
        using var responseDocument =
            System.Text.Json.JsonDocument.Parse(
                responseJson);

        var result = responseDocument.RootElement;

        var successful =
            result.TryGetProperty(
                "success",
                out var successValue)
            && successValue.ValueKind ==
                System.Text.Json.JsonValueKind.True;

        var invitationSent =
            result.TryGetProperty(
                "invitation_sent",
                out var invitationValue)
            && invitationValue.ValueKind ==
                System.Text.Json.JsonValueKind.True;

        if (!successful || !invitationSent)
        {
            throw new InvalidOperationException(
                "Supabase did not confirm that " +
                "the invitation was requested.");
        }

    }

    //
    // Method Name      : GetParentDetailsAsync
    // Purpose          : Retrieves parent Details.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid parentId
    // Output Type      : Task<ParentDetailsViewModel?>.
    //

    public async Task<ParentDetailsViewModel?>
    GetParentDetailsAsync(
        Guid schoolId,
        Guid parentId)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------
        // Parent record
        // -------------------------------

        var parentResponse =
            await _supabase
                .From<ParentRecord>()
                .Where(x =>
                    x.Id == parentId)
                .Get();


        var parent =
            parentResponse.Models
                .FirstOrDefault();


        if (parent == null)
        {
            return null;
        }


        // -------------------------------
        // Parent user profile
        // -------------------------------

        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == parent.UserId)
                .Get();


        var user =
            userResponse.Models
                .FirstOrDefault();


        if (user == null)
        {
            return null;
        }


        /*
         * Parent records do not contain school_id.
         * The school belongs to the public.users profile.
         */
        if (user.SchoolId != schoolId)
        {
            return null;
        }


        if (!string.Equals(
                user.Role,
                "Parent",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }


        // -------------------------------
        // Parent -> learner links
        // -------------------------------

        var linksResponse =
            await _supabase
                .From<ParentLearnerRecord>()
                .Where(x =>
                    x.ParentId == parentId)
                .Get();


        var links =
            linksResponse.Models
                .Where(x => x.IsActive)
                .ToList();


        var linkedLearners =
            new List<ParentLinkedLearnerViewModel>();


        foreach (var link in links)
        {
            var learnerResponse =
                await _supabase
                    .From<LearnerRecord>()
                    .Where(x =>
                        x.Id == link.LearnerId &&
                        x.SchoolId == schoolId)
                    .Get();


            var learner =
                learnerResponse.Models
                    .FirstOrDefault();


            if (learner == null)
            {
                continue;
            }


            linkedLearners.Add(
                new ParentLinkedLearnerViewModel
                {
                    LearnerId =
                        learner.Id,

                    AdmissionNumber =
                        learner.AdmissionNumber ?? "",

                    FullName =
                        learner.FullName ?? "",

                    Grade =
                        learner.Grade ?? "",

                    Relationship =
                        link.Relationship ?? "",

                    IsActive =
                        learner.IsActive
                });
        }


        linkedLearners =
            linkedLearners
                .OrderBy(x => x.FullName)
                .ToList();


        return new ParentDetailsViewModel
        {
            Id =
                parent.Id,

            GuardianNumber =
                parent.GuardianNumber ?? "",

            FullName =
                user.FullName ?? "",

            Email =
                user.Email ?? "",

            Phone =
                user.Phone ?? "",

            AlternativePhone =
                parent.AlternativePhone ?? "",

            Gender =
                parent.Gender ?? "",

            Address =
                parent.Address ?? "",

            ContactLastVerified =
                parent.ContactLastVerified,

            IsActive =
                parent.IsActive &&
                user.IsActive,

            LinkedLearners =
                linkedLearners
        };
    }

    //
    // Method Name      : GetParentForEditAsync
    // Purpose          : Retrieves parent For Edit.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid parentId
    // Output Type      : Task<EditParentViewModel?>.
    //

    public async Task<EditParentViewModel?>
    GetParentForEditAsync(
        Guid schoolId,
        Guid parentId)
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
                .Where(x =>
                    x.Id == parentId)
                .Get();


        var parent =
            parentResponse.Models
                .FirstOrDefault();


        if (parent == null)
        {
            return null;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == parent.UserId &&
                    x.SchoolId == schoolId)
                .Get();


        var user =
            userResponse.Models
                .FirstOrDefault();


        if (user == null ||
            !string.Equals(
                user.Role,
                "Parent",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }


        return new EditParentViewModel
        {
            Id =
                parent.Id,

            GuardianNumber =
                parent.GuardianNumber ?? "",

            FullName =
                user.FullName ?? "",

            Email =
                user.Email ?? "",

            Phone =
                user.Phone,

            Gender =
                parent.Gender,

            AlternativePhone =
                parent.AlternativePhone,

            Address =
                parent.Address
        };
    }

    //
    // Method Name      : UpdateParentAsync
    // Purpose          : Updates parent.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid parentId; EditParentViewModel
    //                    model
    // Output Type      : Task<bool>.
    //

    public async Task<bool> UpdateParentAsync(
    Guid schoolId,
    Guid parentId,
    EditParentViewModel model)
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
                .Where(x =>
                    x.Id == parentId)
                .Get();


        var parent =
            parentResponse.Models
                .FirstOrDefault();


        if (parent == null)
        {
            return false;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == parent.UserId &&
                    x.SchoolId == schoolId)
                .Get();


        var user =
            userResponse.Models
                .FirstOrDefault();


        if (user == null ||
            !string.Equals(
                user.Role,
                "Parent",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }


        var requestedEmail =
            model.Email
                .Trim()
                .ToLowerInvariant();


        var currentEmail =
            (user.Email ?? "")
                .Trim()
                .ToLowerInvariant();


        // =====================================================
        // UPDATE EMAIL THROUGH PROTECTED EDGE FUNCTION
        // =====================================================

        if (!string.Equals(
                currentEmail,
                requestedEmail,
                StringComparison.OrdinalIgnoreCase))
        {
            var accessToken =
                _supabase.Auth
                    .CurrentSession?
                    .AccessToken;


            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException(
                    "A valid Supabase session is required.");
            }


            var emailBody =
                new Dictionary<string, object>
                {
                    ["parent_id"] =
                        parentId.ToString(),

                    ["email"] =
                        requestedEmail
                };


            var emailOptions =
                new Supabase.Functions.Client
                    .InvokeFunctionOptions
                {
                    Headers =
                        new Dictionary<string, string>
                        {
                            ["Authorization"] =
                                $"Bearer {accessToken}"
                        },

                    Body =
                        emailBody
                };


            await _supabase
                .Functions
                .Invoke(
                    "update-school-user-email",
                    options: emailOptions);
        }


        // =====================================================
        // UPDATE GENERAL USER INFORMATION
        // =====================================================

        await _supabase
            .From<UserProfile>()
            .Where(x =>
                x.Id == parent.UserId)
            .Set(
                x => x.FullName,
                model.FullName.Trim())
            .Set(
                x => x.Phone,
                string.IsNullOrWhiteSpace(model.Phone)
                    ? null
                    : model.Phone.Trim())
            .Update();


        // =====================================================
        // UPDATE PARENT INFORMATION
        // =====================================================

        await _supabase
            .From<ParentRecord>()
            .Where(x =>
                x.Id == parentId)
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
    // Method Name      : SetParentActiveStatusAsync
    // Purpose          : Sets parent Active Status.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid schoolId; Guid parentId; bool isActive
    // Output Type      : Task<bool>.
    //

    public async Task<bool> SetParentActiveStatusAsync(
    Guid schoolId,
    Guid parentId,
    bool isActive)
    {
        var restored =
            await _sessionService.RestoreAsync();

        if (!restored)
        {
            throw new UnauthorizedAccessException();
        }


        var parentResponse =
            await _supabase
                .From<ParentRecord>()
                .Where(x => x.Id == parentId)
                .Get();

        var parent =
            parentResponse.Models.FirstOrDefault();

        if (parent == null)
        {
            return false;
        }


        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x =>
                    x.Id == parent.UserId &&
                    x.SchoolId == schoolId)
                .Get();

        var user =
            userResponse.Models.FirstOrDefault();

        if (user == null ||
            !string.Equals(
                user.Role,
                "Parent",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }


        await _supabase
            .From<UserProfile>()
            .Where(x => x.Id == parent.UserId)
            .Set(x => x.IsActive, isActive)
            .Update();


        try
        {
            await _supabase
                .From<ParentRecord>()
                .Where(x => x.Id == parentId)
                .Set(x => x.IsActive, isActive)
                .Update();
        }
        catch
        {
            await _supabase
                .From<UserProfile>()
                .Where(x => x.Id == parent.UserId)
                .Set(x => x.IsActive, !isActive)
                .Update();

            throw;
        }


        return true;
    }
}