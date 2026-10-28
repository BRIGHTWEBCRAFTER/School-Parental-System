using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.Services.Teacher;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels.Teacher;

namespace SchoolParentalSystem.Web.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly TeacherDashboardService _dashboardService;
    private readonly TeacherAssignedClassesService _classesService;
    private readonly TeacherAttendanceService _attendanceService;
    private readonly TeacherGradesService _gradesService;
    private readonly TeacherCommunicationService _communicationService;
    private readonly TeacherEventsService _eventsService;
    private readonly ParentCommunicationEmailService _parentEmailService;


    public TeacherController(
        ILogger<TeacherController> logger,
        TeacherDashboardService dashboardService,
        TeacherAssignedClassesService classesService,
        TeacherAttendanceService attendanceService,
        TeacherGradesService gradesService,
        TeacherCommunicationService communicationService,
        TeacherEventsService eventsService,
        ParentCommunicationEmailService parentEmailService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
        _classesService = classesService;
        _attendanceService = attendanceService;
        _gradesService = gradesService;
        _communicationService = communicationService;
        _eventsService = eventsService;
        _parentEmailService = parentEmailService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            var model = await _dashboardService.GetDashboardAsync(userId, schoolId);

            ViewBag.GradeStats =
                await _gradesService.GetDashboardGradeStatsAsync(
                    userId,
                    schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load Teacher dashboard.");
            return View(new TeacherDashboardViewModel { DataLoadFailed = true });
        }
    }


    [HttpGet]
    public async Task<IActionResult> AssignedClasses(
        string? query)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            // 1. Get real classes from Supabase.
            var model = await _classesService
                .GetAssignedClassesAsync(
                    userId,
                    schoolId);

            var searchText = query?.Trim() ?? "";

            if (searchText.Length > 100)
            {
                searchText = searchText[..100];
            }

            model.SearchQuery = searchText;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // 2. Search class names,
                //    subjects and room numbers.
                model.ClassesITeach =
                    model.ClassesITeach
                        .Where(item =>
                            item.ClassName.Contains(
                                searchText,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            item.SubjectName.Contains(
                                searchText,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            item.RoomNumber.Contains(
                                searchText,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // 3. Search learners in the
                //    teacher's authorized classes.
                await _classesService
                    .PopulateLearnerMatchesAsync(
                        model,
                        schoolId,
                        searchText);
            }

            // 4. Show the combined results.
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher assigned classes.");

            return View(
                new TeacherAssignedClassesViewModel
                {
                    DataLoadFailed = true
                });
        }
    }


    /* ==========================================
       TEACHER ATTENDANCE MANAGEMENT
       ========================================== */

    [HttpGet]

    public async Task<IActionResult> Attendance(
    Guid? classId,
    DateTime? date,
    Guid? subjectId)

    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            var attendanceDate =
                (date ?? DateTime.Today).Date;


            var model = await _attendanceService
                .GetAttendanceAsync(
                    userId,
                    schoolId,
                    classId,
                    attendanceDate,
                    subjectId);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher attendance.");

            return View(
                new TeacherAttendanceViewModel
                {
                    DataLoadFailed = true
                });
        }
    }


    /* ==========================================
       SAVE / EDIT TEACHER ATTENDANCE
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAttendance(
        TeacherAttendanceSaveViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Invalid attendance information. " +
                    "Please check your entries.";

                return RedirectToAction(
                    nameof(Attendance),
                    new
                    {
                        classId = request.ClassId,
                        date = request.AttendanceDate
                            .ToString("yyyy-MM-dd")
                    });
            }


            await _attendanceService
                .SaveAttendanceAsync(
                    userId,
                    schoolId,
                    request);

            TempData["SuccessMessage"] =
                "Attendance saved successfully.";

            return RedirectToAction(
                nameof(Attendance),
                new
                {
                    classId = request.ClassId,

                    subjectId = request.SubjectId,

                    date = request.AttendanceDate
                        .ToString("yyyy-MM-dd")
                });
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to save Teacher attendance.");

            TempData["ErrorMessage"] =
                "Attendance could not be saved. " +
                "Please reload the register and try again.";
        }

        return RedirectToAction(
            nameof(Attendance),
            new
            {
                classId = request.ClassId,
                date = request.AttendanceDate
                    .ToString("yyyy-MM-dd")
            });
    }


    /* ==========================================
       TEACHER GRADE RECORDING
       GET: /Teacher/GradeRecording
       ========================================== */


    [HttpGet]
    public async Task<IActionResult> GradeRecording(
        Guid? classId,
        Guid? subjectId,
        string? term,
        string? search,
        string? editAssessment)

    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {

            var model = await _gradesService
                .GetGradesAsync(
                    userId,
                    schoolId,
                    classId,
                    subjectId,
                    term,
                    search,
                    editAssessment);


            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher grade records.");

            return View(new TeacherGradesViewModel
            {
                DataLoadFailed = true
            });
        }
    }


    /* ==========================================
       SAVE GRADE RECORDING DRAFT
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveGradeDraft(
        TeacherGradeDraftSaveViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Please check the assessment information.";
            }
            else
            {
                await _gradesService.SaveDraftAsync(
                    userId,
                    schoolId,
                    request);

                TempData["SuccessMessage"] =
                    "Grade draft saved successfully.";
            }
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to save Teacher grade draft.");

            TempData["ErrorMessage"] =
                "Grade draft could not be saved. " +
                "Please check the register and try again.";
        }

        return RedirectToAction(
            nameof(GradeRecording),
            new
            {
                classId = request.ClassId,
                subjectId = request.SubjectId,
                term = request.AcademicTerm
            });
    }


    /* ==========================================
       PUBLISH TEACHER GRADES
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishGradeAssessment(
        Guid classId,
        Guid subjectId,
        string assessmentName,
        string academicTerm,
        int academicYear)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            await _gradesService.PublishAssessmentAsync(
                userId,
                schoolId,
                classId,
                subjectId,
                assessmentName,
                academicTerm,
                academicYear);

            TempData["SuccessMessage"] =
                "Assessment grades published successfully.";
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to publish Teacher assessment.");

            TempData["ErrorMessage"] =
                "Publishing failed. Ensure every active " +
                "learner has saved marks and the assessment " +
                "contains only your unpublished grades.";
        }

        return RedirectToAction(
            nameof(GradeRecording),
            new
            {
                classId,
                subjectId,
                term = academicTerm
            });
    }



    /* ==========================================
       TEACHER COMMUNICATION MANAGEMENT
       GET: /Teacher/Communication
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> Communication(
        string? search,
        string? priority,
        string? targetType,
        string? status)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            var model = await _communicationService
                .GetCommunicationAsync(
                    userId,
                    schoolId,
                    searchTerm: search,
                    priority: priority,
                    targetType: targetType,
                    status: status);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher Communication.");

            return View(new TeacherCommunicationViewModel
            {
                DataLoadFailed = true
            });
        }
    }



    /* ==========================================
       CREATE TEACHER ANNOUNCEMENT
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> CreateAnnouncement()
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            var data = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!data.HasTeacherProfile || data.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Your Teacher profile or assigned classes " +
                    "could not be loaded.";

                return RedirectToAction(nameof(Communication));
            }

            var model = new TeacherCreateClassAnnouncementViewModel
            {
                Classes = data.Classes,
                Parents = data.Parents
            };

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Create Announcement for Teacher");

            TempData["ErrorMessage"] =
                "Unable to load assigned classes and parents.";

            return RedirectToAction(nameof(Communication));
        }
    }


    [HttpGet]
    public async Task<IActionResult> AnnouncementDetails(Guid id)
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();
        if (id == Guid.Empty) return NotFound();

        try
        {
            var data = await _communicationService.GetCommunicationAsync(userId, schoolId);
            var announcement = data.Announcements.FirstOrDefault(x => x.Id == id);
            if (announcement == null) return NotFound();
            return View(announcement);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load Teacher announcement details");
            return StatusCode(500, "Unable to display the announcement.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> CommunicationStatistics()
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            var model = await _communicationService.GetCommunicationAsync(userId, schoolId);
            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to load Teacher communication statistics");
            return View(new TeacherCommunicationViewModel { DataLoadFailed = true });
        }
    }


    /* ==========================================
       SEND TEACHER ANNOUNCEMENT
       CLASS OR SPECIFIC PARENT
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendClassAnnouncement(
        TeacherCreateClassAnnouncementViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "Please complete all required announcement fields.";

            return RedirectToAction(
                nameof(CreateAnnouncement));
        }

        try
        {
            var announcementId = await _communicationService
                .SendClassAnnouncementAsync(userId, schoolId, request);

            // The announcement and in-app notifications already exist.
            try
            {
                var result = await _parentEmailService.SendAsync(
                    announcementId, "announcement", request.Title,
                    "Priority: " + request.Priority + "\n\n" + request.Content);

                TempData["SuccessMessage"] =
                    $"Announcement created. Parent emails sent: {result.SentCount}.";
                if (result.FailedCount > 0)
                    TempData["ErrorMessage"] =
                        $"{result.FailedCount} Parent email(s) could not be delivered.";
            }
            catch (Exception emailError)
            {
                _logger.LogError(emailError,
                    "Announcement {AnnouncementId} saved but email dispatch failed",
                    announcementId);
                TempData["SuccessMessage"] =
                    "Announcement created; mobile notifications saved.";
                TempData["ErrorMessage"] =
                    "Parent email dispatch was unsuccessful. Check application logs.";
            }

            return RedirectToAction(
                nameof(Communication));
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send Teacher announcement. Target: {TargetType}",
                request.TargetType);

            TempData["ErrorMessage"] =
                "Unable to send the announcement. " +
                "Please check the Visual Studio output for details.";
        }

        return RedirectToAction(
            nameof(CreateAnnouncement));
    }


    /* ==========================================
       TEACHER EVENTS MANAGEMENT
       GET: /Teacher/Events
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> Events()
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            var model = await _eventsService
                .GetEventsAsync(userId, schoolId);

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher Events.");

            return View(new TeacherEventManagementViewModel
            {
                DataLoadFailed = true
            });
        }
    }


    /* ==========================================
       TEACHER EVENT DETAILS
       GET: /Teacher/EventDetails/{id}
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> EventDetails(Guid id)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (id == Guid.Empty)
        {
            return NotFound();
        }

        try
        {
            var schoolEvent = await _eventsService
                .GetEventDetailsAsync(
                    userId,
                    schoolId,
                    id);

            if (schoolEvent == null)
            {
                return NotFound();
            }

            return View(schoolEvent);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher Event Details: {EventId}",
                id);

            return StatusCode(
                500,
                "Unable to display the event.");
        }
    }


    /* ==========================================
       CREATE TEACHER EVENT - GET
       GET: /Teacher/CreateEvent
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            // Load assigned classes and eligible parents.

            var data = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!data.HasTeacherProfile ||
                data.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Your Teacher profile could not be loaded.";

                return RedirectToAction(nameof(Events));
            }

            if (!data.Classes.Any())
            {
                TempData["ErrorMessage"] =
                    "You have no assigned classes. " +
                    "Contact your School Administrator.";

                return RedirectToAction(nameof(Events));
            }

            var model = new TeacherCreateEventViewModel
            {
                EventDate = DateTime.Today,
                TargetType = "Class",
                Classes = data.Classes,
                Parents = data.Parents
            };

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher Create Event.");

            TempData["ErrorMessage"] =
                "Unable to load event creation details.";

            return RedirectToAction(nameof(Events));
        }
    }


    /* ==========================================
       CREATE TEACHER EVENT - POST
       POST: /Teacher/CreateEvent
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(
        TeacherCreateEventViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            // ==========================================
            // LOAD ASSIGNED CLASSES AND PARENTS
            // ==========================================

            var data = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!data.HasTeacherProfile ||
                data.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Your Teacher profile is unavailable.";

                return RedirectToAction(nameof(Events));
            }

            request.Classes = data.Classes;
            request.Parents = data.Parents;

            // ==========================================
            // VALIDATE FORM
            // ==========================================

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // ==========================================
            // CREATE EVENT IN SUPABASE
            // ==========================================

            // The service validates the Teacher's
            // assigned class.
            //
            // The Supabase RPC independently validates
            // the authenticated Teacher and recipients.

            var eventId = await _eventsService.CreateEventAsync(
                userId, schoolId, request);

            // The Supabase RPC already created Flutter notifications.
            try
            {
                var body = "Event: " + request.EventName +
                    "\nDate: " + request.EventDate!.Value.ToString("dd MMM yyyy") +
                    "\nTime: " + (request.StartTime?.ToString(@"hh\:mm") ?? "Not specified") +
                    " - " + (request.EndTime?.ToString(@"hh\:mm") ?? "Not specified") +
                    "\nVenue: " + (request.Venue ?? "Not specified") +
                    "\n\n" + (request.Description ?? "");

                var result = await _parentEmailService.SendAsync(
                    eventId, "event", request.EventName, body);

                TempData["SuccessMessage"] =
                    $"Event created. Parent emails sent: {result.SentCount}.";
                if (result.FailedCount > 0)
                    TempData["ErrorMessage"] =
                        $"{result.FailedCount} Parent email(s) could not be delivered.";
            }
            catch (Exception emailError)
            {
                _logger.LogError(emailError,
                    "Event {EventId} saved but email dispatch failed", eventId);
                TempData["SuccessMessage"] =
                    "Event created; mobile notifications saved.";
                TempData["ErrorMessage"] =
                    "Parent email dispatch was unsuccessful. Check application logs.";
            }

            return RedirectToAction(nameof(Events));
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to create Teacher event.");

            ModelState.AddModelError(
                string.Empty,
                "Unable to create the event. " +
                "Please check the details and try again.");
        }

        // If saving failed, restore dropdown options
        // so the user can correct the form.

        try
        {
            var data = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            request.Classes = data.Classes;
            request.Parents = data.Parents;
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to reload Teacher event options.");

            TempData["ErrorMessage"] =
                "Unable to reload event options.";

            return RedirectToAction(nameof(Events));
        }

        return View(request);
    }


    /* ==========================================
       EDIT TEACHER EVENT - GET
       GET: /Teacher/EditEvent/{id}
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> EditEvent(Guid id)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (id == Guid.Empty)
        {
            return NotFound();
        }

        try
        {
            // ==========================================
            // 1. LOAD EXISTING EVENT
            // ==========================================

            var model = await _eventsService
                .GetEventForEditingAsync(
                    userId,
                    schoolId,
                    id);

            if (model == null)
            {
                return NotFound();
            }

            // ==========================================
            // 2. LOAD ASSIGNED CLASSES AND PARENTS
            // ==========================================

            var communication = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!communication.HasTeacherProfile ||
                communication.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Unable to load your assigned classes.";

                return RedirectToAction(nameof(Events));
            }

            // ==========================================
            // 3. POPULATE EDIT FORM OPTIONS
            // ==========================================

            model.Classes = communication.Classes;
            model.Parents = communication.Parents;

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher event for editing: {EventId}",
                id);

            TempData["ErrorMessage"] =
                "Unable to load the event for editing.";

            return RedirectToAction(nameof(Events));
        }
    }


    /* ==========================================
       EDIT TEACHER EVENT - POST
       POST: /Teacher/EditEvent
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(
        TeacherEditEventViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (request == null ||
            request.Id == Guid.Empty)
        {
            return BadRequest();
        }

        try
        {
            // ==========================================
            // 1. VERIFY EVENT ACCESS
            // ==========================================

            var existingEvent = await _eventsService
                .GetEventDetailsAsync(
                    userId,
                    schoolId,
                    request.Id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            // ==========================================
            // 2. LOAD DROPDOWN OPTIONS
            // ==========================================

            var communication = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!communication.HasTeacherProfile ||
                communication.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Unable to verify assigned classes.";

                return RedirectToAction(nameof(Events));
            }

            request.Classes = communication.Classes;
            request.Parents = communication.Parents;

            // ==========================================
            // 3. VALIDATE FORM
            // ==========================================

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // ==========================================
            // 4. UPDATE EVENT
            // ==========================================

            // The service verifies event ownership.
            //
            // The Supabase RPC independently checks:
            // - Authenticated active Teacher
            // - Original assigned class
            // - Newly selected assigned class
            // - Eligible linked parent
            //
            // Parent recipients are synchronized
            // in the database transaction.

            await _eventsService.UpdateEventAsync(
                userId,
                schoolId,
                request);

            // ==========================================
            // 5. SUCCESS
            // ==========================================

            TempData["SuccessMessage"] =
                "Event updated successfully.";

            return RedirectToAction(
                nameof(EventDetails),
                new { id = request.Id });
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to update Teacher event: {EventId}",
                request.Id);

            ModelState.AddModelError(
                string.Empty,
                "Unable to update the event. " +
                "Please check the details and try again.");
        }

        // ==========================================
        // 6. RELOAD OPTIONS AFTER ERROR
        // ==========================================

        try
        {
            var communication = await _communicationService
                .GetCommunicationAsync(userId, schoolId);

            if (!communication.HasTeacherProfile ||
                communication.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Unable to reload assigned classes.";

                return RedirectToAction(nameof(Events));
            }

            request.Classes = communication.Classes;
            request.Parents = communication.Parents;
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to reload Teacher Edit Event options.");

            TempData["ErrorMessage"] =
                "Unable to reload event editing options.";

            return RedirectToAction(nameof(Events));
        }

        return View(request);
    }


    /* ==========================================
       EDIT TEACHER ANNOUNCEMENT - GET
       GET: /Teacher/EditAnnouncement/{id}
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> EditAnnouncement(Guid id)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (id == Guid.Empty)
        {
            return NotFound();
        }

        try
        {
            // ==========================================
            // 1. LOAD EXISTING ANNOUNCEMENT
            // ==========================================

            var model = await _communicationService
                .GetAnnouncementForEditingAsync(
                    userId,
                    schoolId,
                    id);

            if (model == null)
            {
                return NotFound();
            }

            // ==========================================
            // 2. DISPLAY EDIT PAGE
            // ==========================================

            // The service already supplies:
            //
            // - Announcement details
            // - Assigned classes
            // - Eligible parents
            // - Previously selected parent

            return View(model);
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher announcement for editing: {AnnouncementId}",
                id);

            TempData["ErrorMessage"] =
                "Unable to load the announcement for editing.";

            return RedirectToAction(nameof(Communication));
        }
    }


    /* ==========================================
       EDIT TEACHER ANNOUNCEMENT - POST
       POST: /Teacher/EditAnnouncement
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAnnouncement(
        TeacherEditAnnouncementViewModel request)
    {
        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        if (request == null ||
            request.Id == Guid.Empty)
        {
            return BadRequest();
        }

        // ==========================================
        // 1. VALIDATE AND SAVE
        // ==========================================

        if (ModelState.IsValid)
        {
            try
            {
                // The service validates the announcement
                // and calls the Teacher-only Supabase RPC.
                //
                // Database authorization checks:
                // - Active Teacher account
                // - Announcement ownership
                // - Assigned classes
                // - Eligible parent recipients

                await _communicationService
                    .UpdateAnnouncementAsync(
                        userId,
                        schoolId,
                        request);

                // ==================================
                // SUCCESS
                // ==================================

                TempData["SuccessMessage"] =
                    "Announcement updated successfully.";

                return RedirectToAction(
                    nameof(AnnouncementDetails),
                    new { id = request.Id });
            }
            catch (UnauthorizedAccessException)
            {
                return await RedirectToLoginAsync();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to update Teacher announcement: {AnnouncementId}",
                    request.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the announcement. " +
                    "Please check the details and try again.");
            }
        }

        // ==========================================
        // 2. RELOAD DROPDOWN OPTIONS
        // ==========================================

        // When validation fails, Razor must receive
        // Classes and Parents again so the Teacher
        // can correct the form without losing data.

        try
        {
            var communication = await _communicationService
                .GetCommunicationAsync(
                    userId,
                    schoolId);

            if (!communication.HasTeacherProfile ||
                communication.DataLoadFailed)
            {
                TempData["ErrorMessage"] =
                    "Unable to load assigned classes and parents.";

                return RedirectToAction(nameof(Communication));
            }

            request.Classes = communication.Classes;
            request.Parents = communication.Parents;
        }
        catch (UnauthorizedAccessException)
        {
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to reload Teacher announcement editing options.");

            TempData["ErrorMessage"] =
                "Unable to reload announcement editing options.";

            return RedirectToAction(nameof(Communication));
        }

        // ==========================================
        // 3. RETURN FORM WITH VALIDATION ERRORS
        // ==========================================

        return View(request);
    }


    /* ==========================================
       DELETE TEACHER ANNOUNCEMENT
       POST: /Teacher/DeleteAnnouncement
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(Guid id)
    {
        // ==========================================
        // 1. VERIFY LOGGED-IN TEACHER
        // ==========================================

        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        // ==========================================
        // 2. VALIDATE ANNOUNCEMENT ID
        // ==========================================

        if (id == Guid.Empty)
        {
            TempData["ErrorMessage"] =
                "Invalid announcement selected.";

            return RedirectToAction(nameof(Communication));
        }

        try
        {
            // ==========================================
            // 3. DELETE ANNOUNCEMENT
            // ==========================================

            await _communicationService
                .DeleteAnnouncementAsync(
                    userId,
                    schoolId,
                    id);

            // ==========================================
            // 4. SUCCESS MESSAGE
            // ==========================================

            TempData["SuccessMessage"] =
                "Announcement deleted successfully.";

            return RedirectToAction(nameof(Communication));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                ex,
                "Teacher could not delete announcement {AnnouncementId}",
                id);

            TempData["ErrorMessage"] =
                "You are not authorized to delete this announcement.";

            return RedirectToAction(nameof(Communication));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting Teacher announcement {AnnouncementId}",
                id);

            TempData["ErrorMessage"] =
                "Unable to delete the announcement. " +
                "Please try again.";

            return RedirectToAction(nameof(Communication));
        }
    }


    /* ==========================================
       DELETE TEACHER EVENT
       POST: /Teacher/DeleteEvent
       ========================================== */

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        // ==========================================
        // 1. VERIFY LOGGED-IN TEACHER
        // ==========================================

        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        // ==========================================
        // 2. VALIDATE EVENT ID
        // ==========================================

        if (id == Guid.Empty)
        {
            TempData["ErrorMessage"] =
                "Invalid event selected.";

            return RedirectToAction(nameof(Events));
        }

        try
        {
            // ==========================================
            // 3. DELETE EVENT
            // ==========================================

            await _eventsService.DeleteEventAsync(
                userId,
                schoolId,
                id);

            // ==========================================
            // 4. SUCCESS MESSAGE
            // ==========================================

            TempData["SuccessMessage"] =
                "Event deleted successfully.";

            return RedirectToAction(nameof(Events));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                ex,
                "Teacher could not delete event {EventId}",
                id);

            TempData["ErrorMessage"] =
                "You are not authorized to delete this event.";

            return RedirectToAction(nameof(Events));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting Teacher event {EventId}",
                id);

            TempData["ErrorMessage"] =
                "Unable to delete the event. " +
                "Please try again.";

            return RedirectToAction(nameof(Events));
        }
    }


    /* ==========================================
       TEACHER PROFILE
       GET: /Teacher/Profile
       ========================================== */

    [HttpGet]
    public async Task<IActionResult> Profile(
        [FromServices] TeacherProfileService profileService)
    {
        // ==========================================
        // 1. VERIFY LOGGED-IN TEACHER
        // ==========================================

        if (!TryGetIdentity(
            out var userId,
            out var schoolId))
        {
            return await RedirectToLoginAsync();
        }

        try
        {
            // ==========================================
            // 2. LOAD TEACHER PROFILE
            // ==========================================

            var model = await profileService
                .GetProfileAsync(
                    userId,
                    schoolId);

            // ==========================================
            // 3. DISPLAY PROFILE
            // ==========================================

            return View(model);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                ex,
                "Unauthorized Teacher Profile access for {UserId}",
                userId);

            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load Teacher Profile for {UserId}",
                userId);

            TempData["ErrorMessage"] =
                "Unable to load your Teacher profile. " +
                "Please try again.";

            return RedirectToAction(nameof(Communication));
        }
    }

    // =========================================
    // EDIT PROFILE (GET)
    // =========================================
    [HttpGet]
    public async Task<IActionResult> EditProfile(
        [FromServices] TeacherProfileService profileService)
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            var model = await profileService.GetEditProfileAsync(userId, schoolId);
            return View(model);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Teacher profile edit access denied for {UserId}", userId);
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not load Teacher edit profile for {UserId}", userId);
            TempData["ErrorMessage"] = "Unable to load your profile for editing.";
            return RedirectToAction(nameof(Profile));
        }
    }

    // =========================================
    // EDIT PROFILE (POST)
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(
        TeacherEditProfileViewModel request,
        [FromServices] TeacherProfileService profileService)
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            if (!ModelState.IsValid)
            {
                var current = await profileService.GetEditProfileAsync(userId, schoolId);
                // Do not trust the POSTed display-only account fields.
                request.Email = current.Email;
                request.Role = current.Role;
                request.SchoolName = current.SchoolName;
                request.SchoolAddress = current.SchoolAddress;
                request.IsActive = current.IsActive;
                return View(request);
            }

            await profileService.UpdateProfileAsync(userId, schoolId, request);
            TempData["SuccessMessage"] = "Your profile was updated successfully.";
            return RedirectToAction(nameof(Profile));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized profile update for {UserId}", userId);
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var current = await profileService.GetEditProfileAsync(userId, schoolId);
            request.Email = current.Email;
            request.Role = current.Role;
            request.SchoolName = current.SchoolName;
            request.SchoolAddress = current.SchoolAddress;
            request.IsActive = current.IsActive;
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not save Teacher profile for {UserId}", userId);
            TempData["ErrorMessage"] = "Could not save your profile. Please try again.";
            return RedirectToAction(nameof(Profile));
        }
    }

    // =========================================
    // UPLOAD PROFILE PICTURE (POST)
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProfilePicture(
        IFormFile? picture,
        [FromServices] TeacherProfileService profileService)
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            await profileService.UploadProfilePictureAsync(userId, schoolId, picture);
            TempData["SuccessMessage"] = "Profile picture uploaded successfully.";
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized Teacher avatar upload for {UserId}", userId);
            return await RedirectToLoginAsync();
        }
        catch (ArgumentException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Teacher profile picture upload failed for {UserId}", userId);
            TempData["ErrorMessage"] = "Could not upload your picture. Please try again.";
        }

        return RedirectToAction(nameof(Profile));
    }

    // =========================================
    // REMOVE PROFILE PICTURE (POST)
    // =========================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProfilePicture(
        [FromServices] TeacherProfileService profileService)
    {
        if (!TryGetIdentity(out var userId, out var schoolId))
            return await RedirectToLoginAsync();

        try
        {
            await profileService.RemoveProfilePictureAsync(userId, schoolId);
            TempData["SuccessMessage"] = "Profile picture removed successfully.";
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized Teacher avatar removal for {UserId}", userId);
            return await RedirectToLoginAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Teacher profile picture removal failed for {UserId}", userId);
            TempData["ErrorMessage"] = "Could not remove your picture. Please try again.";
        }

        return RedirectToAction(nameof(Profile));
    }


    private bool TryGetIdentity(out Guid userId, out Guid schoolId)
    {
        var hasUserId = Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        var hasSchoolId = Guid.TryParse(
            User.FindFirstValue("school_id"), out schoolId);

        return hasUserId && hasSchoolId;
    }

    private async Task<IActionResult> RedirectToLoginAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}
