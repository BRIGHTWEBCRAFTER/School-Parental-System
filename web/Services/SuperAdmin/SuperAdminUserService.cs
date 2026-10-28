using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace SchoolParentalSystem.Web.Services.SuperAdmin;


//
// Class Name       : SuperAdminUserService
// Purpose          : Encapsulates super admin user operations for the
//                    School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminUserService
{
    private readonly Supabase.Client
        _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SuperAdminUserService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminUserService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase =
            supabase;

        _sessionService =
            sessionService;
    }


    // =====================================================
    // USERS MANAGEMENT LIST
    // =====================================================

    //
    // Method Name      : GetUsersAsync
    // Purpose          : Retrieves users.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<UserManagementViewModel>.
    //

    public async Task<UserManagementViewModel>
        GetUsersAsync()
    {
        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        var usersResponse =
            await _supabase
                .From<UserRecord>()
                .Get();


        var users =
            usersResponse.Models;


        var schoolsResponse =
            await _supabase
                .From<SchoolRecord>()
                .Get();


        var schools =
            schoolsResponse.Models;


        var schoolLookup =
            schools.ToDictionary(
                x => x.Id,
                x => x.Name ?? "");


        var userItems =
            users
                .Select(user =>
                {
                    var schoolName = "";


                    if (user.SchoolId.HasValue &&
                        schoolLookup.TryGetValue(
                            user.SchoolId.Value,
                            out var foundSchool))
                    {
                        schoolName =
                            foundSchool;
                    }


                    return new SuperAdminUserItemViewModel
                    {
                        Id =
                            user.Id,

                        FullName =
                            user.FullName ?? "",

                        Email =
                            user.Email ?? "",

                        Phone =
                            user.Phone ?? "",

                        Role =
                            FormatRole(
                                user.Role),

                        SchoolName =
                            schoolName,

                        IsActive =
                            user.IsActive
                    };
                })
                .OrderBy(
                    x => x.FullName)
                .ToList();


        return new UserManagementViewModel
        {
            Users =
                userItems,

            TotalUsers =
                userItems.Count,

            ActiveUsers =
                userItems.Count(
                    x => x.IsActive),

            InactiveUsers =
                userItems.Count(
                    x => !x.IsActive),

            SchoolAdmins =
                userItems.Count(
                    x =>
                        x.Role.Equals(
                            "School Admin",
                            StringComparison.OrdinalIgnoreCase)),

            Teachers =
                userItems.Count(
                    x =>
                        x.Role.Equals(
                            "Teacher",
                            StringComparison.OrdinalIgnoreCase)),

            Parents =
                userItems.Count(
                    x =>
                        x.Role.Equals(
                            "Parent",
                            StringComparison.OrdinalIgnoreCase))
        };
    }


    // =====================================================
    // USER DETAILS
    // =====================================================

    //
    // Method Name      : GetUserDetailsAsync
    // Purpose          : Retrieves user Details.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id
    // Output Type      : Task<UserDetailsViewModel?>.
    //

    public async Task<UserDetailsViewModel?>
        GetUserDetailsAsync(
            Guid id)
    {
        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        var usersResponse =
            await _supabase
                .From<UserRecord>()
                .Where(
                    x =>
                        x.Id == id)
                .Get();


        var user =
            usersResponse.Models
                .FirstOrDefault();


        if (user == null)
        {
            return null;
        }


        var schoolName = "";


        if (user.SchoolId.HasValue)
        {
            var schoolResponse =
                await _supabase
                    .From<SchoolRecord>()
                    .Where(
                        x =>
                            x.Id ==
                            user.SchoolId.Value)
                    .Get();


            var school =
                schoolResponse.Models
                    .FirstOrDefault();


            if (school != null)
            {
                schoolName =
                    school.Name ?? "";
            }
        }


        return new UserDetailsViewModel
        {
            Id =
                user.Id,

            FullName =
                user.FullName ?? "",

            Email =
                user.Email ?? "",

            Phone =
                user.Phone ?? "",

            Role =
                FormatRole(
                    user.Role),

            SchoolName =
                schoolName,

            IsActive =
                user.IsActive,

            CreatedAt =
                user.CreatedAt
        };
    }


    // =====================================================
    // ACTIVATE / DEACTIVATE USER
    // =====================================================

    //
    // Method Name      : SetUserActiveStatusAsync
    // Purpose          : Sets user Active Status.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id; bool isActive
    // Output Type      : Task.
    //

    public async Task SetUserActiveStatusAsync(
        Guid id,
        bool isActive)
    {
        // -------------------------------------------------
        // RESTORE SUPER ADMIN SUPABASE SESSION
        // -------------------------------------------------

        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        // -------------------------------------------------
        // FIND USER
        // -------------------------------------------------

        var userResponse =
            await _supabase
                .From<UserRecord>()
                .Where(
                    x =>
                        x.Id == id)
                .Get();


        var user =
            userResponse.Models
                .FirstOrDefault();


        if (user == null)
        {
            throw new InvalidOperationException(
                "The selected user could not be found.");
        }


        // -------------------------------------------------
        // ALREADY IN REQUESTED STATE
        // -------------------------------------------------

        if (user.IsActive == isActive)
        {
            return;
        }


        // -------------------------------------------------
        // UPDATE SUPABASE
        // -------------------------------------------------

        await _supabase
            .From<UserRecord>()
            .Where(
                x =>
                    x.Id == id)
            .Set(
                x =>
                    x.IsActive,
                isActive)
            .Update();


        // -------------------------------------------------
        // VERIFY DATABASE UPDATE
        // -------------------------------------------------

        var verifyResponse =
            await _supabase
                .From<UserRecord>()
                .Where(
                    x =>
                        x.Id == id)
                .Get();


        var updatedUser =
            verifyResponse.Models
                .FirstOrDefault();


        if (updatedUser == null)
        {
            throw new InvalidOperationException(
                "The user could not be verified after updating the account status.");
        }


        if (updatedUser.IsActive != isActive)
        {
            throw new InvalidOperationException(
                "The account status was not updated in Supabase. Please check the users table RLS update policy.");
        }
    }


    // =====================================================
    // ROLE DISPLAY
    // =====================================================

    //
    // Method Name      : FormatRole
    // Purpose          : Formats role.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string? role
    // Output Type      : string.
    //

    private static string FormatRole(
        string? role)
    {
        if (string.IsNullOrWhiteSpace(
            role))
        {
            return "";
        }


        return role
            .Trim()
            .ToLowerInvariant()
            switch
        {
            "school_admin" =>
                "School Admin",

            "schooladmin" =>
                "School Admin",

            "school admin" =>
                "School Admin",

            "teacher" =>
                "Teacher",

            "parent" =>
                "Parent",

            "super_admin" =>
                "Super Admin",

            "superadmin" =>
                "Super Admin",

            "super admin" =>
                "Super Admin",

            _ =>
                role
        };
    }


    // =====================================================
    // SUPABASE USERS TABLE
    // =====================================================

    //
    // Class Name       : UserRecord
    // Purpose          : Holds user record data used by the application.
    // Re-use           : Used as typed data by related services and MVC
    //                    pages.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    [Table("users")]
    private class UserRecord : BaseModel
    {
        [PrimaryKey(
            "id",
            false)]
        public Guid Id { get; set; }


        [Column("full_name")]
        public string? FullName { get; set; }


        [Column("email")]
        public string? Email { get; set; }


        [Column("phone")]
        public string? Phone { get; set; }


        [Column("role")]
        public string? Role { get; set; }


        [Column("school_id")]
        public Guid? SchoolId { get; set; }


        [Column("is_active")]
        public bool IsActive { get; set; }


        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }


    // =====================================================
    // SUPABASE SCHOOLS TABLE
    // =====================================================

    //
    // Class Name       : SchoolRecord
    // Purpose          : Holds school record data used by the application.
    // Re-use           : Used as typed data by related services and MVC
    //                    pages.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    [Table("schools")]
    private class SchoolRecord : BaseModel
    {
        [PrimaryKey(
            "id",
            false)]
        public Guid Id { get; set; }


        [Column("name")]
        public string? Name { get; set; }
    }
}