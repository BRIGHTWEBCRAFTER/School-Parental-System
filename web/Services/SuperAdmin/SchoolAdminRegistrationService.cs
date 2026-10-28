using System.Security.Cryptography;

using SchoolParentalSystem.Web.Models.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;


namespace SchoolParentalSystem.Web.Services.SuperAdmin;


//
// Class Name       : SchoolAdminRegistrationService
// Purpose          : Encapsulates school admin registration operations for
//                    the School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminRegistrationService
{
    private readonly Supabase.Client _supabase;

    private readonly RegistrationCodeEmailService
        _emailService;


    //
    // Method Name      : SchoolAdminRegistrationService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase;
    //                    RegistrationCodeEmailService emailService
    // Output Type      : Constructor.
    //

    public SchoolAdminRegistrationService(
        Supabase.Client supabase,
        RegistrationCodeEmailService emailService)
    {
        _supabase = supabase;

        _emailService = emailService;
    }


    // =====================================================
    // LOAD REGISTRATION PAGE
    // =====================================================

    //
    // Method Name      : GetRegistrationPageAsync
    // Purpose          : Retrieves registration Page.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<SchoolAdminRegistrationViewModel>.
    //

    public async Task<SchoolAdminRegistrationViewModel>
        GetRegistrationPageAsync()
    {
        var response =
            await _supabase
                .From<SchoolRegistrationSchoolRecord>()
                .Get();


        var activeSchools =
            response.Models
                .Where(school =>
                    string.Equals(
                        school.Status,
                        "Active",
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(school =>
                    school.Name ?? "")
                .Select(school =>
                    new SchoolRegistrationItemViewModel
                    {
                        Id = school.Id,

                        Name =
                            school.Name ?? ""
                    })
                .ToList();


        return new SchoolAdminRegistrationViewModel
        {
            Schools = activeSchools
        };
    }


    // =====================================================
    // VALIDATE REGISTRATION CODE
    // =====================================================

    //
    // Method Name      : ValidateRegistrationCodeAsync
    // Purpose          : Validates registration Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid schoolId; string registrationCode
    // Output Type      : Task<Guid?>.
    //

    public async Task<Guid?>
        ValidateRegistrationCodeAsync(
            Guid schoolId,
            string registrationCode)
    {
        if (schoolId == Guid.Empty)
        {
            return null;
        }


        if (string.IsNullOrWhiteSpace(
                registrationCode))
        {
            return null;
        }


        var parameters =
            new Dictionary<string, object>
            {
                {
                    "p_school_id",
                    schoolId
                },

                {
                    "p_code",
                    registrationCode.Trim()
                }
            };


        var response =
            await _supabase.Rpc(
                "validate_school_admin_registration_code",
                parameters);


        if (response == null ||
            string.IsNullOrWhiteSpace(
                response.Content))
        {
            return null;
        }


        var result =
            response.Content
                .Trim()
                .Trim('"');


        if (string.Equals(
                result,
                "null",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }


        if (!Guid.TryParse(
                result,
                out var registrationCodeId))
        {
            return null;
        }


        return registrationCodeId;
    }


    // =====================================================
    // CREATE SCHOOL ADMIN ACCOUNT
    // =====================================================

    //
    // Method Name      : RegisterSchoolAdminAsync
    // Purpose          : Handles register School Admin operations.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : SchoolAdminRegistrationViewModel model
    // Output Type      : Task<SchoolAdminRegistrationResult>.
    //

    public async Task<SchoolAdminRegistrationResult>
        RegisterSchoolAdminAsync(
            SchoolAdminRegistrationViewModel model)
    {
        if (!model.SchoolId.HasValue ||
            model.SchoolId.Value == Guid.Empty)
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "Please select your school."
            };
        }


        if (string.IsNullOrWhiteSpace(
                model.Email))
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "Please enter your email address."
            };
        }


        var schoolId =
            model.SchoolId.Value;


        var email =
            model.Email
                .Trim()
                .ToLowerInvariant();


        // -------------------------------------------------
        // VALIDATE REGISTRATION CODE AGAIN
        // -------------------------------------------------

        var registrationCodeId =
            await ValidateRegistrationCodeAsync(
                schoolId,
                model.RegistrationCode);


        if (!registrationCodeId.HasValue)
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The registration code is invalid, expired, already used, or does not belong to the selected school."
            };
        }


        // -------------------------------------------------
        // LOAD SCHOOL
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<SchoolRegistrationSchoolRecord>()
                .Where(
                    school =>
                        school.Id == schoolId)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        if (school == null)
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The selected school could not be found."
            };
        }


        if (!string.Equals(
                school.Status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The selected school is not currently active."
            };
        }


        // -------------------------------------------------
        // CREATE SUPABASE AUTH ACCOUNT
        // -------------------------------------------------

        Supabase.Gotrue.Session? signUpResult = null;

        try
        {
            signUpResult =
                await _supabase.Auth
                    .SignUp(
                        email,
                        model.Password);
        }
        catch (Supabase.Gotrue.Exceptions.GotrueException ex)
        {
            if (ex.Message.Contains(
                    "over_email_send_rate_limit",
                    StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains(
                    "email rate limit exceeded",
                    StringComparison.OrdinalIgnoreCase))
            {
                return new SchoolAdminRegistrationResult
                {
                    Success = false,

                    Message =
                        "Too many verification emails have been requested. Please try again later."
                };
            }


            if (ex.Message.Contains(
                    "already registered",
                    StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains(
                    "user already registered",
                    StringComparison.OrdinalIgnoreCase))
            {
                return new SchoolAdminRegistrationResult
                {
                    Success = false,

                    Message =
                        "An account with this email address already exists. Please log in instead."
                };
            }


            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The School Admin account could not be created."
            };
        }


        if (signUpResult == null ||
            signUpResult.User == null ||
            string.IsNullOrWhiteSpace(
                signUpResult.User.Id))
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The School Admin account could not be created."
            };
        }


        if (!Guid.TryParse(
                signUpResult.User.Id,
                out var userId))
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The new School Admin account could not be verified."
            };
        }


        // -------------------------------------------------
        // GENERATE LOGIN ACCESS CODE
        // -------------------------------------------------

        var loginAccessCode =
            GenerateLoginAccessCode();


        // -------------------------------------------------
        // COMPLETE APPLICATION REGISTRATION
        // -------------------------------------------------

        var completeParameters =
            new Dictionary<string, object>
            {
                {
                    "p_code_id",
                    registrationCodeId.Value
                },

                {
                    "p_user_id",
                    userId
                },

                {
                    "p_full_name",
                    model.FullName.Trim()
                },

                {
                    "p_email",
                    email
                },

                {
                    "p_phone",
                    model.Phone?.Trim() ?? ""
                },

                {
                    "p_school_id",
                    schoolId
                },

                {
                    "p_login_access_code",
                    loginAccessCode
                }
            };


        var completeResponse =
            await _supabase.Rpc(
                "complete_school_admin_registration",
                completeParameters);


        if (completeResponse == null ||
            string.IsNullOrWhiteSpace(
                completeResponse.Content))
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The account was created, but the School Admin profile could not be completed."
            };
        }


        var completionResult =
            completeResponse.Content
                .Trim()
                .Trim('"');


        if (!bool.TryParse(
                completionResult,
                out var completed) ||
            !completed)
        {
            return new SchoolAdminRegistrationResult
            {
                Success = false,

                Message =
                    "The account was created, but registration could not be completed."
            };
        }


        // -------------------------------------------------
        // EMAIL LOGIN ACCESS CODE
        // -------------------------------------------------

        try
        {
            await _emailService
                .SendSchoolAdminLoginAccessCodeAsync(
                    email,
                    school.Name ?? "Your School",
                    model.FullName.Trim(),
                    loginAccessCode);
        }
        catch
        {
            // Registration itself has already completed.
            // Do not undo the account because the email
            // failed.

            return new SchoolAdminRegistrationResult
            {
                Success = true,

                Message =
                    "Your School Admin account was created, but the login access-code email could not be sent. Please contact the Super Admin."
            };
        }


        // -------------------------------------------------
        // CLEAR LOCAL AUTH STATE
        // -------------------------------------------------

        try
        {
            await _supabase.Auth.SignOut();
        }
        catch
        {
            // Ignore cleanup failure.
        }


        return new SchoolAdminRegistrationResult
        {
            Success = true,

            Message =
                "Your School Admin account has been created. Please verify your email address. Your School Admin login access code has also been sent to your email."
        };
    }


    // =====================================================
    // GENERATE LOGIN ACCESS CODE
    // =====================================================

    //
    // Method Name      : GenerateLoginAccessCode
    // Purpose          : Generates login Access Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : string.
    //

    private static string GenerateLoginAccessCode()
    {
        var number =
            RandomNumberGenerator.GetInt32(
                100000,
                1000000);


        return $"ADM-{number}";
    }
}


// =========================================================
// REGISTRATION RESULT
// =========================================================

//
// Class Name       : SchoolAdminRegistrationResult
// Purpose          : Holds school admin registration result data used by
//                    the application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

public class SchoolAdminRegistrationResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = "";
}