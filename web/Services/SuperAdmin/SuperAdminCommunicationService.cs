using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

namespace SchoolParentalSystem.Web.Services.SuperAdmin;

//
// Class Name       : SuperAdminCommunicationService
// Purpose          : Encapsulates super admin communication operations for
//                    the School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminCommunicationService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SuperAdminCommunicationService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminCommunicationService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    // -----------------------------------------------------
    // CREATE ANNOUNCEMENT - LOAD FORM
    // -----------------------------------------------------

    //
    // Method Name      : GetCreateAnnouncementAsync
    // Purpose          : Retrieves create Announcement.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<CreateAnnouncementViewModel>.
    //

    public async Task<CreateAnnouncementViewModel>
        GetCreateAnnouncementAsync()
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
            schoolResponse.Models
                .OrderBy(x => x.Name)
                .Select(x =>
                    new SuperAdminAnnouncementSchoolItemViewModel
                    {
                        Id =
                            x.Id,

                        Name =
                            x.Name
                    })
                .ToList();


        return new CreateAnnouncementViewModel
        {
            Schools =
                schools
        };
    }


    // -----------------------------------------------------
    // CREATE ANNOUNCEMENT - SAVE
    // -----------------------------------------------------

    //
    // Method Name      : CreateAnnouncementAsync
    // Purpose          : Creates announcement.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : CreateAnnouncementViewModel model; Guid createdBy
    // Output Type      : Task.
    //

    public async Task CreateAnnouncementAsync(
        CreateAnnouncementViewModel model,
        Guid createdBy)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        if (string.IsNullOrWhiteSpace(
                model.Title))
        {
            throw new InvalidOperationException(
                "Announcement title is required.");
        }


        if (string.IsNullOrWhiteSpace(
                model.Content))
        {
            throw new InvalidOperationException(
                "Announcement message is required.");
        }


        // -------------------------------------------------
        // LOAD SCHOOLS
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();


        var schools =
            schoolResponse.Models;


        List<SuperAdminSchoolRecord>
            targetSchools;


        // -------------------------------------------------
        // SPECIFIC SCHOOL
        // -------------------------------------------------

        if (string.Equals(
                model.Target,
                "Specific School",
                StringComparison.OrdinalIgnoreCase))
        {
            if (!model.SchoolId.HasValue)
            {
                throw new InvalidOperationException(
                    "Please select a school.");
            }


            targetSchools =
                schools
                    .Where(x =>
                        x.Id ==
                        model.SchoolId.Value)
                    .ToList();


            if (targetSchools.Count == 0)
            {
                throw new InvalidOperationException(
                    "The selected school could not be found.");
            }
        }

        // -------------------------------------------------
        // ALL SCHOOLS
        // -------------------------------------------------

        else
        {
            targetSchools =
                schools.ToList();
        }


        if (targetSchools.Count == 0)
        {
            throw new InvalidOperationException(
                "No schools are available for this announcement.");
        }


        // -------------------------------------------------
        // SAVE ANNOUNCEMENTS
        // -------------------------------------------------

        foreach (var school in targetSchools)
        {
            var now =
                DateTime.UtcNow;


            var announcement =
                new AnnouncementRecord
                {
                    Id =
                        Guid.NewGuid(),

                    SchoolId =
                        school.Id,

                    CreatedBy =
                        createdBy,

                    Title =
                        model.Title.Trim(),

                    Content =
                        model.Content.Trim(),

                    Priority =
                        string.IsNullOrWhiteSpace(
                            model.Priority)
                            ? "Normal"
                            : model.Priority.Trim(),

                    // Allowed by your Supabase
                    // ck_announcements_target_type constraint.
                    TargetType =
                        "Whole School",

                    TargetRole =
                        null,

                    TargetGrade =
                        null,

                    TargetClassId =
                        null,

                    AcknowledgementRequired =
                        model.AcknowledgementRequired,

                    ExpiresAt =
                        model.ExpiresAt,

                    CreatedAt =
                        now,

                    UpdatedAt =
                        now
                };


            await _supabase
                .From<AnnouncementRecord>()
                .Insert(announcement);
        }
    }


    // -----------------------------------------------------
    // ANNOUNCEMENTS LIST
    // -----------------------------------------------------

    //
    // Method Name      : GetAnnouncementsAsync
    // Purpose          : Retrieves announcements.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<AnnouncementManagementViewModel>.
    //

    public async Task<AnnouncementManagementViewModel>
        GetAnnouncementsAsync()
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Get();


        var announcements =
            announcementResponse.Models;


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();


        var schools =
            schoolResponse.Models;


        return new AnnouncementManagementViewModel
        {
            Announcements =
                announcements
                    .OrderByDescending(x =>
                        x.CreatedAt)
                    .Select(x =>
                        new SuperAdminAnnouncementItemViewModel
                        {
                            Id =
                                x.Id,

                            Title =
                                x.Title,

                            Content =
                                x.Content,

                            Priority =
                                x.Priority,

                            AcknowledgementRequired =
                                x.AcknowledgementRequired,

                            CreatedAt =
                                x.CreatedAt,

                            ExpiresAt =
                                x.ExpiresAt,

                            SchoolName =
                                schools
                                    .FirstOrDefault(s =>
                                        s.Id == x.SchoolId)
                                    ?.Name
                                ?? "Unknown School"
                        })
                    .ToList()
        };
    }

    // -----------------------------------------------------
    // ANNOUNCEMENT DETAILS
    // -----------------------------------------------------

    //
    // Method Name      : GetAnnouncementDetailsAsync
    // Purpose          : Retrieves announcement Details.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id
    // Output Type      : Task<AnnouncementDetailsViewModel?>.
    //

    public async Task<AnnouncementDetailsViewModel?>
        GetAnnouncementDetailsAsync(
            Guid id)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        var announcementResponse =
            await _supabase
                .From<AnnouncementRecord>()
                .Where(x =>
                    x.Id == id)
                .Get();


        var announcement =
            announcementResponse.Models
                .FirstOrDefault();


        if (announcement == null)
        {
            return null;
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Where(x =>
                    x.Id == announcement.SchoolId)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        return new AnnouncementDetailsViewModel
        {
            Id =
                announcement.Id,

            Title =
                announcement.Title,

            Content =
                announcement.Content,

            SchoolName =
                school?.Name
                ?? "Unknown School",

            Priority =
                announcement.Priority,

            AcknowledgementRequired =
                announcement.AcknowledgementRequired,

            CreatedAt =
                announcement.CreatedAt,

            ExpiresAt =
                announcement.ExpiresAt
        };
    }
}