using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.ViewModels;
using SchoolParentalSystem.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SchoolParentalSystem.Web.Controllers;

[Authorize(Roles = "School Admin")]
public class SchoolAdminController : Controller
{
    private readonly SchoolAdminLearnerService _learnerService;
    private readonly ILogger<SchoolAdminController> _logger;
    private readonly SchoolAdminTeacherService _teacherService;
    private readonly SchoolAdminParentService _parentService;
    private readonly SchoolAdminClassService _classService;
    private readonly SchoolAdminAttendanceService _attendanceService;
    private readonly SchoolAdminGradeService _gradeService;
    private readonly SchoolAdminCommunicationService _communicationService;
    private readonly SchoolAdminProfileService _profileService;
    private readonly ParentCommunicationEmailService _parentEmailService;

    public SchoolAdminController(
    SchoolAdminLearnerService learnerService,
    SchoolAdminTeacherService teacherService,
    SchoolAdminParentService parentService,
    SchoolAdminClassService classService,
    SchoolAdminAttendanceService attendanceService,
    SchoolAdminGradeService gradeService,
    SchoolAdminCommunicationService communicationService,
    SchoolAdminProfileService profileService,
    ParentCommunicationEmailService parentEmailService,
    ILogger<SchoolAdminController> logger)
    {
        _learnerService = learnerService;

        _teacherService = teacherService;

        _parentService = parentService;

        _classService = classService;

        _attendanceService = attendanceService;

        _gradeService = gradeService;

        _communicationService = communicationService;

        _profileService = profileService;
        _parentEmailService = parentEmailService;

        _logger = logger;
    }
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
        schoolIdValue,
        out var schoolId))
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }

        try
        {
            var adminName =
                User.Identity?.Name ??
                "School Admin";

            var model =
                await _learnerService
                    .GetDashboardAsync(
                        schoolId,
                        adminName);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load School Admin dashboard.");

            TempData["ErrorMessage"] =
                "The dashboard could not be loaded.";

            return RedirectToAction(
                nameof(Learners));
        }
    }


    [HttpGet]
    [Authorize(Roles = "School Admin")]
    public async Task<IActionResult> Search(
        string? query,
        [FromServices] SchoolAdminSearchService searchService)
    {
        // -----------------------------------------
        // GET LOGGED-IN SCHOOL ADMIN
        // -----------------------------------------

        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(userIdValue, out var userId) ||
            !Guid.TryParse(schoolIdValue, out var schoolId))
        {
            return RedirectToAction("Login", "Auth");
        }

        // -----------------------------------------
        // SEARCH SCHOOL RECORDS
        // -----------------------------------------

        try
        {
            var model =
                await searchService.SearchAsync(
                    query,
                    userId,
                    schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ViewData["SearchError"] = ex.Message;

            return View(
                new SchoolAdminSearchViewModel
                {
                    Query = query?.Trim() ?? ""
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "School Admin search failed.");

            ViewData["SearchError"] =
                "Search results could not be loaded. Please try again.";

            return View(
                new SchoolAdminSearchViewModel
                {
                    Query = query?.Trim() ?? ""
                });
        }
    }


    public async Task<IActionResult> Learners()
    {
        try
        {
            var model =
                await _learnerService
                    .GetLearnersAsync();

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load learners.");

            TempData["ErrorMessage"] =
                "Learner records could not be loaded.";

            return View(
                new LearnersPageViewModel());
        }
    }


    [HttpGet]
    [Authorize(Roles = "School Admin")]
    public async Task<IActionResult> Profile()
    {
        // -----------------------------------------
        // GET LOGGED-IN ADMINISTRATOR
        // -----------------------------------------

        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var userId) ||
            !Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }


        // -----------------------------------------
        // LOAD PROFILE FROM SUPABASE
        // -----------------------------------------

        try
        {
            var model =
                await _profileService
                    .GetProfileAsync(
                        userId,
                        schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load School Admin profile.");

            TempData["ErrorMessage"] =
                "Your profile could not be loaded.";

            return RedirectToAction(
                nameof(Dashboard));
        }
    }


    [HttpGet]
    [Authorize(Roles = "School Admin")]
    public async Task<IActionResult> EditProfile()
    {
        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var userId) ||
            !Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _profileService
                    .GetEditProfileAsync(
                        userId,
                        schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load edit profile form.");

            TempData["ErrorMessage"] =
                "Your profile could not be loaded for editing.";

            return RedirectToAction(
                nameof(Profile));
        }
    }


    [HttpPost]
    [Authorize(Roles = "School Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(
        EditSchoolAdminProfileViewModel model)
    {
        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var userId) ||
            !Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        try
        {
            // -------------------------------------
            // SAVE CHANGES TO SUPABASE
            // -------------------------------------

            await _profileService
                .UpdateProfileAsync(
                    model,
                    userId,
                    schoolId);


            // -------------------------------------
            // REFRESH ADMINISTRATOR NAME CLAIM
            // -------------------------------------

            var identity =
                new ClaimsIdentity(
                    User.Claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);

            var existingNameClaim =
                identity.FindFirst(
                    ClaimTypes.Name);

            if (existingNameClaim != null)
            {
                identity.RemoveClaim(
                    existingNameClaim);
            }

            identity.AddClaim(
                new Claim(
                    ClaimTypes.Name,
                    model.FullName.Trim()));


            var principal =
                new ClaimsPrincipal(identity);


            // Preserve existing cookie properties.
            var authentication =
                await HttpContext.AuthenticateAsync(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                principal,
                authentication.Succeeded
                    ? authentication.Properties
                    : null);


            // -------------------------------------
            // SUCCESS
            // -------------------------------------

            TempData["SuccessMessage"] =
                "Your profile has been updated successfully.";

            return RedirectToAction(
                nameof(Profile));
        }
        catch (UnauthorizedAccessException)
        {
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
                "Unable to update School Admin profile.");

            ModelState.AddModelError(
                "",
                "Your profile could not be updated. Please try again.");

            return View(model);
        }
    }


    [HttpPost]
    [Authorize(Roles = "School Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(
        IFormFile picture)
    {
        // -----------------------------------------
        // GET LOGGED-IN ADMINISTRATOR
        // -----------------------------------------

        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var userId) ||
            !Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }


        // -----------------------------------------
        // VALIDATE SELECTED FILE
        // -----------------------------------------

        if (picture == null ||
            picture.Length == 0)
        {
            TempData["ErrorMessage"] =
                "Please select a profile picture.";

            return RedirectToAction(
                nameof(Profile));
        }


        // -----------------------------------------
        // UPLOAD PROFILE PICTURE
        // -----------------------------------------

        try
        {
            await _profileService
                .UploadProfilePictureAsync(
                    picture,
                    userId,
                    schoolId);


            TempData["SuccessMessage"] =
                "Profile picture updated successfully.";


            return RedirectToAction(
                nameof(Profile));
        }
        catch (UnauthorizedAccessException)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;

            return RedirectToAction(
                nameof(Profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to upload School Admin profile picture.");


            TempData["ErrorMessage"] =
                "The profile picture could not be uploaded. Please try again.";


            return RedirectToAction(
                nameof(Profile));
        }
    }


    [HttpPost]
    [Authorize(Roles = "School Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        // -----------------------------------------
        // GET LOGGED-IN ADMINISTRATOR
        // -----------------------------------------

        var userIdValue =
            User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                userIdValue,
                out var userId) ||
            !Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }


        // -----------------------------------------
        // REMOVE PROFILE PICTURE
        // -----------------------------------------

        try
        {
            var removedSuccessfully =
                await _profileService
                    .RemoveProfilePictureAsync(
                        userId,
                        schoolId);


            if (removedSuccessfully)
            {
                TempData["SuccessMessage"] =
                    "Profile picture removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Your profile picture was cleared, " +
                    "but the old storage file could not " +
                    "be removed. Please contact support " +
                    "if cleanup is required.";
            }


            return RedirectToAction(
                nameof(Profile));
        }
        catch (UnauthorizedAccessException)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;

            return RedirectToAction(
                nameof(Profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to remove School Admin profile picture.");


            TempData["ErrorMessage"] =
                "The profile picture could not be removed. " +
                "Please try again.";


            return RedirectToAction(
                nameof(Profile));
        }
    }



    [HttpGet]
    public async Task<IActionResult> AddLearner()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        var model =
            new AddLearnerViewModel();

        model.AvailableClasses =
            await _learnerService
                .GetAvailableClassesAsync(schoolId);

        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult>
    LearnerDetails(Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _learnerService
                    .GetLearnerDetailsAsync(
                        id,
                        schoolId);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "The learner could not be found.";

                return RedirectToAction(
                    nameof(Learners));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load learner {LearnerId}.",
                id);

            TempData["ErrorMessage"] =
                "The learner details could not be loaded.";

            return RedirectToAction(
                nameof(Learners));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Teachers()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _teacherService
                    .GetTeachersAsync(
                        schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load teachers.");

            TempData["ErrorMessage"] =
                "Teachers could not be loaded.";

            return RedirectToAction(
                nameof(Dashboard));
        }
    }

    [HttpGet]
    public async Task<IActionResult> TeacherDetails(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _teacherService
                    .GetTeacherDetailsAsync(
                        schoolId,
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Teacher could not be found.";

                return RedirectToAction(
                    nameof(Teachers));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load teacher {TeacherId}.",
                id);

            TempData["ErrorMessage"] =
                "The teacher details could not be loaded.";

            return RedirectToAction(
                nameof(Teachers));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditTeacher(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        var model =
            await _teacherService
                .GetTeacherForEditAsync(
                    schoolId,
                    id);


        if (model == null)
        {
            TempData["ErrorMessage"] =
                "Teacher could not be found.";

            return RedirectToAction(
                nameof(Teachers));
        }


        return View(model);
    }

    [HttpGet]
    public IActionResult AddTeacher()
    {
        return View(
            new AddTeacherViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> SearchParents(
    string query)
    {
        if (string.IsNullOrWhiteSpace(query) ||
            query.Trim().Length < 2)
        {
            return Json(
                Array.Empty<ParentSearchResultViewModel>());
        }

        try
        {
            var results =
                await _learnerService
                    .SearchParentsAsync(query);

            return Json(results);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to search parents.");

            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Parents()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _parentService
                    .GetParentsAsync(
                        schoolId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load parent management.");


            TempData["ErrorMessage"] =
                "Parent records could not be loaded.";


            return View(
                new ParentsPageViewModel());
        }
    }

    [HttpGet]
    public IActionResult AddParent()
    {
        return View(
            new AddParentViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddParent(
        AddParentViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _parentService
                .AddParentAsync(
                    schoolId,
                    model);

            TempData["SuccessMessage"] =
                $"{model.FullName} was invited successfully as a parent.";

            return RedirectToAction(
                nameof(Parents));
        }
        catch (UnauthorizedAccessException)
        {
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
                "Unable to add parent.");

            ModelState.AddModelError(
                "",
                "The parent could not be added. " +
                "Please check the details and try again.");

            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> ParentDetails(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _parentService
                    .GetParentDetailsAsync(
                        schoolId,
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Parent could not be found.";

                return RedirectToAction(
                    nameof(Parents));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load parent {ParentId}.",
                id);


            TempData["ErrorMessage"] =
                "The parent details could not be loaded.";


            return RedirectToAction(
                nameof(Parents));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditParent(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _parentService
                    .GetParentForEditAsync(
                        schoolId,
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Parent could not be found.";

                return RedirectToAction(
                    nameof(Parents));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Classes()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        try
        {
            var model =
                await _classService
                    .GetClassesAsync(
                        schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Class Management.");

            TempData["ErrorMessage"] =
                "Class records could not be loaded.";

            return View(
                new ClassesPageViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> ClassDetails(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _classService
                    .GetClassDetailsAsync(
                        schoolId,
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Class record could not be found.";

                return RedirectToAction(
                    nameof(Classes));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load class {ClassId}.",
                id);


            TempData["ErrorMessage"] =
                "Class details could not be loaded.";


            return RedirectToAction(
                nameof(Classes));
        }
    }


    [HttpGet]
    public async Task<IActionResult> AddClass()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _classService
                    .GetAddClassModelAsync(
                        schoolId,
                        DateTime.Today.Year);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditClass(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _classService
                    .GetClassForEditAsync(
                        schoolId,
                        id);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Class record could not be found.";

                return RedirectToAction(
                    nameof(Classes));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load class {ClassId} for editing.",
                id);


            TempData["ErrorMessage"] =
                "Class record could not be loaded.";


            return RedirectToAction(
                nameof(Classes));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Attendance(
    DateTime? date,
    Guid? classId,
    Guid? subjectId,
    string? search)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var selectedDate =
                date?.Date ??
                DateTime.Today;


            var model =
                await _attendanceService
                    .GetAttendanceAsync(
                        schoolId,
                        selectedDate,
                        classId,
                        subjectId,
                        search);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load attendance management.");

            return View(
                new AttendanceManagementViewModel
                {
                    AttendanceDate =
                        date?.Date ??
                        DateTime.Today
                });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Grades(
    Guid? classId,
    Guid? subjectId,
    string? term,
    int? academicYear,
    string? search)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _gradeService
                    .GetGradesAsync(
                        schoolId,
                        classId,
                        subjectId,
                        term,
                        academicYear,
                        search);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load grades management.");


            return View(
                new GradeManagementViewModel
                {
                    SearchTerm =
                        search ?? "",

                    SelectedClassId =
                        classId,

                    SelectedSubjectId =
                        subjectId,

                    SelectedTerm =
                        term ?? "",

                    SelectedAcademicYear =
                        academicYear
                });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Communication(
    string? search,
    string? priority,
    string? targetType,
    string? status)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetCommunicationAsync(
                        schoolId,
                        search,
                        priority,
                        targetType,
                        status);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load communication management.");


            return View(
                new CommunicationManagementViewModel
                {
                    SearchTerm =
                        search ?? "",

                    SelectedPriority =
                        priority ?? "",

                    SelectedTarget =
                        targetType ?? "",

                    SelectedStatus =
                        status ?? ""
                });
        }
    }


    [HttpGet]
    public async Task<IActionResult> CommunicationStatistics()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetCommunicationStatisticsAsync(
                        schoolId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load communication statistics.");


            TempData["ErrorMessage"] =
                "Communication statistics could not be loaded.";


            return View(
                new CommunicationStatisticsViewModel());
        }
    }


    [HttpGet]
    public async Task<IActionResult> CreateAnnouncement()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetCreateAnnouncementAsync(
                        schoolId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load create announcement form.");


            TempData["ErrorMessage"] =
                "The announcement form could not be loaded.";


            return RedirectToAction(
                nameof(Communication));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAnnouncement(
        CreateAnnouncementViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            await _communicationService
                .PopulateCreateAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }


        try
        {
            var announcementId = await _communicationService
                .CreateAnnouncementAsync(model, schoolId);

            // In-app notifications were created by Supabase.
            // Email delivery is a separate, non-destructive step.
            try
            {
                var result = await _parentEmailService.SendAsync(
                    announcementId,
                    "announcement",
                    model.Title,
                    "Priority: " + model.Priority + "\n\n" + model.Content);

                TempData["SuccessMessage"] =
                    $"Announcement created. Parent emails sent: {result.SentCount}.";
                if (result.FailedCount > 0)
                    TempData["ErrorMessage"] =
                        $"{result.FailedCount} Parent email(s) could not be delivered.";
            }
            catch (Exception emailError)
            {
                _logger.LogError(emailError,
                    "Announcement {AnnouncementId} was saved but email dispatch failed",
                    announcementId);
                TempData["SuccessMessage"] =
                    "Announcement created; mobile notifications saved.";
                TempData["ErrorMessage"] =
                    "Parent email dispatch was unsuccessful. Check application logs.";
            }


            return RedirectToAction(
                nameof(Communication));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);


            await _communicationService
                .PopulateCreateAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to create announcement.");


            ModelState.AddModelError(
                "",
                "The announcement could not be created.");


            await _communicationService
                .PopulateCreateAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditAnnouncement(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetEditAnnouncementAsync(
                        id,
                        schoolId);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Announcement could not be found.";


                return RedirectToAction(
                    nameof(Communication));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load announcement for editing.");


            TempData["ErrorMessage"] =
                "The announcement could not be loaded.";


            return RedirectToAction(
                nameof(Communication));
        }
    }

    [HttpGet]
    public async Task<IActionResult> AnnouncementDetails(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetAnnouncementDetailsAsync(
                        id,
                        schoolId);


            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "Announcement could not be found.";


                return RedirectToAction(
                    nameof(Communication));
            }


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load announcement details.");


            TempData["ErrorMessage"] =
                "The announcement details could not be loaded.";


            return RedirectToAction(
                nameof(Communication));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Events()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetEventsAsync(
                        schoolId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load event management.");


            return View(
                new EventManagementViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetCreateEventAsync(
                        schoolId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load create event form.");


            TempData["ErrorMessage"] =
                "The event form could not be loaded.";


            return RedirectToAction(
                nameof(Communication));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EventDetails(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetEventDetailsAsync(
                        id,
                        schoolId);


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
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load event details.");


            TempData["ErrorMessage"] =
                "The event details could not be loaded.";


            return RedirectToAction(
                nameof(Events));
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _communicationService
                    .GetEditEventAsync(
                        id,
                        schoolId);


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
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load edit event form.");


            TempData["ErrorMessage"] =
                "The event could not be loaded for editing.";


            return RedirectToAction(
                nameof(Events));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(
        EditEventViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            await _communicationService
                .PopulateEditEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }


        try
        {
            await _communicationService
                .UpdateEventAsync(
                    model,
                    schoolId);


            TempData["SuccessMessage"] =
                "Event updated successfully.";


            return RedirectToAction(
                nameof(EventDetails),
                new
                {
                    id = model.Id
                });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);


            await _communicationService
                .PopulateEditEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update event.");


            ModelState.AddModelError(
                "",
                "The event could not be updated.");


            await _communicationService
                .PopulateEditEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
        CreateEventViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            await _communicationService
                .PopulateCreateEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }


        try
        {
            var eventId = await _communicationService
                .CreateEventAsync(model, schoolId);

            // The RPC has already created Flutter notifications.
            try
            {
                var body = "Event: " + model.EventName +
                    "\nDate: " + model.EventDate.ToString("dd MMM yyyy") +
                    "\nTime: " + (model.StartTime?.ToString(@"hh\:mm") ?? "Not specified") +
                    " - " + (model.EndTime?.ToString(@"hh\:mm") ?? "Not specified") +
                    "\nVenue: " + (model.Venue ?? "Not specified") +
                    "\n\n" + (model.Description ?? "");

                var result = await _parentEmailService.SendAsync(
                    eventId, "event", model.EventName, body);

                TempData["SuccessMessage"] =
                    $"Event created. Parent emails sent: {result.SentCount}.";
                if (result.FailedCount > 0)
                    TempData["ErrorMessage"] =
                        $"{result.FailedCount} Parent email(s) could not be delivered.";
            }
            catch (Exception emailError)
            {
                _logger.LogError(emailError,
                    "Event {EventId} was saved but email dispatch failed", eventId);
                TempData["SuccessMessage"] =
                    "Event created; mobile notifications saved.";
                TempData["ErrorMessage"] =
                    "Parent email dispatch was unsuccessful. Check application logs.";
            }


            return RedirectToAction(
                nameof(Communication));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);


            await _communicationService
                .PopulateCreateEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to create event.");


            ModelState.AddModelError(
                "",
                "The event could not be created.");


            await _communicationService
                .PopulateCreateEventOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            await _communicationService
                .DeleteEventAsync(
                    id,
                    schoolId);


            TempData["SuccessMessage"] =
                "Event deleted successfully.";


            return RedirectToAction(
                nameof(Events));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Events));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to delete event.");


            TempData["ErrorMessage"] =
                "The event could not be deleted.";


            return RedirectToAction(
                nameof(Events));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAnnouncement(
        EditAnnouncementViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            await _communicationService
                .PopulateEditAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }


        try
        {
            await _communicationService
                .UpdateAnnouncementAsync(
                    model,
                    schoolId);


            TempData["SuccessMessage"] =
                "Announcement updated successfully.";


            return RedirectToAction(
                nameof(Communication));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);


            await _communicationService
                .PopulateEditAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update announcement.");


            ModelState.AddModelError(
                "",
                "The announcement could not be updated.");


            await _communicationService
                .PopulateEditAnnouncementOptionsAsync(
                    model,
                    schoolId);


            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(
    Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            await _communicationService
                .DeleteAnnouncementAsync(
                    id,
                    schoolId);


            TempData["SuccessMessage"] =
                "Announcement deleted successfully.";
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to delete announcement.");


            TempData["ErrorMessage"] =
                "The announcement could not be deleted.";
        }


        return RedirectToAction(
            nameof(Communication));
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditParent(
    Guid id,
    EditParentViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var updated =
                await _parentService
                    .UpdateParentAsync(
                        schoolId,
                        id,
                        model);


            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Parent could not be found.";

                return RedirectToAction(
                    nameof(Parents));
            }


            TempData["SuccessMessage"] =
            "Parent account information updated successfully.";


            return RedirectToAction(
                nameof(ParentDetails),
                new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update parent {ParentId}.",
                id);


            ModelState.AddModelError(
                "",
                "The parent information could not be updated.");


            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateParent(
    Guid id,
    string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        var updated =
            await _parentService
                .SetParentActiveStatusAsync(
                    schoolId,
                    id,
                    false);


        TempData[
            updated
                ? "SuccessMessage"
                : "ErrorMessage"
        ] =
            updated
                ? "Parent deactivated successfully."
                : "Parent could not be found.";


        return RedirectAfterStatusChange(
            returnUrl,
            nameof(Parents));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivateParent(
    Guid id,
    string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        var updated =
            await _parentService
                .SetParentActiveStatusAsync(
                    schoolId,
                    id,
                    true);


        TempData[
            updated
                ? "SuccessMessage"
                : "ErrorMessage"
        ] =
            updated
                ? "Parent reactivated successfully."
                : "Parent could not be found.";


        return RedirectAfterStatusChange(
            returnUrl,
            nameof(Parents));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTeacher(
    AddTeacherViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        try
        {
            await _teacherService
                .AddTeacherAsync(
                    model,
                    schoolId);


            TempData["SuccessMessage"] =
                $"{model.FullName} was invited successfully as a teacher.";


            return RedirectToAction(
                nameof(Teachers));
        }
        catch (UnauthorizedAccessException)
        {
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
                "Unable to add teacher.");

            ModelState.AddModelError(
                "",
                "The teacher could not be added. " +
                "Please check the details and try again.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTeacher(
    Guid id,
    EditTeacherViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var updated =
                await _teacherService
                    .UpdateTeacherAsync(
                        schoolId,
                        id,
                        model);


            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Teacher could not be found.";

                return RedirectToAction(
                    nameof(Teachers));
            }


            TempData["SuccessMessage"] =
                "Teacher information updated successfully.";


            return RedirectToAction(
                nameof(TeacherDetails),
                new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update teacher {TeacherId}.",
                id);

            ModelState.AddModelError(
                "",
                "The teacher could not be updated.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateTeacher(
    Guid id,
    string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        var updated =
            await _teacherService
                .SetTeacherActiveStatusAsync(
                    schoolId,
                    id,
                    false);


        TempData[
            updated
                ? "SuccessMessage"
                : "ErrorMessage"
        ] =
            updated
                ? "Teacher deactivated successfully."
                : "Teacher could not be found.";


        return RedirectAfterStatusChange(
            returnUrl,
            nameof(Teachers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivateTeacher(
    Guid id,
    string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        var updated =
            await _teacherService
                .SetTeacherActiveStatusAsync(
                    schoolId,
                    id,
                    true);


        TempData[
            updated
                ? "SuccessMessage"
                : "ErrorMessage"
        ] =
            updated
                ? "Teacher reactivated successfully."
                : "Teacher could not be found.";


        return RedirectAfterStatusChange(
            returnUrl,
            nameof(Teachers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
    EditLearner(
        Guid id,
        EditLearnerViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (id != model.Id)
        {
            return BadRequest();
        }


        if (!ModelState.IsValid)
        {
            await PopulateEditLearnerFormAsync(
                model,
                schoolId);

            return View(model);
        }


        try
        {
            await _learnerService
                .UpdateLearnerAsync(
                    model,
                    schoolId);

            TempData["SuccessMessage"] =
                $"{model.FullName} was updated successfully.";

            return RedirectToAction(
                nameof(LearnerDetails),
                new { id = model.Id });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            await PopulateEditLearnerFormAsync(
                model,
                schoolId);

            ModelState.AddModelError(
                "",
                ex.Message);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update learner {LearnerId}.",
                model.Id);

            await PopulateEditLearnerFormAsync(
                model,
                schoolId);

            ModelState.AddModelError(
                "",
                "The learner could not be updated. Please try again.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLearner(
    AddLearnerViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        if (!ModelState.IsValid)
        {
            model.AvailableClasses =
                await _learnerService
                    .GetAvailableClassesAsync(
                        schoolId);

            return View(model);
        }

        try
        {
            await _learnerService
                .AddLearnerAsync(
                    model,
                    schoolId);

            TempData["SuccessMessage"] =
                $"{model.FullName} was added successfully.";

            return RedirectToAction(
                nameof(Learners));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            model.AvailableClasses =
                await _learnerService
                    .GetAvailableClassesAsync(
                        schoolId);

            ModelState.AddModelError(
                "",
                ex.Message);

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to add learner.");

            model.AvailableClasses =
                await _learnerService
                    .GetAvailableClassesAsync(
                        schoolId);

            ModelState.AddModelError(
                "",
                "The learner could not be added. Please try again.");

            return View(model);
        }
    }
    [HttpGet]
    public async Task<IActionResult>
    EditLearner(Guid id)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _learnerService
                    .GetEditLearnerAsync(
                        id,
                        schoolId);

            if (model == null)
            {
                TempData["ErrorMessage"] =
                    "The learner could not be found.";

                return RedirectToAction(
                    nameof(Learners));
            }

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load learner {LearnerId} for editing.",
                id);

            TempData["ErrorMessage"] =
                "The learner could not be loaded.";

            return RedirectToAction(
                nameof(Learners));
        }
    }
    private async Task PopulateEditLearnerFormAsync(
    EditLearnerViewModel model,
    Guid schoolId)
    {
        model.AvailableClasses =
            await _learnerService
                .GetAvailableClassesAsync(
                    schoolId);


        if (model.ParentId.HasValue)
        {
            var parent =
                await _learnerService
                    .GetParentSelectionAsync(
                        model.ParentId.Value,
                        schoolId);

            if (parent != null)
            {
                model.SelectedParentName =
                    parent.FullName;

                model.SelectedGuardianNumber =
                    parent.GuardianNumber;

                model.SelectedParentPhone =
                    parent.Phone ?? "";

                model.SelectedParentEmail =
                    parent.Email ?? "";
            }
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
    DeactivateLearner(Guid id,
string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        try
        {
            await _learnerService
                .SetLearnerActiveStatusAsync(
                    id,
                    schoolId,
                    false);

            TempData["SuccessMessage"] =
                "Learner deactivated successfully.";

            return RedirectAfterStatusChange(
            returnUrl,
            nameof(Learners));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;

            return RedirectToAction(
                nameof(Learners));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to deactivate learner {LearnerId}.",
                id);

            TempData["ErrorMessage"] =
                "The learner could not be deactivated.";

            return RedirectToAction(
                nameof(LearnerDetails),
                new { id });
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
    ReactivateLearner(Guid id,
string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }

        try
        {
            await _learnerService
                .SetLearnerActiveStatusAsync(
                    id,
                    schoolId,
                    true);

            TempData["SuccessMessage"] =
                "Learner reactivated successfully.";

            return RedirectAfterStatusChange(
            returnUrl,
            nameof(Learners));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;

            return RedirectToAction(
                nameof(Learners));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to reactivate learner {LearnerId}.",
                id);

            TempData["ErrorMessage"] =
                "The learner could not be reactivated.";

            return RedirectToAction(
                nameof(LearnerDetails),
                new { id });
        }
    }

    private IActionResult RedirectAfterStatusChange(
    string? returnUrl,
    string fallbackAction)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(fallbackAction);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClass(
    AddClassViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            var formModel =
                await _classService
                    .GetAddClassModelAsync(
                        schoolId,
                        model.AcademicYear);

            model.Teachers =
                formModel.Teachers;

            return View(model);
        }


        try
        {
            await _classService
                .AddClassAsync(
                    schoolId,
                    model);


            TempData["SuccessMessage"] =
                "Class added successfully.";


            return RedirectToAction(
                nameof(Classes));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to add class.");


            ModelState.AddModelError(
                "",
                "The class could not be added.");


            var formModel =
                await _classService
                    .GetAddClassModelAsync(
                        schoolId,
                        model.AcademicYear);

            model.Teachers =
                formModel.Teachers;


            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClass(
    EditClassViewModel model)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (!ModelState.IsValid)
        {
            var formModel =
                await _classService
                    .GetClassForEditAsync(
                        schoolId,
                        model.Id);


            if (formModel != null)
            {
                model.Teachers =
                    formModel.Teachers;
            }


            return View(model);
        }


        try
        {
            var updated =
                await _classService
                    .UpdateClassAsync(
                        schoolId,
                        model);


            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Class record could not be found.";

                return RedirectToAction(
                    nameof(Classes));
            }


            TempData["SuccessMessage"] =
                "Class updated successfully.";


            return RedirectToAction(
                nameof(ClassDetails),
                new
                {
                    id = model.Id
                });
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                nameof(model.ClassTeacherId),
                ex.Message);


            var formModel =
                await _classService
                    .GetClassForEditAsync(
                        schoolId,
                        model.Id);


            if (formModel != null)
            {
                model.Teachers =
                    formModel.Teachers;
            }


            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update class {ClassId}.",
                model.Id);


            ModelState.AddModelError(
                "",
                "The class could not be updated.");


            var formModel =
                await _classService
                    .GetClassForEditAsync(
                        schoolId,
                        model.Id);


            if (formModel != null)
            {
                model.Teachers =
                    formModel.Teachers;
            }


            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateClass(
    Guid id,
    string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var updated =
                await _classService
                    .DeactivateClassAsync(
                        schoolId,
                        id);


            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Class record could not be found.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Class deactivated successfully.";
            }


            return RedirectAfterStatusChange(
                returnUrl,
                nameof(Classes));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to deactivate class {ClassId}.",
                id);


            TempData["ErrorMessage"] =
                "The class could not be deactivated.";


            return RedirectAfterStatusChange(
                returnUrl,
                nameof(Classes));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivateClass(
        Guid id,
        string? returnUrl)
    {
        var schoolIdValue =
            User.FindFirst("school_id")?.Value;


        if (!Guid.TryParse(
                schoolIdValue,
                out var schoolId))
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var updated =
                await _classService
                    .ReactivateClassAsync(
                        schoolId,
                        id);


            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "Class record could not be found.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Class reactivated successfully.";
            }


            return RedirectAfterStatusChange(
                returnUrl,
                nameof(Classes));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to reactivate class {ClassId}.",
                id);


            TempData["ErrorMessage"] =
                "The class could not be reactivated.";


            return RedirectAfterStatusChange(
                returnUrl,
                nameof(Classes));
        }
    }
}