using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;


namespace SchoolParentalSystem.Web.Controllers;


//
// Class Name       : SuperAdminAccessCodeController
// Purpose           : Manages registration and access codes for school administrators.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
[Authorize(Roles = "Super Admin")]
[Route("SuperAdmin")]
public class SuperAdminAccessCodeController : Controller
{
    private readonly SuperAdminAccessCodeService _accessCodeService;

    private readonly RegistrationCodeEmailService _emailService;

    private readonly ILogger<SuperAdminAccessCodeController> _logger;


    //
    // Method Name       : SuperAdminAccessCodeController
    // Purpose           : Initializes required dependencies for SuperAdminAccessCodeController.
    // Re-use            : Invoked by SuperAdminAccessCodeController or MVC requests.
    // Input Parameters  : SuperAdminAccessCodeService accessCodeService;
    //                      RegistrationCodeEmailService emailService;
    //                      ILogger<SuperAdminAccessCodeController> logger
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public SuperAdminAccessCodeController(
        SuperAdminAccessCodeService accessCodeService,
        RegistrationCodeEmailService emailService,
        ILogger<SuperAdminAccessCodeController> logger)
    {
        _accessCodeService = accessCodeService;
        _emailService = emailService;
        _logger = logger;
    }


    // =====================================================
    // ACCESS CODES PAGE
    // GET: /SuperAdmin/AccessCodes
    // =====================================================

    //
    // Method Name       : AccessCodes
    // Purpose           : List school administrator registration access codes.
    // Re-use            : Invoked by SuperAdminAccessCodeController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet("AccessCodes")]
    public async Task<IActionResult> AccessCodes()
    {
        ViewData["Title"] = "Access Codes";


        try
        {
            var model =
                await _accessCodeService
                    .GetAccessCodesPageAsync();


            return View(
                "~/Views/SuperAdmin/AccessCodes.cshtml",
                model);
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
                "Failed to load the Super Admin Access Codes page.");


            ModelState.AddModelError(
                string.Empty,
                "Access Codes could not be loaded.");


            return View(
                "~/Views/SuperAdmin/AccessCodes.cshtml",
                new SuperAdminAccessCodeViewModel());
        }
    }


    // =====================================================
    // GENERATE SCHOOL ADMIN REGISTRATION CODE
    // POST: /SuperAdmin/AccessCodes/Generate
    // =====================================================

    //
    // Method Name       : GenerateRegistrationCode
    // Purpose           : Generate or send a school administrator registration code.
    // Re-use            : Invoked by SuperAdminAccessCodeController or MVC requests.
    // Input Parameters  : SuperAdminAccessCodeViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost("AccessCodes/Generate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRegistrationCode(
        SuperAdminAccessCodeViewModel model)
    {
        ViewData["Title"] = "Access Codes";


        try
        {
            // -------------------------------------------------
            // VALIDATE SCHOOL
            // -------------------------------------------------

            if (!model.SelectedSchoolId.HasValue ||
                model.SelectedSchoolId.Value == Guid.Empty)
            {
                ModelState.AddModelError(
                    nameof(
                        SuperAdminAccessCodeViewModel
                            .SelectedSchoolId),
                    "Please select a school.");


                var validationPageModel =
                    await _accessCodeService
                        .GetAccessCodesPageAsync();


                validationPageModel.SelectedSchoolId =
                    model.SelectedSchoolId;


                return View(
                    "~/Views/SuperAdmin/AccessCodes.cshtml",
                    validationPageModel);
            }


            // -------------------------------------------------
            // GET LOGGED-IN SUPER ADMIN USER ID
            // -------------------------------------------------

            var userIdText =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);


            if (!Guid.TryParse(
                    userIdText,
                    out var createdByUserId))
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }


            // -------------------------------------------------
            // GENERATE REGISTRATION CODE
            // -------------------------------------------------

            var result =
                await _accessCodeService
                    .GenerateRegistrationCodeAsync(
                        model.SelectedSchoolId.Value,
                        createdByUserId);


            // -------------------------------------------------
            // SEND REGISTRATION CODE TO SCHOOL EMAIL
            // -------------------------------------------------

            var emailSent =
                false;


            if (string.IsNullOrWhiteSpace(
                    result.SchoolEmail))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The registration code was generated, but the selected school does not have a registered email address.");
            }
            else
            {
                try
                {
                    await _emailService
                        .SendSchoolAdminRegistrationCodeAsync(
                            result.SchoolEmail,
                            result.SchoolName,
                            result.Code,
                            result.ExpiresAt);


                    emailSent =
                        true;
                }
                catch (Exception ex)
                {
                    // The code already exists at this point.
                    // Do NOT generate another code just because
                    // email delivery failed.

                    _logger.LogError(
                        ex,
                        "The School Admin registration code was generated, but the email could not be sent.");


                    ModelState.AddModelError(
                        string.Empty,
                        "The registration code was generated successfully, but the email could not be sent.");
                }
            }


            // -------------------------------------------------
            // RELOAD ACTIVE SCHOOLS
            // -------------------------------------------------

            var successPageModel =
                await _accessCodeService
                    .GetAccessCodesPageAsync();


            successPageModel.SelectedSchoolId =
                model.SelectedSchoolId;


            successPageModel.GeneratedCode =
                result.Code;


            successPageModel.GeneratedSchoolName =
                result.SchoolName;


            successPageModel.GeneratedCodeExpiresAt =
                result.ExpiresAt;


            successPageModel.CodeGeneratedSuccessfully =
                true;


            // -------------------------------------------------
            // EMAIL SUCCESS MESSAGE
            // -------------------------------------------------

            if (emailSent)
            {
                ViewData["RegistrationEmailSuccess"] =
                    $"The registration code was also sent to {result.SchoolEmail}.";
            }


            return View(
                "~/Views/SuperAdmin/AccessCodes.cshtml",
                successPageModel);
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
                string.Empty,
                ex.Message);


            var invalidPageModel =
                await _accessCodeService
                    .GetAccessCodesPageAsync();


            invalidPageModel.SelectedSchoolId =
                model.SelectedSchoolId;


            return View(
                "~/Views/SuperAdmin/AccessCodes.cshtml",
                invalidPageModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to generate a School Admin registration code.");


            ModelState.AddModelError(
                string.Empty,
                "The registration code could not be generated.");


            var errorPageModel =
                await _accessCodeService
                    .GetAccessCodesPageAsync();


            errorPageModel.SelectedSchoolId =
                model.SelectedSchoolId;


            return View(
                "~/Views/SuperAdmin/AccessCodes.cshtml",
                errorPageModel);
        }
    }
}