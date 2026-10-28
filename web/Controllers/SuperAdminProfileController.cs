using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;


namespace SchoolParentalSystem.Web.Controllers;


[Authorize(Roles = "Super Admin")]
[Route("SuperAdmin")]
public class SuperAdminProfileController : Controller
{
    private readonly SuperAdminProfileService
        _profileService;

    private readonly ILogger<SuperAdminProfileController>
        _logger;


    public SuperAdminProfileController(
        SuperAdminProfileService profileService,
        ILogger<SuperAdminProfileController> logger)
    {
        _profileService =
            profileService;

        _logger =
            logger;
    }


    // =====================================================
    // PROFILE
    // GET: /SuperAdmin/Profile
    // =====================================================

    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        ViewData["Title"] =
            "Profile";


        var userId =
            GetCurrentUserId();


        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var model =
                await _profileService
                    .GetProfileAsync(
                        userId.Value);


            return View(
                "~/Views/SuperAdmin/Profile.cshtml",
                model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Super Admin profile could not be loaded.");


            TempData["ErrorMessage"] =
                ex.Message;


            return View(
                "~/Views/SuperAdmin/Profile.cshtml",
                new SuperAdminProfileViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Super Admin profile.");


            TempData["ErrorMessage"] =
                "Profile could not be loaded.";


            return View(
                "~/Views/SuperAdmin/Profile.cshtml",
                new SuperAdminProfileViewModel());
        }
    }


    // =====================================================
    // UPDATE PROFILE
    // POST: /SuperAdmin/Profile/Update
    // =====================================================

    [HttpPost("Profile/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(
        SuperAdminProfileViewModel model)
    {
        var userId =
            GetCurrentUserId();


        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        if (string.IsNullOrWhiteSpace(
            model.UpdatedFullName))
        {
            TempData["ErrorMessage"] =
                "Full name is required.";


            return RedirectToAction(
                nameof(Profile));
        }


        try
        {
            var updatedFullName =
                model.UpdatedFullName
                    .Trim();


            // =================================================
            // UPDATE DATABASE
            // =================================================

            await _profileService
                .UpdateProfileAsync(
                    userId.Value,
                    updatedFullName,
                    model.UpdatedPhone);


            // =================================================
            // REFRESH AUTHENTICATION NAME CLAIM
            // =================================================

            await RefreshUserNameClaimAsync(
                updatedFullName);


            TempData["SuccessMessage"] =
                "Your profile was updated successfully.";


            return RedirectToAction(
                nameof(Profile));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Profile));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to update Super Admin profile.");


            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while updating Super Admin profile.");


            TempData["ErrorMessage"] =
                "Your profile could not be updated.";


            return RedirectToAction(
                nameof(Profile));
        }
    }


    // =====================================================
    // SEND PASSWORD RESET
    // POST: /SuperAdmin/Profile/PasswordReset
    // =====================================================

    [HttpPost("Profile/PasswordReset")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendPasswordReset()
    {
        var userId =
            GetCurrentUserId();


        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }


        try
        {
            var redirectUrl =
                Url.Action(
                    "ResetPassword",
                    "Auth",
                    values: null,
                    protocol: Request.Scheme);


            if (string.IsNullOrWhiteSpace(
                redirectUrl))
            {
                throw new InvalidOperationException(
                    "Password reset URL could not be created.");
            }


            await _profileService
                .SendPasswordResetAsync(
                    userId.Value,
                    redirectUrl);


            TempData["SuccessMessage"] =
                "A password reset link has been sent to your registered email address.";


            return RedirectToAction(
                nameof(Profile));
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Profile));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Unable to send Super Admin password reset email.");


            TempData["ErrorMessage"] =
                ex.Message;


            return RedirectToAction(
                nameof(Profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Password reset failed for Super Admin. Error: {Message}",
                ex.Message);


            TempData["ErrorMessage"] =
                $"Password reset failed: {ex.Message}";


            return RedirectToAction(
                nameof(Profile));
        }
    }



    // =====================================================
    // UPLOAD PROFILE PICTURE
    // Purpose          : Upload the signed-in Super Admin's avatar.
    // Re-use           : Profile page photo form.
    // Input Parameters : IFormFile picture.
    // Output Type      : Task<IActionResult>
    // =====================================================

    [HttpPost("Profile/UploadPicture")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfilePicture(IFormFile picture)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Auth");

        try
        {
            await _profileService.UploadProfilePictureAsync(userId.Value, picture);
            TempData["SuccessMessage"] = "Profile picture updated successfully.";
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Super Admin profile picture upload failed.");
            TempData["ErrorMessage"] = "Profile picture upload failed. Please try again.";
        }

        return RedirectToAction(nameof(Profile));
    }


    // =====================================================
    // REMOVE PROFILE PICTURE
    // Purpose          : Remove the signed-in Super Admin's avatar.
    // Re-use           : Profile page Remove Picture button.
    // Input Parameters : None.
    // Output Type      : Task<IActionResult>
    // =====================================================

    [HttpPost("Profile/RemovePicture")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return RedirectToAction("Login", "Auth");

        try
        {
            await _profileService.RemoveProfilePictureAsync(userId.Value);
            TempData["SuccessMessage"] = "Profile picture removed.";
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Super Admin profile picture removal failed.");
            TempData["ErrorMessage"] = "Profile picture could not be removed.";
        }

        return RedirectToAction(nameof(Profile));
    }


    // =====================================================
    // REFRESH CURRENT USER NAME CLAIM
    // =====================================================

    private async Task RefreshUserNameClaimAsync(
        string updatedFullName)
    {
        var authenticateResult =
            await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);


        if (!authenticateResult.Succeeded ||
            authenticateResult.Principal == null)
        {
            return;
        }


        var existingIdentity =
            authenticateResult.Principal.Identity
                as ClaimsIdentity;


        if (existingIdentity == null)
        {
            return;
        }


        var claims =
            existingIdentity.Claims
                .Where(
                    claim =>
                        claim.Type !=
                        ClaimTypes.Name)
                .ToList();


        claims.Add(
            new Claim(
                ClaimTypes.Name,
                updatedFullName));


        var refreshedIdentity =
            new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);


        var refreshedPrincipal =
            new ClaimsPrincipal(
                refreshedIdentity);


        var authenticationProperties =
            authenticateResult.Properties
            ?? new AuthenticationProperties();


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme,
            refreshedPrincipal,
            authenticationProperties);
    }


    // =====================================================
    // CURRENT USER ID
    // =====================================================

    private Guid? GetCurrentUserId()
    {
        var userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (string.IsNullOrWhiteSpace(
            userIdValue))
        {
            return null;
        }


        if (!Guid.TryParse(
            userIdValue,
            out var userId))
        {
            return null;
        }


        return userId;
    }
}