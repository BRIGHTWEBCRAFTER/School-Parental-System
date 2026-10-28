using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

namespace SchoolParentalSystem.Web.Controllers;


//
// Class Name       : SuperAdminController
// Purpose           : Handles system-wide schools, users, communications and reporting.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
[Authorize(Roles = "Super Admin")]
public class SuperAdminController : Controller
{
    private readonly ILogger<SuperAdminController>
        _logger;

    private readonly SuperAdminDashboardService
        _dashboardService;

    private readonly SuperAdminSchoolService
        _schoolService;

    private readonly SuperAdminCommunicationService
        _communicationService;

    private readonly SuperAdminEventService
        _eventService;

    private readonly SuperAdminUserService
        _userService;

    private readonly SuperAdminReportsService
        _reportsService;


    //
    // Method Name       : SuperAdminController
    // Purpose           : Initializes required dependencies for SuperAdminController.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : ILogger<SuperAdminController> logger; SuperAdminDashboardService
    //                      dashboardService; SuperAdminSchoolService schoolService;
    //                      SuperAdminCommunicationService communicationService;
    //                      SuperAdminEventService eventService; SuperAdminUserService userService;
    //                      SuperAdminReportsService reportsService
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public SuperAdminController(
        ILogger<SuperAdminController> logger,
        SuperAdminDashboardService dashboardService,
        SuperAdminSchoolService schoolService,
        SuperAdminCommunicationService communicationService,
        SuperAdminEventService eventService,
        SuperAdminUserService userService,
        SuperAdminReportsService reportsService)
    {
        _logger =
            logger;

        _dashboardService =
            dashboardService;

        _schoolService =
            schoolService;

        _communicationService =
            communicationService;

        _eventService =
            eventService;

        _userService =
            userService;

        _reportsService =
            reportsService;
    }


    // -----------------------------------------------------
    // DASHBOARD
    // -----------------------------------------------------

    //
    // Method Name       : Dashboard
    // Purpose           : Load the dashboard summary for the current user role.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        ViewData["Title"] =
            "Dashboard";


        var adminName =
            User.Identity?.Name
            ?? "Super Admin";


