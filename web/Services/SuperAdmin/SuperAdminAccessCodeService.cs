using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace SchoolParentalSystem.Web.Services.SuperAdmin;


//
// Class Name       : SuperAdminAccessCodeService
// Purpose          : Encapsulates super admin access code operations for
//                    the School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminAccessCodeService
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _sessionService;


    //
    // Method Name      : SuperAdminAccessCodeService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminAccessCodeService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    // =====================================================
    // LOAD ACCESS CODES PAGE
    // =====================================================

    //
    // Method Name      : GetAccessCodesPageAsync
    // Purpose          : Retrieves access Codes Page.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<SuperAdminAccessCodeViewModel>.
    //

    public async Task<SuperAdminAccessCodeViewModel>
        GetAccessCodesPageAsync()
    {
        await EnsureSessionAsync();


        var response =
            await _supabase
                .From<AccessCodeSchoolRecord>()
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
                    new AccessCodeSchoolItemViewModel
                    {
                        Id = school.Id,
                        Name = school.Name ?? ""
                    })
                .ToList();


        return new SuperAdminAccessCodeViewModel
        {
            Schools = activeSchools
        };
    }


    // =====================================================
    // GENERATE SCHOOL ADMIN REGISTRATION CODE
    // =====================================================

    //
    // Method Name      : GenerateRegistrationCodeAsync
    // Purpose          : Generates registration Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid schoolId; Guid createdByUserId
    // Output Type      : Task<GeneratedRegistrationCodeResult>.
    //

    public async Task<GeneratedRegistrationCodeResult>
        GenerateRegistrationCodeAsync(
            Guid schoolId,
            Guid createdByUserId)
    {
        await EnsureSessionAsync();


        // -------------------------------------------------
        // VALIDATE SCHOOL ID
        // -------------------------------------------------

        if (schoolId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Please select a school.");
        }


        // -------------------------------------------------
        // LOAD SELECTED SCHOOL
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<AccessCodeSchoolRecord>()
                .Where(school =>
                    school.Id == schoolId)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        if (school == null)
        {
            throw new InvalidOperationException(
                "The selected school could not be found.");
        }


        if (!string.Equals(
                school.Status,
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Registration codes can only be generated for active schools.");
        }


        // -------------------------------------------------
        // GENERATE READABLE CODE
        //
        // Example:
        // GOL-482731
        // -------------------------------------------------

        var readableCode =
            GenerateReadableCode(
                school.Name ?? "SCH");


        // -------------------------------------------------
        // HASH CODE USING SUPABASE
        // -------------------------------------------------

        var hashParameters =
            new Dictionary<string, object>
            {
                {
                    "plain_code",
                    readableCode
                }
            };


        var hashResponse =
            await _supabase.Rpc(
                "hash_registration_code",
                hashParameters);


        if (hashResponse == null ||
            string.IsNullOrWhiteSpace(
                hashResponse.Content))
        {
            throw new InvalidOperationException(
                "The registration code could not be secured.");
        }


        var codeHash =
            hashResponse.Content
                .Trim()
                .Trim('"');


        if (string.IsNullOrWhiteSpace(
                codeHash))
        {
            throw new InvalidOperationException(
                "The registration code could not be secured.");
        }


        // -------------------------------------------------
        // EXPIRY
        // -------------------------------------------------

        var expiresAt =
            DateTime.UtcNow.AddDays(7);


        // -------------------------------------------------
        // CREATE NEW DATABASE RECORD
        // -------------------------------------------------

        var newRecord =
            new AdminRegistrationCodeRecord
            {
                Id =
                    Guid.NewGuid(),

                CodeHash =
                    codeHash,

                AdminRole =
                    "School Admin",

                SchoolId =
                    schoolId,

                ExpiresAt =
                    expiresAt,

                IsActive =
                    true,

                UsedAt =
                    null,

                UsedBy =
                    null,

                CreatedBy =
                    createdByUserId,

                CreatedAt =
                    DateTime.UtcNow
            };


        var insertResponse =
            await _supabase
                .From<AdminRegistrationCodeRecord>()
                .Insert(newRecord);


        var insertedRecord =
            insertResponse.Models
                .FirstOrDefault();


        if (insertedRecord == null)
        {
            throw new InvalidOperationException(
                "The registration code could not be saved.");
        }


        // -------------------------------------------------
        // DEACTIVATE PREVIOUS UNUSED CODES
        // FOR THE SAME SCHOOL
        // -------------------------------------------------

        var existingCodesResponse =
            await _supabase
                .From<AdminRegistrationCodeRecord>()
                .Where(code =>
                    code.SchoolId == schoolId)
                .Get();


        foreach (var oldCode
                 in existingCodesResponse.Models)
        {
            if (oldCode.Id ==
                insertedRecord.Id)
            {
                continue;
            }


            if (!string.Equals(
                    oldCode.AdminRole,
                    "School Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }


            if (!oldCode.IsActive)
            {
                continue;
            }


            if (oldCode.UsedAt.HasValue)
            {
                continue;
            }


            await _supabase
                .From<AdminRegistrationCodeRecord>()
                .Where(code =>
                    code.Id == oldCode.Id)
                .Set(
                    code =>
                        code.IsActive,
                    false)
                .Update();
        }


        // -------------------------------------------------
        // RETURN READABLE CODE ONCE
        // -------------------------------------------------

        return new GeneratedRegistrationCodeResult
        {
            Code =
                readableCode,

            SchoolName =
                school.Name ?? "",

            SchoolEmail =
                school.Email ?? "",

            ExpiresAt =
                expiresAt
        };
    }


    // =====================================================
    // GENERATE READABLE CODE
    // =====================================================

    //
    // Method Name      : GenerateReadableCode
    // Purpose          : Generates readable Code.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string schoolName
    // Output Type      : string.
    //

    private static string GenerateReadableCode(
        string schoolName)
    {
        var cleanedSchoolName =
            Regex.Replace(
                    schoolName,
                    @"[^a-zA-Z]",
                    "")
                .ToUpperInvariant();


        var prefix =
            cleanedSchoolName.Length >= 3
                ? cleanedSchoolName[..3]
                : cleanedSchoolName
                    .PadRight(
                        3,
                        'X');


        var randomNumber =
            RandomNumberGenerator.GetInt32(
                100000,
                1000000);


        return
            $"{prefix}-{randomNumber}";
    }


    // =====================================================
    // ENSURE SUPABASE SESSION
    // =====================================================

    //
    // Method Name      : EnsureSessionAsync
    // Purpose          : Ensures session.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task.
    //

    private async Task EnsureSessionAsync()
    {
        var sessionRestored =
            await _sessionService
                .RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }
    }


    // =====================================================
    // SUPABASE SCHOOL RECORD
    // =====================================================

    //
    // Class Name       : AccessCodeSchoolRecord
    // Purpose          : Holds access code school record data used by the
    //                    application.
    // Re-use           : Used as typed data by related services and MVC
    //                    pages.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    [Table("schools")]
    private class AccessCodeSchoolRecord
        : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }


        [Column("name")]
        public string? Name { get; set; }


        [Column("email")]
        public string? Email { get; set; }


        [Column("status")]
        public string? Status { get; set; }
    }
}