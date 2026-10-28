using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Services.SuperAdmin;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;


namespace SchoolParentalSystem.Web.Controllers;


//
// Class Name       : SchoolAdminRegistrationController
// Purpose           : Handles new School Admin account registration.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
[AllowAnonymous]
[Route("Register")]
public class SchoolAdminRegistrationController : Controller
{
    private readonly SchoolAdminRegistrationService
        _registrationService;

    private readonly ILogger<SchoolAdminRegistrationController>
        _logger;


    //
    // Method Name       : SchoolAdminRegistrationController
    // Purpose           : Initializes required dependencies for SchoolAdminRegistrationController.
    // Re-use            : Invoked by SchoolAdminRegistrationController or MVC requests.
    // Input Parameters  : SchoolAdminRegistrationService registrationService;
    //                      ILogger<SchoolAdminRegistrationController> logger
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public SchoolAdminRegistrationController(
        SchoolAdminRegistrationService registrationService,
        ILogger<SchoolAdminRegistrationController> logger)
    {
        _registrationService =
            registrationService;

        _logger =
            logger;
    }


    // =====================================================
    // SCHOOL ADMIN REGISTRATION PAGE
    // GET: /Register/SchoolAdmin
    // =====================================================

    //
    // Method Name       : SchoolAdmin
    // Purpose           : Display or submit the school administrator registration form.
    // Re-use            : Invoked by SchoolAdminRegistrationController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet("SchoolAdmin")]
    public async Task<IActionResult> SchoolAdmin()
    {
        ViewData["Title"] =
            "School Admin Registration";


        try
        {
            var model =
                await _registrationService
                    .GetRegistrationPageAsync();


            return View(
                "~/Views/Auth/SchoolAdminRegistration.cshtml",
                model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load the School Admin registration page.");


            ModelState.AddModelError(
                string.Empty,
                "The registration page could not be loaded.");


            return View(
                "~/Views/Auth/SchoolAdminRegistration.cshtml",
                new SchoolAdminRegistrationViewModel());
        }
    }


    // =====================================================
    // SCHOOL ADMIN REGISTRATION SUBMIT
    // POST: /Register/SchoolAdmin
    // =====================================================

    //
    // Method Name       : SchoolAdmin
    // Purpose           : Display or submit the school administrator registration form.
    // Re-use            : Invoked by SchoolAdminRegistrationController or MVC requests.
    // Input Parameters  : SchoolAdminRegistrationViewModel model
    // Output Type       : Task<IActionResult>
    //
    [HttpPost("SchoolAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SchoolAdmin(
        SchoolAdminRegistrationViewModel model)
    {
        ViewData["Title"] =
            "School Admin Registration";


        // -------------------------------------------------
        // MODEL VALIDATION
        // -------------------------------------------------

        if (!ModelState.IsValid)
        {
            await ReloadSchoolsAsync(model);

            return View(
                "~/Views/Auth/SchoolAdminRegistration.cshtml",
                model);
        }


        try
        {
            // -------------------------------------------------
            // CREATE SCHOOL ADMIN ACCOUNT
            // -------------------------------------------------

            var result =
                await _registrationService
                    .RegisterSchoolAdminAsync(
                        model);


            // -------------------------------------------------
            // REGISTRATION FAILED
            // -------------------------------------------------

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);


                await ReloadSchoolsAsync(
                    model);


                return View(
                    "~/Views/Auth/SchoolAdminRegistration.cshtml",
                    model);
            }


            // -------------------------------------------------
            // SUCCESS
            // -------------------------------------------------

            TempData["LoginMessage"] =
                result.Message;


            return RedirectToAction(
                "Login",
                "Auth");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "School Admin registration failed.");


            ModelState.AddModelError(
                string.Empty,
                "Registration could not be completed right now. Please try again.");


            await ReloadSchoolsAsync(
                model);


            return View(
                "~/Views/Auth/SchoolAdminRegistration.cshtml",
                model);
        }
    }


    // =====================================================
    // RELOAD ACTIVE SCHOOLS
    // =====================================================

    //
    // Method Name       : ReloadSchoolsAsync
    // Purpose           : Reload school options for the registration form.
    // Re-use            : Invoked by SchoolAdminRegistrationController or MVC requests.
    // Input Parameters  : SchoolAdminRegistrationViewModel model
    // Output Type       : Task
    //
    private async Task ReloadSchoolsAsync(
        SchoolAdminRegistrationViewModel model)
    {
        var pageModel =
            await _registrationService
                .GetRegistrationPageAsync();


        model.Schools =
            pageModel.Schools;
    }
}