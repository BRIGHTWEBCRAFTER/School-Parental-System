using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels;
using Supabase.Gotrue;
using System.Security.Claims;


namespace SchoolParentalSystem.Web.Controllers;


//
// Class Name       : AuthController
// Purpose           : Handles sign-in, logout, account invitations and password recovery.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
public class AuthController : Controller
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService _supabaseSession;

    private readonly ILogger<AuthController> _logger;


    //
    // Method Name       : AuthController
    // Purpose           : Initializes required dependencies for AuthController.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : Supabase.Client supabase; SupabaseSessionService supabaseSession;
    //                      ILogger<AuthController> logger
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public AuthController(
        Supabase.Client supabase,
        SupabaseSessionService supabaseSession,
        ILogger<AuthController> logger)
    {
        _supabase = supabase;
        _supabaseSession = supabaseSession;
        _logger = logger;
    }


    // =====================================================
    // LOGIN PAGE
    // =====================================================

    //
    // Method Name       : Login
    // Purpose           : Show sign-in screen or authenticate the submitted credentials.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectAuthenticatedUser();
        }


        return View(
            new LoginViewModel());
    }


    // =====================================================
    // LOGIN SUBMIT
    // =====================================================

    //
    // Method Name       : Login
    // Purpose           : Show sign-in screen or authenticate the submitted credentials.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : LoginViewModel model
    // Output Type       : Task<IActionResult>
    //
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
                model.Email
                    .Trim()
                    .ToLowerInvariant();


            var selectedRole =
                model.Role
                    .Trim();


            // -------------------------------------------------
            // AUTHENTICATE WITH SUPABASE
            // -------------------------------------------------

            var authSession =
                await _supabase.Auth
                    .SignIn(
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


            // -------------------------------------------------
            // LOAD APPLICATION PROFILE
            // -------------------------------------------------

            var profileResponse =
                await _supabase
                    .From<UserProfile>()
                    .Where(
                        user =>
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


            // -------------------------------------------------
            // ACCOUNT STATUS
            // -------------------------------------------------

            if (!profile.IsActive)
            {
                await SafeSupabaseSignOut();


                ModelState.AddModelError(
                    "",
                    "This account has been deactivated. Please contact your school administrator.");


                return View(model);
            }


            //
            // Purpose : Prevent a Teacher from accessing
            //           the portal before account activation.
            //

            if (profile.Role == "Teacher" &&
                profile.RequiresPasswordChange)
            {
                await SafeSupabaseSignOut();

                ModelState.AddModelError(
                    "",
                    "Your account requires activation. " +
                    "Use the temporary PIN sent to your email " +
                    "to create your password.");

                return View(model);
            }



            // -------------------------------------------------
            // WEB ACCESS ROLES
            // -------------------------------------------------

            var allowedWebRoles =
                new[]
                {
                    "Super Admin",
                    "School Admin",
                    "Teacher"
                };


            if (string.IsNullOrWhiteSpace(
                    profile.Role) ||
                !allowedWebRoles.Contains(
                    profile.Role))
            {
                await SafeSupabaseSignOut();


                ModelState.AddModelError(
                    "",
                    "This account does not have access to the web portal.");


                return View(model);
            }


            // -------------------------------------------------
            // SELECTED ROLE MUST MATCH DATABASE ROLE
            // -------------------------------------------------
            //
            // This prevents:
            //
            // School Admin -> selecting Teacher
            // Super Admin  -> selecting Teacher
            //
            // to avoid the Access Code requirement.
            // -------------------------------------------------

            if (!string.Equals(
                    selectedRole,
                    profile.Role,
                    StringComparison.OrdinalIgnoreCase))
            {
                await SafeSupabaseSignOut();


                ModelState.AddModelError(
                    nameof(LoginViewModel.Role),
                    "The selected role does not match this account.");


                return View(model);
            }


            // -------------------------------------------------
            // SCHOOL STATUS
            // -------------------------------------------------

            if (profile.SchoolId.HasValue &&
                !string.Equals(
                    profile.Role,
                    "Super Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                var schoolResponse =
                    await _supabase
                        .From<SchoolStatusRecord>()
                        .Where(
                            school =>
                                school.Id ==
                                profile.SchoolId.Value)
                        .Get();


                var schoolStatus =
                    schoolResponse.Models
                        .FirstOrDefault()?
                        .Status;


                if (string.Equals(
                    schoolStatus,
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
                {
                    await SafeSupabaseSignOut();


                    ModelState.AddModelError(
                        "",
                        "Your school has been deactivated. Please contact the system administrator.");


                    return View(model);
                }
            }


            // -------------------------------------------------
            // ADMIN ACCESS CODE
            // -------------------------------------------------
            //
            // Super Admin:
            // Email + Password + Access Code
            //
            // School Admin:
            // Email + Password + Access Code
            //
            // Teacher:
            // Email + Password only
            // -------------------------------------------------

            var requiresAccessCode =
                string.Equals(
                    profile.Role,
                    "Super Admin",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    profile.Role,
                    "School Admin",
                    StringComparison.OrdinalIgnoreCase);


            if (requiresAccessCode)
            {
                if (string.IsNullOrWhiteSpace(
                    model.AccessCode))
                {
                    await SafeSupabaseSignOut();


                    ModelState.AddModelError(
                        nameof(LoginViewModel.AccessCode),
                        "Access Code is required for this role.");


                    return View(model);
                }


                var accessCodeValid =
                    await VerifyLoginAccessCodeAsync(
                        model.AccessCode);


                if (!accessCodeValid)
                {
                    await SafeSupabaseSignOut();


                    ModelState.AddModelError(
                        nameof(LoginViewModel.AccessCode),
                        "The Access Code is invalid.");


                    return View(model);
                }
            }


            // -------------------------------------------------
            // STORE SUPABASE SESSION
            // -------------------------------------------------

            _supabaseSession.Store(
                authSession.AccessToken,
                authSession.RefreshToken);


            // -------------------------------------------------
            // ASP.NET IDENTITY CLAIMS
            // -------------------------------------------------

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
                        profile.Role),

                    new(
                        "login_at",
                        DateTime.UtcNow.ToString("o"))
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
                new ClaimsPrincipal(
                    identity);


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                principal);


            // -------------------------------------------------
            // ROLE REDIRECT
            // -------------------------------------------------

            if (string.Equals(
                profile.Role,
                "Super Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    "Dashboard",
                    "SuperAdmin");
            }


            if (string.Equals(
                profile.Role,
                "School Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    "Dashboard",
                    "SchoolAdmin");
            }


            return RedirectToAction(
                "Dashboard",
                "Teacher");
        }
        catch (Exception ex)
        {
            // Never log passwords, access codes,
            // Supabase auth tokens or refresh tokens.

            _logger.LogWarning(
                ex,
                "A Supabase login attempt failed.");


            await SafeSupabaseSignOut();


            ModelState.AddModelError(
                "",
                "Unable to sign in. Please check your login details.");


            return View(model);
        }
    }


    // =====================================================
    // VERIFY ADMIN LOGIN ACCESS CODE
    // =====================================================

    //
    // Method Name       : VerifyLoginAccessCodeAsync
    // Purpose           : Handle the verify login access code operation.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : string accessCode
    // Output Type       : Task<bool>
    //
    private async Task<bool>
        VerifyLoginAccessCodeAsync(
            string accessCode)
    {
        try
        {
            var parameters =
                new Dictionary<string, object>
                {
                    {
                        "p_code",
                        accessCode.Trim()
                    }
                };


            var response =
                await _supabase.Rpc(
                    "verify_login_access_code",
                    parameters);


            if (response == null ||
                string.IsNullOrWhiteSpace(
                    response.Content))
            {
                return false;
            }


            var result =
                response.Content
                    .Trim()
                    .Trim('"');


            return bool.TryParse(
                       result,
                       out var isValid)
                   &&
                   isValid;
        }
        catch (Exception ex)
        {
            // Never log the actual access code.

            _logger.LogWarning(
                ex,
                "Login access code verification failed.");


            return false;
        }
    }


    // =====================================================
    // FORGOT PASSWORD
    // GET
    // =====================================================

    //
    // Method Name       : ForgotPassword
    // Purpose           : Display or process a password recovery request.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(
            new ForgotPasswordViewModel());
    }


    // =====================================================
    // FORGOT PASSWORD
    // POST
    // =====================================================

    //
    // Method Name       : ForgotPassword
    // Purpose           : Display or process a password recovery request.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : ForgotPasswordViewModel model
    // Output Type       : Task<IActionResult>
    //
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
            var redirectUrl =
                Url.Action(
                    nameof(ResetPassword),
                    "Auth",
                    values: null,
                    protocol: Request.Scheme);


            if (string.IsNullOrWhiteSpace(
                redirectUrl))
            {
                throw new InvalidOperationException(
                    "Password reset URL could not be created.");
            }


            var options =
                new ResetPasswordForEmailOptions(
                    model.Email
                        .Trim()
                        .ToLowerInvariant())
                {
                    RedirectTo =
                        redirectUrl,

                    FlowType =
                        Constants
                            .OAuthFlowType
                            .Implicit
                };


            await _supabase.Auth
                .ResetPasswordForEmail(
                    options);


            // Do not reveal whether the email exists.

            ViewBag.SuccessMessage =
                "If an account exists for that email, a password reset link has been sent.";


            ModelState.Clear();


            return View(
                new ForgotPasswordViewModel());
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


    // =====================================================
    // RESET PASSWORD
    // GET
    // =====================================================

    //
    // Method Name       : ResetPassword
    // Purpose           : Display or process the password reset form.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword()
    {
        ViewData["Title"] =
            "Reset Password";


        return View(
            new ResetPasswordViewModel());
    }


    // =====================================================
    // RESET PASSWORD
    // POST
    // =====================================================

    //
    // Method Name       : ResetPassword
    // Purpose           : Display or process the password reset form.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : ResetPasswordViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model)
    {
        ViewData["Title"] =
            "Reset Password";


        if (!ModelState.IsValid)
        {
            return View(model);
        }


        // -------------------------------------------------
        // RECOVERY TOKENS
        // -------------------------------------------------

        if (string.IsNullOrWhiteSpace(
                model.AccessToken) ||
            string.IsNullOrWhiteSpace(
                model.RefreshToken))
        {
            ModelState.AddModelError(
                "",
                "The password reset link is invalid or has expired. Please request a new reset link.");


            return View(model);
        }


        try
        {
            // -------------------------------------------------
            // CREATE RECOVERY SESSION
            // -------------------------------------------------

            var recoverySession =
                await _supabase.Auth
                    .SetSession(
                        model.AccessToken,
                        model.RefreshToken);


            if (recoverySession == null)
            {
                ModelState.AddModelError(
                    "",
                    "The password reset link is invalid or has expired. Please request a new reset link.");


                return View(model);
            }


            // -------------------------------------------------
            // CHANGE PASSWORD
            // -------------------------------------------------

            var updatedUser =
                await _supabase.Auth
                    .Update(
                        new UserAttributes
                        {
                            Password =
                                model.NewPassword
                        });


            if (updatedUser == null)
            {
                ModelState.AddModelError(
                    "",
                    "Your password could not be updated.");


                return View(model);
            }


            // -------------------------------------------------
            // CLEAR RECOVERY / LOGIN SESSIONS
            // -------------------------------------------------

            await SafeSupabaseSignOut();


            HttpContext.Session.Clear();


            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);


            // -------------------------------------------------
            // SUCCESS
            // -------------------------------------------------

            TempData["LoginMessage"] =
                "Your password has been changed successfully. You can now sign in with your new password.";


            return RedirectToAction(
                nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Password update failed.");


            ModelState.AddModelError(
                "",
                "The password could not be changed. The reset link may have expired. Please request a new reset link.");


            return View(model);
        }
    }


    // =====================================================
    // LOGOUT
    // =====================================================

    //
    // Method Name       : Logout
    // Purpose           : Sign out the current user and clear the authenticated session.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
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
                await _supabase.Auth
                    .SignOut(
                        Constants
                            .SignOutScope
                            .Local);
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


    // =====================================================
    // ACCESS DENIED
    // =====================================================

    //
    // Method Name       : AccessDenied
    // Purpose           : Display the page for users without access permissions.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


    // =====================================================
    // REDIRECT AUTHENTICATED USER
    // =====================================================

    //
    // Method Name       : RedirectAuthenticatedUser
    // Purpose           : Handle the redirect authenticated user operation.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    private IActionResult
        RedirectAuthenticatedUser()
    {
        var role =
            User.FindFirstValue(
                ClaimTypes.Role);


        return role switch
        {
            "Super Admin" =>
                RedirectToAction(
                    "Dashboard",
                    "SuperAdmin"),

            "School Admin" =>
                RedirectToAction(
                    "Dashboard",
                    "SchoolAdmin"),

            _ =>
                RedirectToAction(
                    "Dashboard",
                    "Teacher")
        };
    }


    // =====================================================
    // SAFE SUPABASE SIGN OUT
    // =====================================================

    //
    // Method Name       : SafeSupabaseSignOut
    // Purpose           : Attempt to clear Supabase authentication without disrupting logout.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task
    //
    private async Task SafeSupabaseSignOut()
    {
        try
        {
            await _supabase.Auth
                .SignOut(
                    Constants
                        .SignOutScope
                        .Local);
        }
        catch
        {
            // Ignore cleanup failure.
        }


        _supabaseSession.Clear();
    }


    // =====================================================
    // INVITATION LINK
    // =====================================================

    //
    // Method Name       : AcceptInvitation
    // Purpose           : Redirect an invited user into the activation flow.
    // Re-use            : Invoked by AuthController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [AllowAnonymous]
    [HttpGet]
    public IActionResult AcceptInvitation()
    {
        return View();
    }
}