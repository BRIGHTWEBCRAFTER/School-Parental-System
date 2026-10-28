using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : TermReportSupabasePublisher
// Purpose          : Upload confidential term report PDFs and record their
//                    locations for authorized Parent mobile-app access.
// Re-use           : SchoolAdminTermReportsController.Send.
// Input Parameters : Supabase Client and IConfiguration.
// Output Type      : Service class.
//
public sealed class TermReportSupabasePublisher
{
    private readonly Supabase.Client _db;
    private readonly IConfiguration _config;

    public TermReportSupabasePublisher(
        Supabase.Client db,
        IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    //
    // Method Name      : PublishAsync
    // Purpose          : Upload a PDF into private Supabase Storage and upsert
    //                    the matching public.reports metadata row.
    // Re-use           : Send PDF to Guardian action after successful email.
    // Input Parameters : School ID, learner ID, academic year, term, PDF data,
    //                    and the authorized School Admin's claims.
    // Output Type      : Task.
    //
    public async Task PublishAsync(
        Guid schoolId,
        Guid learnerId,
        int academicYear,
        string term,
        byte[] pdfBytes,
        ClaimsPrincipal currentUser)
    {
        if (schoolId == Guid.Empty || learnerId == Guid.Empty ||
            academicYear < 2000 || academicYear > 2100 ||
            term is not ("Term 1" or "Term 2" or "Term 3" or "Term 4") ||
            pdfBytes == null || pdfBytes.Length == 0)
            throw new ArgumentException("Invalid report publication details.");

        var userIdText = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdText, out var adminId))
            throw new UnauthorizedAccessException("Missing School Admin identity.");

        var token = _db.Auth.CurrentSession?.AccessToken;
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("Supabase session expired.");

        var url = (_config["Supabase:Url"] ?? "").TrimEnd('/');
        var key = _config["Supabase:Key"] ?? "";
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing Supabase configuration.");

        var termFile = term.Replace(" ", "");
        var path = $"{schoolId:D}/{learnerId:D}/{academicYear}/{termFile}.pdf";
        var filename = $"TermReport-{termFile}-{academicYear}.pdf";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("apikey", key);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Upload the PDF. The Storage RLS policy enforces school and learner ownership.
        using var upload = new HttpRequestMessage(
            HttpMethod.Post,
            $"{url}/storage/v1/object/reports/{path}");
        upload.Headers.TryAddWithoutValidation("x-upsert", "true");
        upload.Content = new ByteArrayContent(pdfBytes);
        upload.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/pdf");

        using var uploaded = await client.SendAsync(upload);
        if (!uploaded.IsSuccessStatusCode)
        {
            var details = await uploaded.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Supabase Storage upload failed (HTTP {(int)uploaded.StatusCode}): {details}");
        }

        // Create or update the one report record allowed for this learner and term.
        var record = new
        {
            learner_id = learnerId,
            school_id = schoolId,
            academic_year = academicYear,
            academic_term = term,
            file_path = path,
            file_name = filename,
            file_size = pdfBytes.Length,
            uploaded_by = adminId
        };

        using var save = new HttpRequestMessage(
            HttpMethod.Post,
            $"{url}/rest/v1/reports?on_conflict=learner_id,academic_year,academic_term");
        save.Headers.TryAddWithoutValidation(
            "Prefer", "resolution=merge-duplicates,return=minimal");
        save.Content = new StringContent(
            JsonSerializer.Serialize(record), Encoding.UTF8, "application/json");

        using var saved = await client.SendAsync(save);
        if (!saved.IsSuccessStatusCode)
        {
            var details = await saved.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Supabase report record failed (HTTP {(int)saved.StatusCode}): {details}");
        }
    } // end method
} // end class
