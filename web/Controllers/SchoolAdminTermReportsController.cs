
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Controllers;

//
// Class Name       : SchoolAdminTermReportsController
// Purpose           : Generates PDF learner term reports and delivers them to guardians.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
[Authorize(Roles = "School Admin")]
public class SchoolAdminTermReportsController : Controller
{
    private readonly Supabase.Client _db;
    private readonly SupabaseSessionService _session;
    private readonly IConfiguration _config;

    //
    // Method Name       : SchoolAdminTermReportsController
    // Purpose           : Initializes required dependencies for SchoolAdminTermReportsController.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : Supabase.Client db; SupabaseSessionService session; IConfiguration config
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public SchoolAdminTermReportsController(
        Supabase.Client db,
        SupabaseSessionService session,
        IConfiguration config)
    {
        _db = db;
        _session = session;
        _config = config;
    }

    //
    // Method Name       : GetSchoolAsync
    // Purpose           : Get the school identifier from the authenticated session.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<Guid>
    //
    private async Task<Guid> GetSchoolAsync()
    {
        var id = User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(id, out var schoolId) ||
            schoolId == Guid.Empty ||
            !await _session.RestoreAsync())
        {
            throw new UnauthorizedAccessException();
        }

        return schoolId;
    }

    //
    // Method Name       : ValidTerm
    // Purpose           : Check whether the selected reporting term is supported.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : string term
    // Output Type       : bool
    //
    private static bool ValidTerm(string term) =>
        term is "Term 1" or "Term 2"
            or "Term 3" or "Term 4";

    // ======================================
    // REPORT SELECTION PAGE
    // ======================================

    //
    // Method Name       : Index
    // Purpose           : Display report filters, learner choices and the report preview.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : int? academicYear; string term; Guid? classId; Guid? learnerId
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Index(
        int? academicYear,
        string term = "Term 1",
        Guid? classId = null,
        Guid? learnerId = null)
    {
        try
        {
            var schoolId = await GetSchoolAsync();

            var year = academicYear ?? DateTime.Today.Year;

            if (year < 2000 || year > 2100 ||
                !ValidTerm(term))
                return BadRequest("Invalid reporting period.");

            var model = new SchoolAdminTermReportViewModel
            {
                AcademicYear = year,
                Term = term,
                ClassId = classId,
                LearnerId = learnerId
            };

            var classes = (await _db
                .From<SchoolClassRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get()).Models
                .Where(x => x.AcademicYear == year)
                .ToList();

            model.Classes = classes
                .Select(x => new TermReportClassOption
                {
                    Id = x.Id,
                    Name = x.Name,
                    Grade = x.Grade
                })
                .OrderBy(x => x.Name)
                .ToList();

            var classIds = classes
                .Select(x => x.Id)
                .ToHashSet();

            var enrollments = (await _db
                .From<EnrollmentRecord>()
                .Get()).Models
                .Where(x =>
                    x.AcademicYear == year &&
                    classIds.Contains(x.ClassId))
                .ToList();

            var learners = (await _db
                .From<LearnerRecord>()
                .Where(x => x.SchoolId == schoolId)
                .Get()).Models;

            model.Learners = learners
                .Where(l => enrollments.Any(e =>
                    e.LearnerId == l.Id &&
                    (!classId.HasValue ||
                     e.ClassId == classId.Value)))
                .Select(l => new TermReportLearnerOption
                {
                    Id = l.Id,
                    FullName = l.FullName,
                    AdmissionNumber = l.AdmissionNumber,
                    ClassId = enrollments
                        .First(e =>
                            e.LearnerId == l.Id &&
                            (!classId.HasValue ||
                             e.ClassId == classId.Value))
                        .ClassId
                })
                .OrderBy(l => l.FullName)
                .ToList();

            if (learnerId.HasValue &&
                model.Learners.Any(x =>
                    x.Id == learnerId.Value))
            {
                model.Report = await BuildReportAsync(
                    schoolId, learnerId.Value, year, term);
            }

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    // ======================================
    // BUILD REPORT FROM PUBLISHED RESULTS
    // ======================================

    //
    // Method Name       : BuildReportAsync
    // Purpose           : Assemble a learner term report from published school results.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : Guid schoolId; Guid learnerId; int year; string term
    // Output Type       : Task<TermReportDocumentViewModel>
    //
    private async Task<TermReportDocumentViewModel>
        BuildReportAsync(
            Guid schoolId,
            Guid learnerId,
            int year,
            string term)
    {
        if (!ValidTerm(term) ||
            year < 2000 || year > 2100)
            throw new ArgumentException(
                "Invalid reporting period.");

        var learner = (await _db
            .From<LearnerRecord>()
            .Where(x => x.Id == learnerId)
            .Get()).Models.FirstOrDefault();

        if (learner == null ||
            learner.SchoolId != schoolId)
            throw new UnauthorizedAccessException();

        var school = (await _db
            .From<SuperAdminSchoolRecord>()
            .Where(x => x.Id == schoolId)
            .Get()).Models.FirstOrDefault();

        if (school == null)
            throw new InvalidOperationException(
                "School not found.");

        var classes = (await _db
            .From<SchoolClassRecord>()
            .Where(x => x.SchoolId == schoolId)
            .Get()).Models
            .Where(x => x.AcademicYear == year)
            .ToList();

        var classIds = classes
            .Select(x => x.Id)
            .ToHashSet();

        var enrollment = (await _db
            .From<EnrollmentRecord>()
            .Where(x => x.LearnerId == learnerId)
            .Get()).Models
            .Where(x =>
                x.AcademicYear == year &&
                classIds.Contains(x.ClassId))
            .OrderByDescending(x => x.EnrollmentDate)
            .FirstOrDefault();

        if (enrollment == null)
            throw new InvalidOperationException(
                "Learner has no enrolment for this year.");

        var schoolClass = classes.First(
            x => x.Id == enrollment.ClassId);

        var subjects = (await _db
            .From<SubjectRecord>()
            .Where(x => x.SchoolId == schoolId)
            .Get()).Models
            .ToDictionary(x => x.Id, x => x.Name);

        var grades = (await _db
            .From<TermReportPublishedGradeRecord>()
            .Where(x => x.LearnerId == learnerId)
            .Where(x => x.AcademicYear == year)
            .Where(x => x.AcademicTerm == term)
            .Where(x => x.PublicationStatus == "Published")
            .Get()).Models
            .Where(x =>
                classIds.Contains(x.ClassId) &&
                subjects.ContainsKey(x.SubjectId))
            .ToList();

        var report = new TermReportDocumentViewModel
        {
            SchoolName = school.Name,
            SchoolEmail = school.Email,
            LearnerId = learner.Id,
            LearnerName = learner.FullName,
            AdmissionNumber = learner.AdmissionNumber,
            Grade = schoolClass.Grade,
            ClassName = schoolClass.Name,
            AcademicYear = year,
            Term = term,
            GeneratedAt = DateTime.UtcNow
        };

        report.Subjects = grades
            .GroupBy(x => x.SubjectId)
            .Select(g => new TermReportSubjectResult
            {
                SubjectId = g.Key,
                SubjectName = subjects[g.Key],
                AssessmentCount = g.Count(),
                AveragePercentage = Math.Round(
                    g.Average(x => x.Score), 1),
                TeacherRemark = g
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => x.Remarks)
                    .FirstOrDefault(x =>
                        !string.IsNullOrWhiteSpace(x))
            })
            .OrderBy(x => x.SubjectName)
            .ToList();

        if (report.Subjects.Count > 0)
        {
            report.OverallAverage = Math.Round(
                report.Subjects.Average(
                    x => x.AveragePercentage), 1);
        }

        // Only guardians linked to this learner
        // may receive the confidential report.

        var links = (await _db
            .From<ParentLearnerRecord>()
            .Where(x => x.LearnerId == learnerId)
            .Get()).Models
            .Where(x => x.IsActive)
            .ToList();

        var parents = (await _db
            .From<ParentRecord>()
            .Get()).Models
            .Where(x => x.IsActive)
            .ToList();

        var users = (await _db
            .From<UserProfile>()
            .Where(x => x.SchoolId == schoolId)
            .Get()).Models
            .Where(x =>
                x.IsActive && x.Role == "Parent")
            .ToList();

        foreach (var link in links)
        {
            var parent = parents.FirstOrDefault(
                x => x.Id == link.ParentId);

            if (parent == null)
                continue;

            var user = users.FirstOrDefault(
                x => x.Id == parent.UserId);

            if (user == null ||
                string.IsNullOrWhiteSpace(user.Email))
                continue;

            report.Guardians.Add(
                new TermReportGuardianRecipient
                {
                    ParentId = parent.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsPrimaryGuardian =
                        link.IsPrimaryGuardian
                });
        }

        return report;
    }

    // ======================================
    // GENERATE PDF
    // ======================================

    //
    // Method Name       : CreatePdf
    // Purpose           : Create PDF document bytes from the learner term report.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : TermReportDocumentViewModel report
    // Output Type       : byte[]
    //
    private static byte[] CreatePdf(
        TermReportDocumentViewModel report)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);

                page.DefaultTextStyle(
                    x => x.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item().Text(report.SchoolName)
                        .Bold().FontSize(20)
                        .FontColor("#075640");

                    header.Item().Text(
                        "OFFICIAL TERM PROGRESS REPORT")
                        .Bold().FontSize(12);

                    header.Item().Text(
                        $"{report.Term} | {report.AcademicYear}")
                        .FontColor("#666666");

                    header.Item().PaddingVertical(8)
                        .LineHorizontal(2)
                        .LineColor("#E8B43A");
                });

                page.Content().PaddingTop(15)
                    .Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text(
                            $"Learner: {report.LearnerName}");

                        column.Item().Text(
                            $"Admission No: {report.AdmissionNumber}");

                        column.Item().Text(
                            $"Class: {report.ClassName}");

                        column.Item().PaddingTop(12)
                            .Text("SUBJECT RESULTS")
                            .Bold().FontSize(13);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(4);
                                cols.RelativeColumn();
                                cols.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Subject").Bold();
                                header.Cell().Text("Assessments").Bold();
                                header.Cell().Text("Average").Bold();
                            });

                            foreach (var subject in report.Subjects)
                            {
                                table.Cell().PaddingVertical(6)
                                    .Text(subject.SubjectName);

                                table.Cell().PaddingVertical(6)
                                    .Text(subject.AssessmentCount.ToString());

                                table.Cell().PaddingVertical(6)
                                    .Text(
                                        $"{subject.AveragePercentage:0.0}%");
                            }
                        });

                        column.Item().PaddingTop(10)
                            .Text(
                                $"Overall Average: " +
                                $"{report.OverallAverage:0.0}%")
                            .Bold().FontSize(14);

                        column.Item().PaddingTop(18).Text(
                            "This report reflects published assessment " +
                            "results recorded by the school.");

                        column.Item().Text(
                            "Generated: " +
                            report.GeneratedAt.ToString(
                                "dd MMM yyyy HH:mm") + " UTC");
                    });

                page.Footer().AlignCenter().Text(
                    "School Parental System | " +
                    "Confidential Learner Report");
            });
        }).GeneratePdf();
    }

    // ======================================
    // DOWNLOAD PDF
    // ======================================

    //
    // Method Name       : Pdf
    // Purpose           : Generate and download the requested learner term report PDF.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : Guid learnerId; int academicYear; string term
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Pdf(
        Guid learnerId,
        int academicYear,
        string term)
    {
        try
        {
            var schoolId = await GetSchoolAsync();

            var report = await BuildReportAsync(
                schoolId, learnerId, academicYear, term);

            if (!report.HasResults)
                return BadRequest(
                    "No published results for this term.");

            var pdf = CreatePdf(report);

            return File(
                pdf,
                "application/pdf",
                $"TermReport-{report.AdmissionNumber}-" +
                $"{term.Replace(" ", "")}-{academicYear}.pdf");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // ======================================
    // SEND PDF TO LINKED GUARDIAN
    // ======================================

    //
    // Method Name       : Send
    // Purpose           : Email a published term report to a registered learner guardian.
    // Re-use            : Invoked by SchoolAdminTermReportsController or MVC requests.
    // Input Parameters  : SendTermReportInput input
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(
        SendTermReportInput input)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new ArgumentException(
                    "Invalid report details.");

            var schoolId = await GetSchoolAsync();

            var report = await BuildReportAsync(
                schoolId,
                input.LearnerId,
                input.AcademicYear,
                input.Term);

            if (!report.HasResults)
                throw new InvalidOperationException(
                    "No published results available.");

            // Recipient MUST be a registered
            // guardian linked to this learner.

            var guardian = report.Guardians
                .FirstOrDefault(x =>
                    x.ParentId == input.ParentId);

            if (guardian == null)
                throw new UnauthorizedAccessException();

            var server = _config["SmtpSettings:Server"];
            var sender = _config["SmtpSettings:SenderEmail"];
            var password = _config["SmtpSettings:Password"];

            if (string.IsNullOrWhiteSpace(server) ||
                string.IsNullOrWhiteSpace(sender) ||
                string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "SMTP is not configured.");

            var port = int.TryParse(
                _config["SmtpSettings:Port"],
                out var configuredPort)
                ? configuredPort : 587;

            var bytes = CreatePdf(report);

            using var message = new MailMessage();

            message.From = new MailAddress(
                sender,
                _config["SmtpSettings:SenderName"]
                    ?? "School Parental System");

            message.To.Add(guardian.Email);

            message.Subject =
                $"{report.SchoolName} - " +
                $"{report.Term} {report.AcademicYear} Results";

            message.Body =
                $"Dear {guardian.FullName},\n\n" +
                $"Please find attached the published " +
                $"term report for {report.LearnerName}.\n\n" +
                $"{report.SchoolName}";

            using var stream = new MemoryStream(bytes);

            message.Attachments.Add(new Attachment(
                stream,
                "TermReport.pdf",
                "application/pdf"));

            using var smtp = new SmtpClient(server, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    sender, password)
            };

            //
            // STEP 1: EMAIL THE PDF TO THE VERIFIED GUARDIAN
            // Keep Gmail delivery exactly as before.
            //
            await smtp.SendMailAsync(message);

            //
            // STEP 2: PUBLISH THE SAME PDF FOR THE PARENT APP
            // School and learner access are checked by Supabase RLS.
            // Publishing failure must not undo a successful email.
            //
            try
            {
                var publisher = new TermReportSupabasePublisher(
                    _db,
                    _config);

                await publisher.PublishAsync(
                    schoolId,
                    input.LearnerId,
                    input.AcademicYear,
                    input.Term,
                    bytes,
                    User);

                TempData["SuccessMessage"] =
                    "Report emailed successfully and published " +
                    "to the Parent mobile app.";
            }
            catch (Exception publishError)
            {
                // Preserve the successful email result while recording
                // the separate Supabase Storage/database failure.
                System.Diagnostics.Debug.WriteLine(
                    "Term report Supabase publication failed: " +
                    publishError);

                TempData["SuccessMessage"] =
                    "Report emailed successfully to the guardian.";

                TempData["ErrorMessage"] =
                    "The PDF could not be published to the Parent app. " +
                    "Check Visual Studio Output for the Supabase error.";
            }
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Report could not be sent. Check the " +
                "published grades and email configuration.";
        }

        return RedirectToAction(nameof(Index), new
        {
            academicYear = input.AcademicYear,
            term = input.Term,
            learnerId = input.LearnerId
        });
    }
}


// ==========================================
// DATABASE MODEL: PUBLISHED GRADES
// ==========================================

//
// Class Name       : TermReportPublishedGradeRecord
// Purpose           : Maps published grade fields used when compiling a term report.
// Re-use            : Used when reading grade records from Supabase.
// Input Parameters  : Values mapped from database columns.
// Output Type       : Database record class
//
[Table("grades")]
public class TermReportPublishedGradeRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("learner_id")]
    public Guid LearnerId { get; set; }

    [Column("subject_id")]
    public Guid SubjectId { get; set; }

    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("academic_year")]
    public int AcademicYear { get; set; }

    [Column("academic_term")]
    public string AcademicTerm { get; set; } = "";

    [Column("publication_status")]
    public string PublicationStatus { get; set; } = "";

    [Column("score")]
    public decimal Score { get; set; }

    [Column("remarks")]
    public string? Remarks { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
