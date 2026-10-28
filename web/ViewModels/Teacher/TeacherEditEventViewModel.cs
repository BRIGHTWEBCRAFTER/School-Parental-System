
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherEditEventViewModel
// Purpose          : Supplies teacher edit event view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEditEventViewModel object.
//
public class TeacherEditEventViewModel : IValidatableObject
{
    // ==========================================
    // EVENT ID
    // ==========================================

    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


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

    [StringLength(
        500,
        ErrorMessage = "Description cannot exceed 500 characters.")]
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
    public DateTime? EventDate { get; set; }

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

    [StringLength(
        150,
        ErrorMessage = "Venue cannot exceed 150 characters.")]
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

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }

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
    // VALIDATION
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
        if (Id == Guid.Empty)
        {
            yield return new ValidationResult(
                "Invalid event ID.",
                new[] { nameof(Id) });
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
