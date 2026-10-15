using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Controllers;

public class AuthController : Controller
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _supabaseSession;

    private readonly ILogger<AuthController>
        _logger;


    public AuthController(
        Supabase.Client supabase,
        SupabaseSessionService supabaseSession,
        ILogger<AuthController> logger)
    {
        _supabase = supabase;

        _supabaseSession = supabaseSession;

        _logger = logger;
    }


    // -----------------------------------------------------
    // LOGIN PAGE
    // -----------------------------------------------------

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }

        return View(new LoginViewModel());
    }


    // -----------------------------------------------------
    // LOGIN SUBMIT
    // -----------------------------------------------------

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        try
        {
            var email =
                model.Email.Trim().ToLowerInvariant();


            // Authenticate against Supabase Auth.
            var authSession =
                await _supabase.Auth.SignIn(
                    email,
                    model.Password);


            if (authSession == null ||
                authSession.User == null ||
                string.IsNullOrWhiteSpace(
                    authSession.AccessToken) ||
                string.IsNullOrWhiteSpace(
                    authSession.RefreshToken))
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }


            if (!Guid.TryParse(
                    authSession.User.Id,
                    out var userId))
            {
                await SafeSupabaseSignOut();

                ModelState.AddModelError(
                    "",
                    "Your account could not be verified.");

                return View(model);
            }


            // The Supabase session is now attached to this
            // scoped client, so RLS applies to this query.
            var profileResponse =
                await _supabase
                    .From<UserProfile>()
                    .Where(user =>
                        user.Id == userId)
                    .Get();


            var profile =
                profileResponse.Models
                    .SingleOrDefault();


            if (profile == null)
            {
                await SafeSupabaseSignOut();

                ModelState.AddModelError(
                    "",
                    "No application profile exists for this account.");

                return View(model);
            }


            if (!profile.IsActive)
            {
                await SafeSupabaseSignOut();

                ModelState.AddModelError(
                    "",
                    "This account is inactive.");

                return View(model);
            }


            // Learner is never an authentication role.
            // Parent belongs primarily to the Flutter app.
            var allowedWebRoles = new[]
            {
                "Super Admin",
                "School Admin",
                "Teacher"
            };


            if (!allowedWebRoles.Contains(profile.Role))
            {
                await SafeSupabaseSignOut();

                ModelState.AddModelError(
                    "",
                    "This account does not have access to the web portal.");

                return View(model);
            }


            // Save Supabase session tokens server-side.
            _supabaseSession.Store(
                authSession.AccessToken,
                authSession.RefreshToken);


            // Create ASP.NET identity claims.
            var claims =
                new List<Claim>
                {
                    new(
                        ClaimTypes.NameIdentifier,
                        profile.Id.ToString()),

                    new(
                        ClaimTypes.Name,
                        profile.FullName),

                    new(
                        ClaimTypes.Email,
                        profile.Email ?? email),

                    new(
                        ClaimTypes.Role,
                        profile.Role)
                };


            if (profile.SchoolId.HasValue)
            {
                claims.Add(
                    new Claim(
                        "school_id",
                        profile.SchoolId.Value
                            .ToString()));
            }


            var identity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults
                        .AuthenticationScheme);


            var principal =
                new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                principal);


            // We currently have the School Admin
            // dashboard implemented.
            if (profile.Role == "School Admin")
            {
                return RedirectToAction(
                    "Dashboard",
                    "SchoolAdmin");
            }


            // Super Admin / Teacher dashboards will be
            // connected when those sections are built.
            return RedirectToAction(
                "Index",
                "Home");
        }
        catch (Exception ex)
        {
            // Do not log passwords or auth tokens.
            _logger.LogWarning(
                ex,
                "A Supabase login attempt failed.");


            ModelState.AddModelError(
                "",
                "Unable to sign in. Check your email and password.");

            return View(model);
        }
    }

    // -----------------------------------------------------
    // Reset Password
    // -----------------------------------------------------
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var redirectUrl = Url.Action(
                nameof(ResetPassword),
                "Auth",
                null,
                Request.Scheme);

            var options =
                new Supabase.Gotrue.ResetPasswordForEmailOptions(
                    model.Email.Trim().ToLowerInvariant())
                {
                    RedirectTo = redirectUrl
                };

            await _supabase.Auth
                .ResetPasswordForEmail(options);

            // Do not reveal whether an email exists.
            ViewBag.SuccessMessage =
                "If an account exists for that email, a password reset link has been sent.";

            ModelState.Clear();

            return View(new ForgotPasswordViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Password reset email request failed.");

            ModelState.AddModelError(
                "",
                "We could not send the reset email right now. Please try again.");

            return View(model);
        }
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword()
    {
        return View(new ResetPasswordViewModel());
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.AccessToken) ||
            string.IsNullOrWhiteSpace(model.RefreshToken))
        {
            ModelState.AddModelError(
                "",
                "The password reset link is invalid or has expired.");

            return View(model);
        }

        try
        {
            var session =
                await _supabase.Auth.SetSession(
                    model.AccessToken,
                    model.RefreshToken);

            if (session == null)
            {
                ModelState.AddModelError(
                    "",
                    "The password reset link is invalid or has expired.");

                return View(model);
            }

            var attributes =
                new Supabase.Gotrue.UserAttributes
                {
                    Password = model.NewPassword
                };

            await _supabase.Auth.Update(attributes);

            await _supabase.Auth.SignOut(
                Supabase.Gotrue.Constants
                    .SignOutScope.Local);

            TempData["LoginMessage"] =
                "Your password has been changed successfully. You can now sign in.";

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Password update failed.");

            ModelState.AddModelError(
                "",
                "The password could not be changed. The reset link may have expired.");

            return View(model);
        }
    }


    // -----------------------------------------------------
    // LOGOUT
    // -----------------------------------------------------

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var restored =
                await _supabaseSession
                    .RestoreAsync();


            if (restored)
            {
                // Log out this browser/session only.
                await _supabase.Auth.SignOut(
                    Supabase.Gotrue.Constants
                        .SignOutScope.Local);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Supabase sign-out failed.");
        }
        finally
        {
            _supabaseSession.Clear();

            HttpContext.Session.Clear();


            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);
        }


        return RedirectToAction(
            nameof(Login));
    }


    // -----------------------------------------------------
    // ACCESS DENIED
    // -----------------------------------------------------

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


    private IActionResult
        RedirectAuthenticatedUser()
    {
        var role =
            User.FindFirstValue(
                ClaimTypes.Role);


        return role switch
        {
            "School Admin" =>
                RedirectToAction(
                    "Dashboard",
                    "SchoolAdmin"),

            _ =>
                RedirectToAction(
                    "Index",
                    "Home")
        };
    }


    private async Task SafeSupabaseSignOut()
    {
        try
        {
            await _supabase.Auth.SignOut(
                Supabase.Gotrue.Constants
                    .SignOutScope.Local);
        }
        catch
        {
            // Ignore cleanup failure.
        }


        _supabaseSession.Clear();
    }

    /*=======================================
     INVITATION LINK
     =========================================*/
    [AllowAnonymous]
    [HttpGet]
    public IActionResult AcceptInvitation()
    {
        return View();
    }
}