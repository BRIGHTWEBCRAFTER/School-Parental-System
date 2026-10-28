using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Controllers;

// Class Name       : SuperAdminTopbarController
// Purpose          : Search the schools and users already managed by the Super Admin.
// Re-use           : Search form shared by Super Admin pages.
// Input Parameters : Existing Super Admin school/user services and logger.
// Output Type      : MVC controller.
[Authorize(Roles = "Super Admin")]
public class SuperAdminTopbarController : Controller
{
    private readonly SuperAdminSchoolService _schools;
    private readonly SuperAdminUserService _users;
    private readonly ILogger<SuperAdminTopbarController> _logger;

    public SuperAdminTopbarController(
        SuperAdminSchoolService schools,
        SuperAdminUserService users,
        ILogger<SuperAdminTopbarController> logger)
    {
        _schools = schools;
        _users = users;
        _logger = logger;
    }

    // Method Name      : Search
    // Purpose          : Search by school name/code/city and user name/email/role/school.
    // Re-use           : Super Admin top bar.
    // Input Parameters : string? query.
    // Output Type      : Task<IActionResult>.
    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        string text = (query ?? "").Trim();
        var model = new SuperAdminTopbarSearchViewModel { Query = text };

        if (text.Length == 0)
            return View(model);

        if (text.Length > 100)
            return BadRequest("Search must be 100 characters or fewer.");

        try
        {
            var schools = await _schools.GetSchoolsAsync();
            var users = await _users.GetUsersAsync();

            model.Schools = schools.Schools
                .Where(s => Contains(s.SchoolName, text)
                    || Contains(s.SchoolCode, text)
                    || Contains(s.Province, text)
                    || Contains(s.TownCity, text))
                .Take(40).ToList();

            model.Users = users.Users
                .Where(u => Contains(u.FullName, text)
                    || Contains(u.Email, text)
                    || Contains(u.Role, text)
                    || Contains(u.SchoolName, text))
                .Take(40).ToList();
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Super Admin top bar search failed.");
            model.LoadFailed = true;
        }

        return View(model);
    }

    private static bool Contains(string? source, string needle) =>
        !string.IsNullOrWhiteSpace(source) &&
        source.Contains(needle, StringComparison.OrdinalIgnoreCase);
}
