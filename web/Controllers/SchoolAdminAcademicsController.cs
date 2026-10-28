
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Controllers;

//
// Class Name       : SchoolAdminAcademicsController
// Purpose           : Manages school subjects and the weekly class timetable.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
[Authorize(Roles = "School Admin")]
public class SchoolAdminAcademicsController : Controller
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseSessionService _session;
    private readonly ILogger<SchoolAdminAcademicsController> _logger;

    //
    // Method Name       : SchoolAdminAcademicsController
    // Purpose           : Initializes required dependencies for SchoolAdminAcademicsController.
    // Re-use            : Invoked by SchoolAdminAcademicsController or MVC requests.
    // Input Parameters  : Supabase.Client supabase; SupabaseSessionService session;
    //                      ILogger<SchoolAdminAcademicsController> logger
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public SchoolAdminAcademicsController(
        Supabase.Client supabase,
        SupabaseSessionService session,
        ILogger<SchoolAdminAcademicsController> logger)
    {
        _supabase = supabase;
        _session = session;
        _logger = logger;
    }


    // ==========================================
    // GET AUTHORIZED SCHOOL
    // ==========================================

    //
    // Method Name       : GetSchoolIdAsync
    // Purpose           : Identify the school linked to the signed-in school administrator.
    // Re-use            : Invoked by SchoolAdminAcademicsController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<Guid?>
    //
    private async Task<Guid?> GetSchoolIdAsync()
    {
        var value = User.FindFirst("school_id")?.Value;

        if (!Guid.TryParse(value, out var schoolId) ||
            schoolId == Guid.Empty ||
            !await _session.RestoreAsync())
        {
            return null;
        }

        return schoolId;
    }


    // ==========================================
    // GET: SUBJECTS & TIMETABLE
    // ==========================================

    //
    // Method Name       : Index
    // Purpose           : Load the subject and weekly timetable management page.
    // Re-use            : Invoked by SchoolAdminAcademicsController or MVC requests.
    // Input Parameters  : None
    // Output Type       : Task<IActionResult>
    //
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        try
        {
            var subjectsResponse = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            var classesResponse = await _supabase
                .From<SchoolClassRecord>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            var teachersResponse = await _supabase
                .From<TeacherRecord>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            var usersResponse = await _supabase
                .From<UserProfile>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            var schedulesResponse = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Get();


            var subjects = subjectsResponse.Models
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            var classes = classesResponse.Models
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();

            var teachers = teachersResponse.Models
                .Where(x => x.IsActive)
                .ToList();

            var users = usersResponse.Models
                .Where(x => x.IsActive)
                .ToList();


            var classNames = classes.ToDictionary(
                x => x.Id,
                x => x.Name);

            var subjectNames = subjects.ToDictionary(
                x => x.Id,
                x => x.Name);

            var teacherNames = teachers.ToDictionary(
                x => x.Id,
                x =>
                    users.FirstOrDefault(
                        u => u.Id == x.UserId)?.FullName
                    ?? x.EmployeeId);


            var lessons = schedulesResponse.Models
                .Where(x =>
                    classNames.ContainsKey(x.ClassId) &&
                    subjectNames.ContainsKey(x.SubjectId))
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .Select(x => new AcademicTimetableItem
                {
                    Id = x.Id,

                    ClassId = x.ClassId,

                    ClassName = classNames[x.ClassId],

                    SubjectName = subjectNames[x.SubjectId],

                    TeacherName =
                        x.TeacherId.HasValue &&
                        teacherNames.TryGetValue(
                            x.TeacherId.Value,
                            out var teacherName)
                            ? teacherName
                            : "Not assigned",

                    TeacherId = x.TeacherId,

                    DayOfWeek = x.DayOfWeek,

                    StartTime = x.StartTime.ToString(@"hh\:mm"),

                    EndTime = x.EndTime.ToString(@"hh\:mm"),

                    Room = x.Room ?? "Not assigned"
                })
                .ToList();


            return View(new SchoolAdminAcademicsViewModel
            {
                Subjects = subjects,

                Classes = classes,

                Teachers = teachers
                    .Select(x => new AcademicTeacherOption
                    {
                        Id = x.Id,

                        Name = teacherNames[x.Id]
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),

                Lessons = lessons
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to load school academic management.");

            TempData["ErrorMessage"] =
                "Could not load subjects and timetable.";

            return RedirectToAction(
                "Dashboard",
                "SchoolAdmin");
        }
    }


    // ==========================================
    // POST: ADD SUBJECT
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSubject(
        [Bind(Prefix = "NewSubject")]
        AddAcademicSubjectInput input)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "Please check the subject name and code.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var code = input.Code.Trim().ToUpperInvariant();

            var existing = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            if (existing.Models.Any(x =>
                string.Equals(
                    x.Code,
                    code,
                    StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] =
                    "A subject with that code already exists.";

                return RedirectToAction(nameof(Index));
            }

            var now = DateTime.UtcNow;

            var newSubject = new SubjectRecord
            {
                Id = Guid.NewGuid(),

                SchoolId = schoolId.Value,

                Name = input.Name.Trim(),

                Code = code,

                Description =
                    string.IsNullOrWhiteSpace(input.Description)
                        ? null
                        : input.Description.Trim(),

                IsActive = true,

                CreatedAt = now,

                UpdatedAt = now
            };

            await _supabase
                .From<SubjectRecord>()
                .Insert(newSubject);

            TempData["SuccessMessage"] =
                "Subject added successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not add subject.");

            TempData["ErrorMessage"] =
                "Could not save the subject.";
        }

        return RedirectToAction(nameof(Index));
    }


    // ==========================================
    // POST: ADD TIMETABLE LESSON
    // ==========================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLesson(
        [Bind(Prefix = "NewLesson")]
        AddAcademicLessonInput input)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (!ModelState.IsValid ||
            input.ClassId == Guid.Empty ||
            input.SubjectId == Guid.Empty)
        {
            TempData["ErrorMessage"] =
                "Please complete the timetable details.";

            return RedirectToAction(nameof(Index));
        }


        if (!TimeSpan.TryParse(
                input.StartTime,
                CultureInfo.InvariantCulture,
                out var start) ||
            !TimeSpan.TryParse(
                input.EndTime,
                CultureInfo.InvariantCulture,
                out var end) ||
            start < TimeSpan.Zero ||
            end >= TimeSpan.FromDays(1) ||
            start >= end)
        {
            TempData["ErrorMessage"] =
                "Enter a valid start and end time.";

            return RedirectToAction(nameof(Index));
        }


        try
        {
            // VERIFY CLASS BELONGS TO SCHOOL

            var classResponse = await _supabase
                .From<SchoolClassRecord>()
                .Where(x => x.Id == input.ClassId)
                .Get();

            if (!classResponse.Models.Any(x =>
                x.SchoolId == schoolId.Value && x.IsActive))
            {
                throw new InvalidOperationException(
                    "The selected class is not available.");
            }


            // VERIFY SUBJECT BELONGS TO SCHOOL

            var subjectResponse = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.Id == input.SubjectId)
                .Get();

            if (!subjectResponse.Models.Any(x =>
                x.SchoolId == schoolId.Value && x.IsActive))
            {
                throw new InvalidOperationException(
                    "The selected subject is not available.");
            }


            // VERIFY OPTIONAL TEACHER

            if (input.TeacherId.HasValue)
            {
                var teacherResponse = await _supabase
                    .From<TeacherRecord>()
                    .Where(x => x.Id == input.TeacherId.Value)
                    .Get();

                if (!teacherResponse.Models.Any(x =>
                    x.SchoolId == schoolId.Value && x.IsActive))
                {
                    throw new InvalidOperationException(
                        "The selected teacher is not available.");
                }
            }


            // CHECK TIME CONFLICTS

            var existingSchedules = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.DayOfWeek == input.DayOfWeek)
                .Get();

            var hasConflict = existingSchedules.Models.Any(x =>
                start < x.EndTime &&
                end > x.StartTime &&
                (
                    x.ClassId == input.ClassId ||
                    (
                        input.TeacherId.HasValue &&
                        x.TeacherId == input.TeacherId
                    )
                ));

            if (hasConflict)
            {
                TempData["ErrorMessage"] =
                    "That class or teacher already has a " +
                    "lesson during the selected time.";

                return RedirectToAction(nameof(Index));
            }


            // SAVE TIMETABLE LESSON

            var now = DateTime.UtcNow;

            var newLesson = new SchoolAdminScheduleRecord
            {
                Id = Guid.NewGuid(),

                SchoolId = schoolId.Value,

                ClassId = input.ClassId,

                SubjectId = input.SubjectId,

                TeacherId = input.TeacherId,

                DayOfWeek = input.DayOfWeek,

                StartTime = start,

                EndTime = end,

                Room = string.IsNullOrWhiteSpace(input.Room)
                    ? null
                    : input.Room.Trim(),

                CreatedAt = now,

                UpdatedAt = now
            };

            await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Insert(newLesson);

            TempData["SuccessMessage"] =
                "Timetable lesson added successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Could not create timetable lesson.");

            TempData["ErrorMessage"] =
                "Unable to save the timetable lesson. " +
                "Please verify the selected class and subject.";
        }

        return RedirectToAction(nameof(Index));
    }


    //
    // Method Name       : EditSubject
    // Purpose           : Updates an existing school subject.
    // Re-use            : Subject Edit action.
    // Input Parameters  : Subject ID, name, code and description.
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSubject(
        Guid id,
        string name,
        string code,
        string? description)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        name = name?.Trim() ?? "";
        code = code?.Trim().ToUpperInvariant() ?? "";

        if (id == Guid.Empty ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(code) ||
            name.Length > 100 ||
            code.Length > 20 ||
            description?.Length > 255)
        {
            TempData["ErrorMessage"] =
                "Enter a valid subject name and code.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var response = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.Id == id)
                .Get();

            var subject = response.Models.FirstOrDefault();

            if (subject == null ||
                subject.SchoolId != schoolId.Value)
                return NotFound();

            var schoolSubjects = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.SchoolId == schoolId.Value)
                .Get();

            if (schoolSubjects.Models.Any(x =>
                x.Id != id &&
                string.Equals(
                    x.Code,
                    code,
                    StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] =
                    "Another subject already uses that code.";

                return RedirectToAction(nameof(Index));
            }

            subject.Name = name;
            subject.Code = code;
            subject.Description =
                string.IsNullOrWhiteSpace(description)
                    ? null
                    : description.Trim();

            subject.UpdatedAt = DateTime.UtcNow;

            await _supabase
                .From<SubjectRecord>()
                .Update(subject);

            TempData["SuccessMessage"] =
                "Subject updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to update subject {Id}", id);

            TempData["ErrorMessage"] =
                "The subject could not be updated.";
        }

        return RedirectToAction(nameof(Index));
    }


    //
    // Method Name       : DeleteSubject
    // Purpose           : Deactivates a subject without
    //                     deleting historical grades.
    // Re-use            : Subject Delete action.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _supabase
                .From<SubjectRecord>()
                .Where(x => x.Id == id)
                .Get();

            var subject = response.Models.FirstOrDefault();

            if (subject == null ||
                subject.SchoolId != schoolId.Value)
                return NotFound();

            var lessons = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.SubjectId == id)
                .Get();

            if (lessons.Models.Any())
            {
                TempData["ErrorMessage"] =
                    "Remove the subject's timetable " +
                    "lessons before deleting it.";

                return RedirectToAction(nameof(Index));
            }

            // Soft delete: preserve subject grades.
            subject.IsActive = false;
            subject.UpdatedAt = DateTime.UtcNow;

            await _supabase
                .From<SubjectRecord>()
                .Update(subject);

            TempData["SuccessMessage"] =
                "Subject removed from the active list.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to remove subject {Id}", id);

            TempData["ErrorMessage"] =
                "The subject could not be removed.";
        }

        return RedirectToAction(nameof(Index));
    }


    //
    // Method Name       : EditLesson
    // Purpose           : Updates lesson time, teacher,
    //                     day and room.
    // Re-use            : Timetable Edit action.
    // Input Parameters  : Lesson ID and updated details.
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLesson(
        Guid id,
        int dayOfWeek,
        string startTime,
        string endTime,
        Guid? teacherId,
        string? room)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        if (id == Guid.Empty ||
            dayOfWeek < 1 ||
            dayOfWeek > 5 ||
            !TimeSpan.TryParse(
                startTime,
                CultureInfo.InvariantCulture,
                out var start) ||
            !TimeSpan.TryParse(
                endTime,
                CultureInfo.InvariantCulture,
                out var end) ||
            start < TimeSpan.Zero ||
            end >= TimeSpan.FromDays(1) ||
            start >= end ||
            room?.Length > 50)
        {
            TempData["ErrorMessage"] =
                "Invalid timetable details.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var response = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.Id == id)
                .Get();

            var lesson = response.Models.FirstOrDefault();

            if (lesson == null)
                return NotFound();

            // Verify that the existing class
            // belongs to the signed-in school.
            var classes = await _supabase
                .From<SchoolClassRecord>()
                .Where(x => x.Id == lesson.ClassId)
                .Get();

            if (!classes.Models.Any(x =>
                x.SchoolId == schoolId.Value &&
                x.IsActive))
                return Forbid();

            // Verify selected teacher belongs to
            // this school and is active.
            if (teacherId.HasValue)
            {
                var teachers = await _supabase
                    .From<TeacherRecord>()
                    .Where(x => x.Id == teacherId.Value)
                    .Get();

                if (!teachers.Models.Any(x =>
                    x.SchoolId == schoolId.Value &&
                    x.IsActive))
                    return Forbid();
            }

            // Prevent overlapping lessons for the
            // same class or teacher.
            var schedules = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.DayOfWeek == dayOfWeek)
                .Get();

            var conflict = schedules.Models.Any(x =>
                x.Id != id &&
                start < x.EndTime &&
                end > x.StartTime &&
                (x.ClassId == lesson.ClassId ||
                 (teacherId.HasValue &&
                  x.TeacherId == teacherId)));

            if (conflict)
            {
                TempData["ErrorMessage"] =
                    "Class or teacher timetable conflict.";

                return RedirectToAction(nameof(Index));
            }

            lesson.SchoolId = schoolId.Value;
            lesson.DayOfWeek = dayOfWeek;
            lesson.StartTime = start;
            lesson.EndTime = end;
            lesson.TeacherId = teacherId;
            lesson.Room =
                string.IsNullOrWhiteSpace(room)
                    ? null
                    : room.Trim();

            lesson.UpdatedAt = DateTime.UtcNow;

            await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Update(lesson);

            TempData["SuccessMessage"] =
                "Timetable lesson updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to update lesson {Id}", id);

            TempData["ErrorMessage"] =
                "The lesson could not be updated.";
        }

        return RedirectToAction(nameof(Index));
    }


    //
    // Method Name       : DeleteLesson
    // Purpose           : Removes a timetable lesson
    //                     belonging to this school.
    // Re-use            : Timetable Delete action.
    // Input Parameters  : Guid id
    // Output Type       : Task<IActionResult>
    //
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        var schoolId = await GetSchoolIdAsync();

        if (!schoolId.HasValue)
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.Id == id)
                .Get();

            var lesson = response.Models.FirstOrDefault();

            if (lesson == null)
                return NotFound();

            var classes = await _supabase
                .From<SchoolClassRecord>()
                .Where(x => x.Id == lesson.ClassId)
                .Get();

            if (!classes.Models.Any(x =>
                x.SchoolId == schoolId.Value))
                return Forbid();

            await _supabase
                .From<SchoolAdminScheduleRecord>()
                .Where(x => x.Id == id)
                .Delete();

            TempData["SuccessMessage"] =
                "Timetable lesson deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Failed to delete lesson {Id}", id);

            TempData["ErrorMessage"] =
                "The timetable lesson could not be deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

}
