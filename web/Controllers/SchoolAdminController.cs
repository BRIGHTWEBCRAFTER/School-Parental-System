using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.ViewModels;
using SchoolParentalSystem.Web.Services;

namespace SchoolParentalSystem.Web.Controllers;

[Authorize(Roles = "School Admin")]
public class SchoolAdminController : Controller
{
    private readonly SchoolAdminLearnerService _learnerService;
    private readonly ILogger<SchoolAdminController> _logger;

    public SchoolAdminController(
        SchoolAdminLearnerService learnerService,
        ILogger<SchoolAdminController> logger)
    {
        _learnerService = learnerService;
        _logger = logger;
    }
    public IActionResult Dashboard()
    {
        var model = new SchoolAdminDashboardViewModel
        {
            AdminName = "School Admin",
            SchoolName = "Goldfield High School",

            TotalLearners = 248,
            TotalTeachers = 34,
            TotalClasses = 12,
            AttendanceRate = 94.2m,
            ActiveAnnouncements = 6,

            AttendanceOverview =
            [
                new() { Date = "Sep 15", Percentage = 91 },
                new() { Date = "Sep 16", Percentage = 95 },
                new() { Date = "Sep 17", Percentage = 92 },
                new() { Date = "Sep 18", Percentage = 89 },
                new() { Date = "Sep 19", Percentage = 94 }
            ],

            RecentAnnouncements =
            [
                new()
                {
                    Title = "Parent-Teacher Meeting",
                    Description = "Scheduled for Sep 28, 2026."
                },
                new()
                {
                    Title = "School Holiday Notice",
                    Description = "School will be closed on Heritage Day."
                },
                new()
                {
                    Title = "Term 4 Assessment",
                    Description = "Assessment schedules are now available."
                }
            ],

            UpcomingEvents =
            [
                new()
                {
                    Month = "SEP",
                    Day = "22",
                    Title = "Parent-Teacher Meeting"
                },
                new()
                {
                    Month = "SEP",
                    Day = "25",
                    Title = "Sports Day"
                },
                new()
                {
                    Month = "OCT",
                    Day = "03",
                    Title = "Career Guidance Seminar"
                }
            ],

            RecentActivities =
            [
                new()
                {
                    Description = "A learner was added to the system",
                    TimeAgo = "10m ago",
                    Type = "learner"
                },
                new()
                {
                    Description = "Attendance updated for Grade 10A",
                    TimeAgo = "1h ago",
                    Type = "attendance"
                },
                new()
                {
                    Description = "Term grades were updated",
                    TimeAgo = "3h ago",
                    Type = "grade"
                },
                new()
                {
                    Description = "A new announcement was posted",
                    TimeAgo = "5h ago",
                    Type = "announcement"
                }
            ],

            ClassPerformance =
            [
                new()
                {
                    ClassName = "Grade 8A",
                    LearnerCount = 28,
                    AverageGrade = 88.4m,
                    Status = "Good"
                },
                new()
                {
                    ClassName = "Grade 9A",
                    LearnerCount = 26,
                    AverageGrade = 85.7m,
                    Status = "Good"
                },
                new()
                {
                    ClassName = "Grade 10A",
                    LearnerCount = 33,
                    AverageGrade = 82.1m,
                    Status = "Needs attention"
                },
                new()
                {
                    ClassName = "Grade 11A",
                    LearnerCount = 27,
                    AverageGrade = 89.2m,
                    Status = "Good"
                },
                new()
                {
                    ClassName = "Grade 12A",
                    LearnerCount = 35,
                    AverageGrade = 91.5m,
                    Status = "Excellent"
                }
            ]
        };

        return View(model);
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


    public IActionResult LearnerDetails(string id)
    {
        var model = new LearnerDetailsViewModel
        {
            Id = id,
            AdmissionNumber = "GF-2026-001",
            FullName = "Sandile Motone",
            DateOfBirth = new DateTime(2010, 6, 15),
            Gender = "Male",
            Grade = "Grade 10",
            ClassName = "Grade 10 A",
            Address = "Welkom, Free State",
            GuardianName = "Xolani mNgadi",
            Relationship = "Guardian",
            IsActive = true
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult EditLearner(string id)
    {
        var model = new EditLearnerViewModel
        {
            Id = id,
            AdmissionNumber = "GF-2026-001",
            FullName = "Sandile Motone",
            DateOfBirth = new DateTime(2010, 6, 15),
            Gender = "Male",
            Grade = "Grade 10",
            ClassName = "Grade 10 A",
            Address = "Welkom, Free State",
            GuardianName = "Xolani mNgadi",
            Relationship = "Guardian",
            IsActive = true
        };

        return View(model);
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
    public IActionResult EditLearner(EditLearnerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Real Supabase UPDATE will be added later.

        TempData["SuccessMessage"] =
            $"{model.FullName}'s learner record was updated.";

        return RedirectToAction(
            nameof(LearnerDetails),
            new { id = model.Id }
        );
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
}