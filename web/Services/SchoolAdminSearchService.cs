
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

using static Supabase.Postgrest.Constants;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminSearchService
// Purpose          : Encapsulates school admin search operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminSearchService
{
    private readonly Supabase.Client _supabase;

    private readonly SchoolAdminProfileService _profileService;

    //
    // Method Name      : SchoolAdminSearchService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase;
    //                    SchoolAdminProfileService profileService
    // Output Type      : Constructor.
    //

    public SchoolAdminSearchService(
        Supabase.Client supabase,
        SchoolAdminProfileService profileService)
    {
        _supabase = supabase;
        _profileService = profileService;
    }

    // =========================================
    // SEARCH SCHOOL RECORDS
    // =========================================

    //
    // Method Name      : SearchAsync
    // Purpose          : Handles search operations.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : string? query; Guid userId; Guid schoolId
    // Output Type      : Task<SchoolAdminSearchViewModel>.
    //

    public async Task<SchoolAdminSearchViewModel> SearchAsync(
        string? query,
        Guid userId,
        Guid schoolId)
    {
        var term = query?.Trim() ?? "";

        if (term.Length > 100)
        {
            throw new InvalidOperationException(
                "Search must not exceed 100 characters.");
        }

        if (term.IndexOfAny(
            new[] { '%', '_', '*', '\\' }) >= 0)
        {
            throw new InvalidOperationException(
                "Please enter search text without wildcards.");
        }

        // -------------------------------------
        // VERIFY SCHOOL ADMIN & RESTORE SESSION
        // -------------------------------------

        await _profileService.GetProfileAsync(
            userId,
            schoolId);

        var result = new SchoolAdminSearchViewModel
        {
            Query = term
        };

        if (string.IsNullOrWhiteSpace(term))
        {
            return result;
        }

        var pattern = $"%{term}%";

        // =====================================
        // SEARCH LEARNERS
        // =====================================

        var learnersResponse = await _supabase
            .From<SchoolSearchLearnerRecord>()
            .Where(x => x.SchoolId == schoolId)
            .Filter(
                x => x.FullName,
                Operator.ILike,
                pattern)
            .Limit(20)
            .Get();

        result.Learners = learnersResponse.Models
            .Select(x => new SchoolAdminSearchItemViewModel
            {
                Id = x.Id,
                Name = x.FullName,
                Description =
                    $"Grade {x.Grade} · {x.AdmissionNumber}"
            })
            .ToList();

        // =====================================
        // SEARCH TEACHERS
        // =====================================

        var teachersResponse = await _supabase
            .From<UserProfile>()
            .Where(x => x.SchoolId == schoolId)
            .Where(x => x.Role == "Teacher")
            .Filter(
                x => x.FullName,
                Operator.ILike,
                pattern)
            .Limit(20)
            .Get();

        result.Teachers = teachersResponse.Models
            .Select(x => new SchoolAdminSearchItemViewModel
            {
                Id = x.Id,
                Name = x.FullName,
                Description = "Teacher"
            })
            .ToList();

        // =====================================
        // SEARCH CLASSES
        // =====================================

        var classesResponse = await _supabase
            .From<SchoolSearchClassRecord>()
            .Where(x => x.SchoolId == schoolId)
            .Filter(
                x => x.Name,
                Operator.ILike,
                pattern)
            .Limit(20)
            .Get();

        result.Classes = classesResponse.Models
            .Select(x => new SchoolAdminSearchItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description =
                    $"Grade {x.Grade} · {x.Code}"
            })
            .ToList();

        return result;
    }
}

// =============================================
// LEARNER SEARCH RECORD
// =============================================

//
// Class Name       : SchoolSearchLearnerRecord
// Purpose          : Holds school search learner record data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("learners")]
public class SchoolSearchLearnerRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = "";

    [Column("admission_number")]
    public string AdmissionNumber { get; set; } = "";

    [Column("grade")]
    public string Grade { get; set; } = "";

    [Column("school_id")]
    public Guid SchoolId { get; set; }
}

// =============================================
// CLASS SEARCH RECORD
// =============================================

//
// Class Name       : SchoolSearchClassRecord
// Purpose          : Holds school search class record data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("classes")]
public class SchoolSearchClassRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("code")]
    public string Code { get; set; } = "";

    [Column("grade")]
    public string Grade { get; set; } = "";

    [Column("school_id")]
    public Guid SchoolId { get; set; }
}
