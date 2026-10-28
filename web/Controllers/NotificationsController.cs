using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Controllers;

//
// Class Name       : NotificationsController
// Purpose          : Lists and acknowledges real user notifications.
// Re-use           : Shared Teacher and School Admin topbar.
// Input Parameters : Supabase client, current session and logger.
// Output Type      : MVC controller.
//
[Authorize(Roles = "School Admin,Teacher,Super Admin")]
public class NotificationsController : Controller
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _session;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        Supabase.Client supabase,
        SupabaseSessionService session,
        ILogger<NotificationsController> logger)
    {
        _supabase = supabase;
        _session = session;
        _logger = logger;
    }

    //
    // Method Name      : GetSignedInUserIdAsync
    // Purpose          : Validate the user ID and restore the Supabase session.
    // Re-use           : All notification actions.
    // Input Parameters : None.
    // Output Type      : Task<Guid?>.
    //
    private async Task<Guid?> GetSignedInUserIdAsync()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(value, out var id) || id == Guid.Empty)
            return null;

        return await _session.RestoreAsync() ? id : null;
    }

    //
    // Method Name      : Index
    // Purpose          : Show the signed-in user's notification history.
    // Re-use           : View all notifications link in the top bar.
    // Input Parameters : None.
    // Output Type      : Task<IActionResult>.
    //
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = await GetSignedInUserIdAsync();
        if (!userId.HasValue)
            return RedirectToAction("Login", "Auth");

        var model = new TopbarNotificationsViewModel();

        try
        {
            var response = await _supabase
                .From<TopbarNotificationRecord>()
                .Where(x => x.UserId == userId.Value)
                .Get();

            var ordered = response.Models
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            model.UnreadCount = ordered.Count(x => !x.IsRead);
            model.Notifications = ordered.Take(100).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to list user notifications.");
            model.LoadFailed = true;
        }

        return View(model);
    }

    //
    // Method Name      : MarkRead
    // Purpose          : Mark one notification belonging to this user as read.
    // Re-use           : Topbar notification list and full notification page.
    // Input Parameters : Guid id.
    // Output Type      : Task<IActionResult>.
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var userId = await GetSignedInUserIdAsync();
        if (!userId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (id == Guid.Empty)
            return BadRequest();

        try
        {
            await _supabase
                .From<TopbarNotificationRecord>()
                .Where(x => x.Id == id)
                .Where(x => x.UserId == userId.Value)
                .Set(x => x.IsRead, true)
                .Set(x => x.ReadAt, DateTime.UtcNow)
                .Update();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not mark notification {NotificationId} as read.", id);
            TempData["ErrorMessage"] = "Could not mark notification as read.";
        }

        return RedirectToAction(nameof(Index));
    }

    //
    // Method Name      : MarkAllRead
    // Purpose          : Mark all of the signed-in user's notifications as read.
    // Re-use           : Notifications page.
    // Input Parameters : None.
    // Output Type      : Task<IActionResult>.
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = await GetSignedInUserIdAsync();
        if (!userId.HasValue)
            return RedirectToAction("Login", "Auth");

        try
        {
            await _supabase
                .From<TopbarNotificationRecord>()
                .Where(x => x.UserId == userId.Value)
                .Where(x => x.IsRead == false)
                .Set(x => x.IsRead, true)
                .Set(x => x.ReadAt, DateTime.UtcNow)
                .Update();

            TempData["SuccessMessage"] = "All notifications marked as read.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not mark all notifications as read.");
            TempData["ErrorMessage"] = "Unable to update notifications.";
        }

        return RedirectToAction(nameof(Index));
    }
}
