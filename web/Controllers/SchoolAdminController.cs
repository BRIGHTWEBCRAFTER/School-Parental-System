using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.ViewModels;
using SchoolParentalSystem.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SchoolParentalSystem.Web.Controllers;

[Authorize(Roles = "School Admin")]
public class SchoolAdminController : Controller
{
    private readonly SchoolAdminLearnerService _learnerService;
    private readonly ILogger<SchoolAdminController> _logger;
    private readonly SchoolAdminTeacherService _teacherService;

    public SchoolAdminController(
    SchoolAdminLearnerService learnerService,
    SchoolAdminTeacherService teacherService,
    ILogger<SchoolAdminController> logger)
    {
        _learnerService = learnerService;
        _teacherService = teacherService;
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
    DeactivateLearner(Guid id)
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

            return RedirectToAction(
                nameof(LearnerDetails),
                new { id });
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
    ReactivateLearner(Guid id)
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

            return RedirectToAction(
                nameof(LearnerDetails),
                new { id });
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
}