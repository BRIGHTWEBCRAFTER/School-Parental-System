using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

namespace SchoolParentalSystem.Web.Services.SuperAdmin;

//
// Class Name       : SuperAdminEventService
// Purpose          : Encapsulates super admin event operations for the
//                    School Parental System.
// Re-use           : Used by Super Admin controllers and workflows.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SuperAdminEventService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SuperAdminEventService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SuperAdminEventService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;
        _sessionService = sessionService;
    }


    // -----------------------------------------------------
    // CREATE EVENT - LOAD FORM
    // -----------------------------------------------------

    //
    // Method Name      : GetCreateEventAsync
    // Purpose          : Retrieves create Event.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<CreateEventViewModel>.
    //

    public async Task<CreateEventViewModel>
        GetCreateEventAsync()
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
                    new SuperAdminEventSchoolItemViewModel
                    {
                        Id =
                            x.Id,

                        Name =
                            x.Name
                    })
                .ToList();


        return new CreateEventViewModel
        {
            Schools =
                schools
        };
    }


    // -----------------------------------------------------
    // CREATE EVENT - SAVE
    // -----------------------------------------------------

    //
    // Method Name      : CreateEventAsync
    // Purpose          : Creates event.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : CreateEventViewModel model; Guid createdBy
    // Output Type      : Task.
    //

    public async Task CreateEventAsync(
        CreateEventViewModel model,
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
                model.EventName))
        {
            throw new InvalidOperationException(
                "Event name is required.");
        }


        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();


        var schools =
            schoolResponse.Models;


        List<SuperAdminSchoolRecord>
            targetSchools;


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
                        x.Id == model.SchoolId.Value)
                    .ToList();


            if (targetSchools.Count == 0)
            {
                throw new InvalidOperationException(
                    "The selected school could not be found.");
            }
        }
        else
        {
            targetSchools =
                schools.ToList();
        }


        if (targetSchools.Count == 0)
        {
            throw new InvalidOperationException(
                "No schools are available for this event.");
        }


        foreach (var school in targetSchools)
        {
            var now =
                DateTime.UtcNow;


            var eventRecord =
                new EventRecord
                {
                    Id =
                        Guid.NewGuid(),

                    SchoolId =
                        school.Id,

                    CreatedBy =
                        createdBy,

                    EventName =
                        model.EventName.Trim(),

                    Description =
                        string.IsNullOrWhiteSpace(
                            model.Description)
                            ? null
                            : model.Description.Trim(),

                    EventDate =
                        model.EventDate,

                    StartTime =
                        model.StartTime,

                    EndTime =
                        model.EndTime,

                    Venue =
                        string.IsNullOrWhiteSpace(
                            model.Venue)
                            ? null
                            : model.Venue.Trim(),

                    TargetType =
                        "Whole School",

                    TargetRole =
                        null,

                    TargetGrade =
                        null,

                    TargetClassId =
                        null,

                    CreatedAt =
                        now,

                    UpdatedAt =
                        now
                };


            await _supabase
                .From<EventRecord>()
                .Insert(eventRecord);
        }
    }

    // -----------------------------------------------------
    // EVENTS LIST
    // -----------------------------------------------------

    //
    // Method Name      : GetEventsAsync
    // Purpose          : Retrieves events.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : None.
    // Output Type      : Task<EventManagementViewModel>.
    //

    public async Task<EventManagementViewModel>
        GetEventsAsync()
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // EVENTS
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Get();


        var events =
            eventResponse.Models;


        // -------------------------------------------------
        // SCHOOLS
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Get();


        var schools =
            schoolResponse.Models;


        // -------------------------------------------------
        // BUILD LIST
        // -------------------------------------------------

        return new EventManagementViewModel
        {
            Events =
                events
                    .OrderByDescending(x =>
                        x.EventDate)
                    .Select(x =>
                        new SuperAdminEventManagementItemViewModel
                        {
                            Id =
                                x.Id,

                            EventName =
                                x.EventName,

                            SchoolName =
                                schools
                                    .FirstOrDefault(s =>
                                        s.Id == x.SchoolId)
                                    ?.Name
                                ?? "Unknown School",

                            Venue =
                                x.Venue ?? "",

                            EventDate =
                                x.EventDate,

                            StartTime =
                                x.StartTime,

                            EndTime =
                                x.EndTime,

                            Description =
                                x.Description ?? "",

                            CreatedAt =
                                x.CreatedAt
                        })
                    .ToList()
        };
    }


    // -----------------------------------------------------
    // EVENT DETAILS
    // -----------------------------------------------------

    //
    // Method Name      : GetEventDetailsAsync
    // Purpose          : Retrieves event Details.
    // Re-use           : Called from Super Admin workflows or helper
    //                    methods.
    // Input Parameters : Guid id
    // Output Type      : Task<EventDetailsViewModel?>.
    //

    public async Task<EventDetailsViewModel?>
        GetEventDetailsAsync(Guid id)
    {
        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------------------
        // EVENT
        // -------------------------------------------------

        var eventResponse =
            await _supabase
                .From<EventRecord>()
                .Where(x => x.Id == id)
                .Get();


        var eventRecord =
            eventResponse.Models
                .FirstOrDefault();


        if (eventRecord == null)
        {
            return null;
        }


        // -------------------------------------------------
        // SCHOOL
        // -------------------------------------------------

        var schoolResponse =
            await _supabase
                .From<SuperAdminSchoolRecord>()
                .Where(x =>
                    x.Id == eventRecord.SchoolId)
                .Get();


        var school =
            schoolResponse.Models
                .FirstOrDefault();


        // -------------------------------------------------
        // DETAILS MODEL
        // -------------------------------------------------

        return new EventDetailsViewModel
        {
            Id =
                eventRecord.Id,

            EventName =
                eventRecord.EventName,

            Description =
                eventRecord.Description ?? "",

            SchoolName =
                school?.Name
                ?? "Unknown School",

            EventDate =
                eventRecord.EventDate,

            StartTime =
                eventRecord.StartTime,

            EndTime =
                eventRecord.EndTime,

            Venue =
                eventRecord.Venue ?? "",

            CreatedAt =
                eventRecord.CreatedAt
        };
    }
}