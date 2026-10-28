using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.ViewComponents;

//
// Class Name       : TopbarNotificationsViewComponent
// Purpose          : Displays the actual unread notification count for the current user.
// Re-use           : School Admin and Teacher top bars.
// Input Parameters : Supabase client, current session and logger.
// Output Type      : ViewComponent.
//
public class TopbarNotificationsViewComponent : ViewComponent
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _session;
    private readonly ILogger<TopbarNotificationsViewComponent> _logger;

    public TopbarNotificationsViewComponent(
        Supabase.Client supabase,
        SupabaseSessionService session,
        ILogger<TopbarNotificationsViewComponent> logger)
    {
        _supabase = supabase;
        _session = session;
        _logger = logger;
    }

    //
    // Method Name      : InvokeAsync
    // Purpose          : Read the signed-in user's notifications.
    // Re-use           : Invoked from _SchoolAdminLayout.cshtml.
    // Input Parameters : None.
    // Output Type      : Task<IViewComponentResult>.
    //
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var model = new TopbarNotificationsViewModel();
        var idValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(idValue, out var userId) ||
            HttpContext.User.Identity?.IsAuthenticated != true)
        {
            return View(model);
        }

        try
        {
            if (!await _session.RestoreAsync())
            {
                model.LoadFailed = true;
                return View(model);
            }

            // Supabase RLS additionally restricts rows to the signed-in user.
            var response = await _supabase
                .From<TopbarNotificationRecord>()
                .Where(x => x.UserId == userId)
                .Get();

            var ordered = response.Models
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            model.UnreadCount = ordered.Count(x => !x.IsRead);
            model.Notifications = ordered.Take(5).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to load the topbar notifications.");
            model.LoadFailed = true;
        }

        return View(model);
    } // end method
}
