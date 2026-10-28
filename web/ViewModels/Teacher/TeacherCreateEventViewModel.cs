
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

/* ==========================================
   TEACHER CREATE EVENT VIEWMODEL
   ========================================== */

//
// Class Name       : TeacherCreateEventViewModel
// Purpose          : Supplies teacher create event view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherCreateEventViewModel object.
//
public class TeacherCreateEventViewModel : IValidatableObject
{
    // ==========================================
    // EVENT DETAILS
    // ==========================================

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(150)]
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";

    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Event date is required.")]
    [DataType(DataType.Date)]
    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime?
    //
    public DateTime? EventDate { get; set; } = DateTime.Today;

    [DataType(DataType.Time)]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? StartTime { get; set; }

    [DataType(DataType.Time)]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? EndTime { get; set; }

    //
    // Property Name    : Venue
    // Purpose          : Gets or sets the venue.
    // Data Type        : string
    //
    public string Venue { get; set; } = "";


    // ==========================================
    // TARGET AUDIENCE
    // ==========================================

    [Required(ErrorMessage = "Select a target type.")]
    [RegularExpression(
        "^(Class|Specific Users)$",
        ErrorMessage = "Select a valid target type.")]
    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "Class";

    // Must belong to the Teacher's
    // assigned classes.

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }

    // Required for Specific Users only.
    // Must be linked to an active learner
    // in the selected assigned class.

    //
    // Property Name    : ParentUserId
    // Purpose          : Identifies the related parent user record.
    // Data Type        : Guid?
    //
    public Guid? ParentUserId { get; set; }


    // ==========================================
    // DROPDOWN OPTIONS
    // ==========================================

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TeacherCommunicationClassOptionViewModel>
    //
    public List<TeacherCommunicationClassOptionViewModel>
        Classes
    { get; set; } = new();

    //
    // Property Name    : Parents
    // Purpose          : Collection of parents used by this model.
    // Data Type        : List<TeacherEligibleParentViewModel>
    //
    public List<TeacherEligibleParentViewModel>
        Parents
    { get; set; } = new();


    // ==========================================
    // FORM VALIDATION
    // ==========================================

    //
    // Method Name      : Validate
    // Purpose          : Validates the supplied model fields and returns any validation errors.
    // Re-use           : Called by ASP.NET validation or other application components.
    // Input Parameters : Method parameters shown in the signature.
    // Output Type      : IEnumerable<ValidationResult>
    //
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (EventDate.HasValue &&
            EventDate.Value.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "Event date cannot be in the past.",
                new[] { nameof(EventDate) });
        }

        if (StartTime.HasValue &&
            EndTime.HasValue &&
            EndTime.Value <= StartTime.Value)
        {
            yield return new ValidationResult(
                "End time must be later than start time.",
                new[] { nameof(EndTime) });
        }

        if (!TargetClassId.HasValue ||
            TargetClassId.Value == Guid.Empty)
        {
            yield return new ValidationResult(
                "Select an assigned class.",
                new[] { nameof(TargetClassId) });
        }

        if (TargetType == "Specific Users" &&
            (!ParentUserId.HasValue ||
             ParentUserId.Value == Guid.Empty))
        {
            yield return new ValidationResult(
                "Select one eligible parent.",
                new[] { nameof(ParentUserId) });
        }
    }
}