        try
        {
            var model =
                await _dashboardService
                    .GetDashboardAsync(
                        adminName);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Super Admin dashboard.");


            TempData["ErrorMessage"] =
                "Some dashboard information could not be loaded.";


            return View(
                new SuperAdminDashboardViewModel
                {
                    AdminName =
                        adminName
                });
        }
    }


    // -----------------------------------------------------
    // USERS MANAGEMENT
    // -----------------------------------------------------

    //
    // Method Name       : Users
    // Purpose           : List system users managed by the Super Admin.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        ViewData["Title"] =
            "Manage Users";


        try
        {
            var model =
                await _userService
                    .GetUsersAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load users.");


            TempData["ErrorMessage"] =
                "Users could not be loaded.";


            return View(
                new UserManagementViewModel());
        }
    }


    // -----------------------------------------------------
    // USER DETAILS
    // -----------------------------------------------------

    //
    // Method Name       : UserDetails
    // Purpose           : Display details of the selected system user.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> UserDetails(
        Guid id)
    {
        ViewData["Title"] =
            "User Details";


        try
        {
            var model =
                await _userService
                    .GetUserDetailsAsync(id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "User could not be found.";


                return RedirectToAction(
                    nameof(Users));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load user {UserId}.",
                id);


            TempData["ErrorMessage"] =
                "The user details could not be loaded.";


            return RedirectToAction(
                nameof(Users));
        }
    }


    // -----------------------------------------------------
    // ACTIVATE / DEACTIVATE USER
    // -----------------------------------------------------

    //
    // Method Name       : SetUserActiveStatus
    // Purpose           : Activate or deactivate a selected system user.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id; bool isActive
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserActiveStatus(
        Guid id,
        bool isActive)
    {
        var currentUserIdValue =
            User.FindFirst(
                System.Security.Claims
                    .ClaimTypes.NameIdentifier)
                ?.Value;


        if (!isActive &&
            Guid.TryParse(
                currentUserIdValue,
                out var currentUserId) &&
            currentUserId == id)
        {
            TempData["ErrorMessage"] =
                "You cannot deactivate your own account.";


            return RedirectToAction(
                nameof(UserDetails),
                new
                {
                    id
                });
        }


        try
        {
            await _userService
                .SetUserActiveStatusAsync(
                    id,
                    isActive);


            TempData["SuccessMessage"] =
                isActive
                    ? "User activated successfully."
                    : "User deactivated successfully.";


            return RedirectToAction(
                nameof(UserDetails),
                new
                {
                    id
                });
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Users));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update status for user {UserId}.",
                id);


            TempData["ErrorMessage"] =
                "The user account status could not be updated.";


            return RedirectToAction(
                nameof(UserDetails),
                new
                {
                    id
                });
        }
    }


    // -----------------------------------------------------
    // ADD SCHOOL - PAGE
    // -----------------------------------------------------

    //
    // Method Name       : AddSchool
    // Purpose           : Display or process adding a school to the system.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [HttpGet]
    public IActionResult AddSchool()
    {
        ViewData["Title"] =
            "Add School";


        return View(
            new AddSchoolViewModel());
    }


    // -----------------------------------------------------
    // ADD SCHOOL - SAVE
    // -----------------------------------------------------

    //
    // Method Name       : AddSchool
    // Purpose           : Display or process adding a school to the system.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : AddSchoolViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSchool(
        AddSchoolViewModel model)
    {
        ViewData["Title"] =
            "Add School";


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        try
        {
            await _schoolService
                .AddSchoolAsync(model);


            TempData["SuccessMessage"] =
                "School added successfully.";


            return RedirectToAction(
                nameof(Dashboard));
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to add school.");


            ModelState.AddModelError(
                "",
                "The school could not be added. Please try again.");


            return View(model);
        }
    }


    // -----------------------------------------------------
    // SCHOOLS MANAGEMENT
    // -----------------------------------------------------

    //
    // Method Name       : Schools
    // Purpose           : List registered schools in the system.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Schools()
    {
        ViewData["Title"] =
            "Schools";


        try
        {
            var model =
                await _schoolService
                    .GetSchoolsAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load schools.");


            TempData["ErrorMessage"] =
                "The schools could not be loaded.";


            return View(
                new SchoolManagementViewModel());
        }
    }


    // -----------------------------------------------------
    // SCHOOL DETAILS
    // -----------------------------------------------------

    //
    // Method Name       : SchoolDetails
    // Purpose           : Display details of the selected school.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> SchoolDetails(
        Guid id)
    {
        try
        {
            var model =
                await _schoolService
                    .GetSchoolDetailsAsync(id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "School could not be found.";

                return RedirectToAction(
                    nameof(Schools));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load school details.");


            TempData["ErrorMessage"] =
                "The school details could not be loaded.";


            return RedirectToAction(
                nameof(Schools));
        }
    }


    // -----------------------------------------------------
    // EDIT SCHOOL - PAGE
    // -----------------------------------------------------

    //
    // Method Name       : EditSchool
    // Purpose           : Display or save updates to a school record.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> EditSchool(
        Guid id)
    {
        try
        {
            var model =
                await _schoolService
                    .GetEditSchoolAsync(id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "School could not be found.";

                return RedirectToAction(
                    nameof(Schools));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load school for editing.");


            TempData["ErrorMessage"] =
                "The school could not be loaded for editing.";


            return RedirectToAction(
                nameof(Schools));
        }
    }


    // -----------------------------------------------------
    // EDIT SCHOOL - SAVE
    // -----------------------------------------------------

    //
    // Method Name       : EditSchool
    // Purpose           : Display or save updates to a school record.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : EditSchoolViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSchool(
        EditSchoolViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        try
        {
            await _schoolService
                .UpdateSchoolAsync(model);


            TempData["SuccessMessage"] =
                "School updated successfully.";


            return RedirectToAction(
                nameof(SchoolDetails),
                new
                {
                    id =
                        model.Id
                });
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update school {SchoolId}.",
                model.Id);


            ModelState.AddModelError(
                "",
                "The school could not be updated.");


            return View(model);
        }
    }


    // -----------------------------------------------------
    // ANNOUNCEMENTS LIST
    // -----------------------------------------------------

    //
    // Method Name       : Announcements
    // Purpose           : Handle the announcements operation.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Announcements()
    {
        ViewData["Title"] =
            "Announcements";


        try
        {
            var model =
                await _communicationService
                    .GetAnnouncementsAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load announcements.");


            TempData["ErrorMessage"] =
                "Announcements could not be loaded.";


            return View(
                new AnnouncementManagementViewModel());
        }
    }


    // -----------------------------------------------------
    // CREATE ANNOUNCEMENT - PAGE
    // -----------------------------------------------------

    //
    // Method Name       : CreateAnnouncement
    // Purpose           : Display or process creation of a school announcement.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> CreateAnnouncement()
    {
        ViewData["Title"] =
            "Create Announcement";


        try
        {
            var model =
                await _communicationService
                    .GetCreateAnnouncementAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load create announcement page.");


            TempData["ErrorMessage"] =
                "The announcement page could not be loaded.";


            return RedirectToAction(
                nameof(Dashboard));
        }
    }


    // -----------------------------------------------------
    // CREATE ANNOUNCEMENT - SAVE
    // -----------------------------------------------------

    //
    // Method Name       : CreateAnnouncement
    // Purpose           : Display or process creation of a school announcement.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : CreateAnnouncementViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAnnouncement(
        CreateAnnouncementViewModel model)
    {
        ViewData["Title"] =
            "Create Announcement";


        if (!ModelState.IsValid)
        {
            await ReloadAnnouncementSchoolsAsync(
                model);

            return View(model);
        }


        var userIdValue =
            User.FindFirst(
                System.Security.Claims
                    .ClaimTypes.NameIdentifier)
                ?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var createdBy))
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            await _communicationService
                .CreateAnnouncementAsync(
                    model,
                    createdBy);


            TempData["SuccessMessage"] =
                "Announcement was sent successfully.";


            return RedirectToAction(
                nameof(Dashboard));
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to create announcement.");


            ModelState.AddModelError(
                "",
                "Supabase error: " +
                ex.Message);
        }


        await ReloadAnnouncementSchoolsAsync(
            model);


        return View(model);
    }


    // -----------------------------------------------------
    // ANNOUNCEMENT DETAILS
    // -----------------------------------------------------

    //
    // Method Name       : AnnouncementDetails
    // Purpose           : Display the details of an announcement.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> AnnouncementDetails(
        Guid id)
    {
        ViewData["Title"] =
            "Announcement Details";


        try
        {
            var model =
                await _communicationService
                    .GetAnnouncementDetailsAsync(
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Announcement could not be found.";


                return RedirectToAction(
                    nameof(Announcements));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();


            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load announcement {AnnouncementId}.",
                id);


            TempData["ErrorMessage"] =
                "The announcement details could not be loaded.";


            return RedirectToAction(
                nameof(Announcements));
        }
    }


    // -----------------------------------------------------
    // EVENTS LIST
    // -----------------------------------------------------

    //
    // Method Name       : Events
    // Purpose           : Display school events available to the current role.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Events()
    {
        ViewData["Title"] =
            "Events";


        try
        {
            var model =
                await _eventService
                    .GetEventsAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load events.");


            TempData["ErrorMessage"] =
                "Events could not be loaded.";


            return View(
                new EventManagementViewModel());
        }
    }


    // -----------------------------------------------------
    // CREATE EVENT - PAGE
    // -----------------------------------------------------

    //
    // Method Name       : CreateEvent
    // Purpose           : Display or process creation of a school event.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        ViewData["Title"] =
            "Create Event";


        try
        {
            var model =
                await _eventService
                    .GetCreateEventAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load create event page.");


            TempData["ErrorMessage"] =
                "The event page could not be loaded.";


            return RedirectToAction(
                nameof(Dashboard));
        }
    }


    // -----------------------------------------------------
    // CREATE EVENT - SAVE
    // -----------------------------------------------------

    //
    // Method Name       : CreateEvent
    // Purpose           : Display or process creation of a school event.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : CreateEventViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
        CreateEventViewModel model)
    {
        ViewData["Title"] =
            "Create Event";


        if (!ModelState.IsValid)
        {
            await ReloadEventSchoolsAsync(
                model);

            return View(model);
        }


        var userIdValue =
            User.FindFirst(
                System.Security.Claims
                    .ClaimTypes.NameIdentifier)
                ?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var createdBy))
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            await _eventService
                .CreateEventAsync(
                    model,
                    createdBy);


            TempData["SuccessMessage"] =
                "Event was created successfully.";


            return RedirectToAction(
                nameof(Dashboard));
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to create event.");


            ModelState.AddModelError(
                "",
                "The event could not be created: " +
                ex.Message);
        }


        await ReloadEventSchoolsAsync(
            model);


        return View(model);
    }


    // -----------------------------------------------------
    // EVENT DETAILS
    // -----------------------------------------------------

    //
    // Method Name       : EventDetails
    // Purpose           : Display details of the selected school event.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> EventDetails(
        Guid id)
    {
        ViewData["Title"] =
            "Event Details";


        try
        {
            var model =
                await _eventService
                    .GetEventDetailsAsync(id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Event could not be found.";


                return RedirectToAction(
                    nameof(Events));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();


            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load event {EventId}.",
                id);


            TempData["ErrorMessage"] =
                "The event details could not be loaded.";


            return RedirectToAction(
                nameof(Events));
        }
    }


    // -----------------------------------------------------
    // REPORTS & ANALYTICS
    // -----------------------------------------------------

    //
    // Method Name       : Reports
    // Purpose           : Display the Super Admin reporting dashboard.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Reports()
    {
        ViewData["Title"] =
            "Reports & Analytics";


        try
        {
            var model =
                await _reportsService
                    .GetReportsAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load reports and analytics.");


            TempData["ErrorMessage"] =
                "Reports and analytics could not be loaded.";


            return View(
                new ReportsAnalyticsViewModel());
        }
    }


    // -----------------------------------------------------
    // SYSTEM ACTIVITY
    // -----------------------------------------------------

    //
    // Method Name       : AuditLogs
    // Purpose           : Display recorded system administration activities.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> AuditLogs()
    {
        ViewData["Title"] =
            "System Activity";


        try
        {
            var model =
                await _dashboardService
                    .GetSystemActivityAsync();


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await SignOutUserAsync();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load system activity.");


            TempData["ErrorMessage"] =
                "System activity could not be loaded.";


            return View(
                new SystemActivityViewModel());
        }
    }


    // -----------------------------------------------------
    // HELPER - RELOAD ANNOUNCEMENT SCHOOLS
    // -----------------------------------------------------

    //
    // Method Name       : ReloadAnnouncementSchoolsAsync
    // Purpose           : Reload school options for the announcement form.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : CreateAnnouncementViewModel model
    // Output Type       : Task
    //
    private async Task ReloadAnnouncementSchoolsAsync(
        CreateAnnouncementViewModel model)
    {
        var formModel =
            await _communicationService
                .GetCreateAnnouncementAsync();


        model.Schools =
            formModel.Schools;
    }


    // -----------------------------------------------------
    // HELPER - RELOAD EVENT SCHOOLS
    // -----------------------------------------------------

    //
    // Method Name       : ReloadEventSchoolsAsync
    // Purpose           : Reload school options for the event form.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : CreateEventViewModel model
    // Output Type       : Task
    //
    private async Task ReloadEventSchoolsAsync(
        CreateEventViewModel model)
    {
        var formModel =
            await _eventService
                .GetCreateEventAsync();


        model.Schools =
            formModel.Schools;
    }


    // -----------------------------------------------------
    // HELPER - SIGN OUT
    // -----------------------------------------------------

    //
    // Method Name       : SignOutUserAsync
    // Purpose           : Clear the current administrative authentication session.
    // Re-use            : Invoked by SuperAdminController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task
    //
    private async Task SignOutUserAsync()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme);


        HttpContext.Session.Clear();
    }
}