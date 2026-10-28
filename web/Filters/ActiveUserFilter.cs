using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using SchoolParentalSystem.Web.Controllers;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services.SuperAdmin;

namespace SchoolParentalSystem.Web.Filters;

public class ActiveUserFilter : IAsyncActionFilter
{
    private readonly Supabase.Client
        _supabase;

    private readonly SupabaseSessionService
        _supabaseSession;

    private readonly ILogger<ActiveUserFilter>
        _logger;


    public ActiveUserFilter(
        Supabase.Client supabase,
        SupabaseSessionService supabaseSession,
        ILogger<ActiveUserFilter> logger)
    {
        _supabase =
            supabase;

        _supabaseSession =
            supabaseSession;

        _logger =
            logger;
    }


    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var httpContext =
            context.HttpContext;


        // -----------------------------------------
        // NOT LOGGED IN
        // -----------------------------------------

        if (httpContext.User.Identity
                ?.IsAuthenticated != true)
        {
            await next();

            return;
        }


        // -----------------------------------------
        // DO NOT CHECK AUTH CONTROLLER
        // -----------------------------------------

        if (context.Controller
            is AuthController)
        {
            await next();

            return;
        }


        var userIdValue =
            httpContext.User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (!Guid.TryParse(
                userIdValue,
                out var userId))
        {
            await SignOutAsync(
                context);

            return;
        }


        try
        {
            // -------------------------------------
            // RESTORE SUPABASE SESSION
            // -------------------------------------

            var restored =
                await _supabaseSession
                    .RestoreAsync();


            if (!restored)
            {
                await SignOutAsync(
                    context);

                return;
            }


            // -------------------------------------
            // CHECK CURRENT USER STATUS
            // -------------------------------------

            var response =
                await _supabase
                    .From<UserProfile>()
                    .Where(
                        user =>
                            user.Id == userId)
                    .Get();


            var profile =
                response.Models
                    .SingleOrDefault();


            // User deleted / profile unavailable.
            if (profile == null)
            {
                await SignOutAsync(
                    context);

                return;
            }


            // -------------------------------------
            // DEACTIVATED ACCOUNT
            // -------------------------------------

            if (!profile.IsActive)
            {
                _logger.LogInformation(
                    "User {UserId} was signed out because the account is inactive.",
                    userId);


                await SignOutAsync(
                    context,
                    "Your account has been deactivated. Please contact your administrator.");

                return;
            }


            // -------------------------------------
            // USER STILL ACTIVE
            // -------------------------------------

            await next();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to validate active status for user {UserId}.",
                userId);


            // Do not automatically deactivate/sign out a
            // valid user because Supabase temporarily failed.
            await next();
        }
    }


    private async Task SignOutAsync(
        ActionExecutingContext context,
        string? message = null)
    {
        var httpContext =
            context.HttpContext;


        try
        {
            var restored =
                await _supabaseSession
                    .RestoreAsync();


            if (restored)
            {
                await _supabase.Auth.SignOut(
                    Supabase.Gotrue.Constants
                        .SignOutScope.Local);
            }
        }
        catch
        {
            // Continue ASP.NET sign-out even if
            // Supabase sign-out fails.
        }


        _supabaseSession.Clear();

        httpContext.Session.Clear();


        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme);


        if (!string.IsNullOrWhiteSpace(
                message) &&
            context.Controller
                is Controller controller)
        {
            controller.TempData["LoginMessage"] =
                message;
        }


        context.Result =
            new RedirectToActionResult(
                "Login",
                "Auth",
                null);
    }
}