using SchoolParentalSystem.Web.ViewModels.SuperAdmin;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Services.SuperAdmin;

//
// Class Name       : SuperAdminReportsService
// Purpose          : Encapsulates super admin reports operations for the
//                    School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminReportsService
{
    private readonly Supabase.Client _supabase;

    //
    // Method Name      : SuperAdminReportsService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase
    // Output Type      : Constructor.
    //

    public SuperAdminReportsService(
        Supabase.Client supabase)
    {
        _supabase = supabase;
    }


    // =====================================================
    // REPORTS & ANALYTICS
    // =====================================================

    //
    // Method Name      : GetReportsAsync
    // Purpose          : Retrieves reports.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<ReportsAnalyticsViewModel>.
    //

    public async Task<ReportsAnalyticsViewModel>
        GetReportsAsync()
    {
        // -------------------------------------------------
        // LOAD SCHOOLS
        // -------------------------------------------------

        var schoolsResponse =
            await _supabase
                .From<SchoolRecord>()
                .Get();

        var schools =
            schoolsResponse.Models;


        // -------------------------------------------------
        // LOAD USERS
        // -------------------------------------------------

        var usersResponse =
            await _supabase
                .From<UserRecord>()
                .Get();

        var users =
            usersResponse.Models;


        // -------------------------------------------------
        // LOAD EVENTS
        // -------------------------------------------------

        var eventsResponse =
            await _supabase
                .From<EventRecord>()
                .Get();

        var events =
            eventsResponse.Models;


        // -------------------------------------------------
        // LOAD ANNOUNCEMENTS
        // -------------------------------------------------

        var announcementsResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();

        var announcements =
            announcementsResponse.Models;


        // -------------------------------------------------
        // SCHOOL ANALYTICS
        // -------------------------------------------------

        var schoolItems =
            schools
                .Select(school =>
                {
                    var schoolUsers =
                        users
                            .Where(
                                user =>
                                    user.SchoolId ==
                                    school.Id)
                            .ToList();


                    return new SchoolAnalyticsItemViewModel
                    {
                        SchoolId =
                            school.Id,

                        SchoolName =
                            school.Name ?? "",

                        Status =
                            school.Status ?? "",

                        TotalUsers =
                            schoolUsers.Count,

                        ActiveUsers =
                            schoolUsers.Count(
                                user =>
                                    user.IsActive),

                        InactiveUsers =
                            schoolUsers.Count(
                                user =>
                                    !user.IsActive),

                        SchoolAdmins =
                            schoolUsers.Count(
                                user =>
                                    IsRole(
                                        user.Role,
                                        "School Admin")),

                        Teachers =
                            schoolUsers.Count(
                                user =>
                                    IsRole(
                                        user.Role,
                                        "Teacher")),

                        Parents =
                            schoolUsers.Count(
                                user =>
                                    IsRole(
                                        user.Role,
                                        "Parent"))
                    };
                })
                .OrderBy(
                    school =>
                        school.SchoolName)
                .ToList();


        // -------------------------------------------------
        // BUILD MAIN REPORT
        // -------------------------------------------------

        return new ReportsAnalyticsViewModel
        {
            TotalSchools =
                schools.Count,

            ActiveSchools =
                schools.Count(
                    school =>
                        string.Equals(
                            school.Status,
                            "Active",
                            StringComparison.OrdinalIgnoreCase)),

            TotalUsers =
                users.Count,

            ActiveUsers =
                users.Count(
                    user =>
                        user.IsActive),

            InactiveUsers =
                users.Count(
                    user =>
                        !user.IsActive),

            SchoolAdmins =
                users.Count(
                    user =>
                        IsRole(
                            user.Role,
                            "School Admin")),

            Teachers =
                users.Count(
                    user =>
                        IsRole(
                            user.Role,
                            "Teacher")),

            Parents =
                users.Count(
                    user =>
                        IsRole(
                            user.Role,
                            "Parent")),

            TotalEvents =
                events.Count,

            TotalAnnouncements =
                announcements.Count,

            Schools =
                schoolItems
        };
    }


    // =====================================================
    // ROLE CHECK
    // =====================================================

    //
    // Method Name      : IsRole
    // Purpose          : Checks whether role.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : string? value; string expectedRole
    // Output Type      : bool.
    //

    private static bool IsRole(
        string? value,
        string expectedRole)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }


        var normalized =
            value
                .Trim()
                .Replace("_", " ")
                .Replace("-", " ");


        return string.Equals(
            normalized,
            expectedRole,
            StringComparison.OrdinalIgnoreCase);
    }


    // =====================================================
    // USERS TABLE
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
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }


        [Column("school_id")]
        public Guid? SchoolId { get; set; }


        [Column("role")]
        public string? Role { get; set; }


        [Column("is_active")]
        public bool IsActive { get; set; }
    }


    // =====================================================
    // SCHOOLS TABLE
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
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }


        [Column("name")]
        public string? Name { get; set; }


        [Column("status")]
        public string? Status { get; set; }
    }


    // =====================================================
    // EVENTS TABLE
    // =====================================================

    //
    // Class Name       : EventRecord
    // Purpose          : Holds event record data used by the application.
    // Re-use           : Used as typed data by related services and MVC
    //                    pages.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    [Table("events")]
    private class EventRecord : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }
    }


    // =====================================================
    // ANNOUNCEMENTS TABLE
    // =====================================================

    //
    // Class Name       : AnnouncementRecord
    // Purpose          : Holds announcement record data used by the
    //                    application.
    // Re-use           : Used as typed data by related services and MVC
    //                    pages.
    // Input Parameters : Property values supplied by callers.
    // Output Type      : Class instance.
    //

    [Table("announcements")]
    private class AnnouncementRecord : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }
    }
}