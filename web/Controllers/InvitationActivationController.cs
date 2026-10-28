
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Controllers;

//
// Class Name       : InvitationActivationController
// Purpose          : Verify a temporary PIN received
//                    by email and create a password.
// Re-use           : New Teachers and Parents.
// Input Parameters : IConfiguration and
//                    IHttpClientFactory.
// Output Type      : MVC controller.
//
[AllowAnonymous]
public class InvitationActivationController : Controller
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClients;

    public InvitationActivationController(
        IConfiguration config,
        IHttpClientFactory httpClients)
    {
        _config = config;
        _httpClients = httpClients;
    }

    private string SupabaseUrl =>
        (_config["Supabase:Url"] ?? "").TrimEnd('/');

    private string SupabaseKey =>
        _config["Supabase:Key"] ?? "";


    //
    // Method Name      : Index
    // Purpose          : Display the activation form.
    // Re-use           : Account activation link.
    // Input Parameters : Optional email.
    // Output Type      : IActionResult.
    //

    [HttpGet]
    public IActionResult Index(string? email = null)
    {
        return View(new InvitationActivationViewModel
        {
            Email = email ?? ""
        });
    } // end method


    //
    // Method Name      : Index POST
    // Purpose          : Verify the emailed PIN,
    //                    then save the new password.
    // Re-use           : Teacher and Parent activation.
    // Input Parameters : Activation ViewModel.
    // Output Type      : Task<IActionResult>.
    //

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        InvitationActivationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(SupabaseUrl) ||
            string.IsNullOrWhiteSpace(SupabaseKey))
        {
            ModelState.AddModelError(
                "",
                "Supabase configuration is missing.");

            return View(model);
        }

        try
        {
            var email = model.Email.Trim().ToLowerInvariant();

            var client = _httpClients.CreateClient();

            //
            // STEP 1: VERIFY TEMPORARY PIN
            //

            using var verifyRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{SupabaseUrl}/auth/v1/verify");

            verifyRequest.Headers.Add(
                "apikey",
                SupabaseKey);

            verifyRequest.Content = JsonContent.Create(new
            {
                email = email,
                token = model.Pin.Trim(),
                type = "email"
            });

            using var verifyResponse =
                await client.SendAsync(verifyRequest);

            if (!verifyResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    nameof(model.Pin),
                    "Incorrect or expired PIN.");

                return View(model);
            }

            using var verifyJson = JsonDocument.Parse(
                await verifyResponse.Content
                    .ReadAsStringAsync());

            var session = verifyJson.RootElement;

            if (!session.TryGetProperty(
                    "access_token", out var tokenElement) ||
                !session.TryGetProperty(
                    "user", out var userElement))
            {
                ModelState.AddModelError(
                    "",
                    "PIN verification failed.");

                return View(model);
            }

            var accessToken = tokenElement.GetString();

            var userId = userElement
                .GetProperty("id")
                .GetString();

            var verifiedEmail = userElement
                .GetProperty("email")
                .GetString();

            if (string.IsNullOrWhiteSpace(accessToken) ||
                string.IsNullOrWhiteSpace(userId) ||
                !string.Equals(
                    verifiedEmail,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    "",
                    "Account verification failed.");

                return View(model);
            }

            //
            // STEP 2: VERIFY APPLICATION ROLE
            //

            using var profileRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"{SupabaseUrl}/rest/v1/users" +
                $"?id=eq.{Uri.EscapeDataString(userId)}" +
                "&select=id,email,role,is_active");

            AddAuthHeaders(
                profileRequest,
                accessToken);

            using var profileResponse =
                await client.SendAsync(profileRequest);

            if (!profileResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "School profile could not be loaded.");

                return View(model);
            }

            using var profileJson = JsonDocument.Parse(
                await profileResponse.Content
                    .ReadAsStringAsync());

            var profiles = profileJson.RootElement;

            if (profiles.ValueKind != JsonValueKind.Array ||
                profiles.GetArrayLength() != 1)
            {
                ModelState.AddModelError(
                    "",
                    "Your school account was not found.");

                return View(model);
            }

            var profile = profiles[0];

            var role = profile.GetProperty("role")
                .GetString();

            var active = profile.GetProperty("is_active")
                .GetBoolean();

            var profileEmail = profile
                .GetProperty("email")
                .GetString();

            if (!active ||
                (role != "Teacher") ||
                !string.Equals(
                    profileEmail,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    "",
                    "This account cannot be activated.");

                return View(model);
            }


            //
            // Method Purpose    : Complete Teacher activation
            //                     after setting a new password.
            // Re-use            : First-time Teacher activation.
            // Input Parameters  : Authenticated activation token.
            // Output Type       : Supabase HTTP response.
            //

            using var completionRequest = new HttpRequestMessage(
                HttpMethod.Patch,
                $"{SupabaseUrl}/rest/v1/users?id=eq.{userId}");

            AddAuthHeaders(
                completionRequest,
                accessToken);

            completionRequest.Content = JsonContent.Create(new
            {
                requires_password_change = false
            });

            using var completionResponse =
                await client.SendAsync(completionRequest);

            if (!completionResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Password changed, but account activation " +
                    "could not be finalized. Contact your school.");

                return View(model);
            }


            //
            // STEP 3: SET PERMANENT PASSWORD
            //

            using var updateRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"{SupabaseUrl}/auth/v1/user");

            AddAuthHeaders(
                updateRequest,
                accessToken);

            updateRequest.Content = JsonContent.Create(new
            {
                password = model.NewPassword
            });

            using var updateResponse =
                await client.SendAsync(updateRequest);

            if (!updateResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Password update failed. " +
                    "Check the password requirements.");

                return View(model);
            }

            //
            // STEP 4: REDIRECT TO NORMAL LOGIN
            //

            TempData["SuccessMessage"] =
                "Account activated successfully. " +
                "Log in using your new password.";

            return RedirectToAction("Login", "Auth");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                "",
                "Unable to connect to Supabase.");
        }
        catch (JsonException)
        {
            ModelState.AddModelError(
                "",
                "Invalid response from Supabase.");
        }

        return View(model);
    } // end method



    //
    // Method Name      : Resend
    // Purpose          : Request a temporary PIN from
    //                    Supabase Authentication.
    // Re-use           : Teacher and Parent activation.
    // Input Parameters : string email.
    // Output Type      : Task<IActionResult>.
    //

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resend(string email)
    {
        var safeEmail = (email ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(safeEmail) ||
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                .IsValid(safeEmail))
        {
            TempData["ErrorMessage"] =
                "Please enter a valid email address.";

            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(SupabaseUrl) ||
            string.IsNullOrWhiteSpace(SupabaseKey))
        {
            TempData["ErrorMessage"] =
                "Supabase configuration is missing. " +
                "Check the URL and API key in appsettings.";

            return RedirectToAction(
                nameof(Index),
                new { email = safeEmail });
        }

        try
        {
            using var client = _httpClients.CreateClient();

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{SupabaseUrl}/auth/v1/otp");

            request.Headers.Add("apikey", SupabaseKey);

            request.Content = JsonContent.Create(new
            {
                email = safeEmail,
                create_user = false
            });

            using var response =
                await client.SendAsync(request);

            //
            // Supabase has accepted the email request
            // only when it returns a success response.
            //

            if (response.IsSuccessStatusCode)
            {
                TempData["InfoMessage"] =
                    "PIN request accepted. Check your email " +
                    "inbox and Spam folder. Delivery is not guaranteed.";

                return RedirectToAction(
                    nameof(Index),
                    new { email = safeEmail });
            }

            //
            // Report the HTTP status while keeping
            // sensitive provider details out of the UI.
            //

            TempData["ErrorMessage"] =
                "Temporary PIN request failed. " +
                $"Supabase returned HTTP {(int)response.StatusCode}. " +
                "Check your Supabase Authentication logs " +
                "and SMTP settings.";
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Cannot connect to Supabase. " +
                "Check your internet connection and URL.";
        }
        catch (TaskCanceledException)
        {
            TempData["ErrorMessage"] =
                "The request timed out. Please try again later.";
        }

        return RedirectToAction(
            nameof(Index),
            new { email = safeEmail });
    } // end method



    //
    // Method Name      : AddAuthHeaders
    // Purpose          : Authenticate requests to
    //                    Supabase using the user's token.
    // Input Parameters : Request and access token.
    // Output Type      : void.
    //

    private void AddAuthHeaders(
        HttpRequestMessage request,
        string accessToken)
    {
        request.Headers.Add(
            "apikey",
            SupabaseKey);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);
    } // end method
} // end class
