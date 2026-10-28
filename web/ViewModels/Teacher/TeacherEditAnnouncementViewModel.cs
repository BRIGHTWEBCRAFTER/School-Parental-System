
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

/* ==========================================
   TEACHER EDIT ANNOUNCEMENT VIEWMODEL
   ========================================== */

//
// Class Name       : TeacherEditAnnouncementViewModel
// Purpose          : Supplies teacher edit announcement view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEditAnnouncementViewModel object.
//
public class TeacherEditAnnouncementViewModel : IValidatableObject
{
    // ==========================================
    // ANNOUNCEMENT ID
    // ==========================================

    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


    // ==========================================
    // ANNOUNCEMENT DETAILS
    // ==========================================

    [Required(ErrorMessage = "Announcement title is required.")]
    [StringLength(
        150,
        ErrorMessage = "Title cannot exceed 150 characters.")]
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";


    [Required(ErrorMessage = "Announcement message is required.")]
    [StringLength(
        5000,
        ErrorMessage = "Message cannot exceed 5000 characters.")]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";


    [Required(ErrorMessage = "Select a priority.")]
    [RegularExpression(
        "^(Normal|Important|Urgent)$",
        ErrorMessage = "Select a valid priority.")]
    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";


    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }


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


    // Teacher's assigned class.

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid?
    //
    public Guid? TargetClassId { get; set; }


    // One eligible parent for Specific Users.

    //
    // Property Name    : ParentUserId
    // Purpose          : Identifies the related parent user record.
    // Data Type        : Guid?
    //
    public Guid? ParentUserId { get; set; }


    // ==========================================
    // ACKNOWLEDGEMENT
    // ==========================================

    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }


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
        // Validate announcement ID.

        if (Id == Guid.Empty)
        {
            yield return new ValidationResult(
                "Invalid announcement ID.",
                new[] { nameof(Id) });
        }


        // Validate target class.

        if (!TargetClassId.HasValue ||
            TargetClassId.Value == Guid.Empty)
        {
            yield return new ValidationResult(
                "Select an assigned class.",
                new[] { nameof(TargetClassId) });
        }


        // Validate specific parent.

        if (TargetType == "Specific Users" &&
            (!ParentUserId.HasValue ||
             ParentUserId.Value == Guid.Empty))
        {
            yield return new ValidationResult(
                "Select one eligible parent.",
                new[] { nameof(ParentUserId) });
        }


        // Only Important or Urgent announcements
        // may require acknowledgement.

        if (AcknowledgementRequired &&
            Priority != "Important" &&
            Priority != "Urgent")
        {
            yield return new ValidationResult(
                "Acknowledgement is available only for " +
                "Important or Urgent announcements.",
                new[]
                {
                    nameof(AcknowledgementRequired),
                    nameof(Priority)
                });
        }
    }
}
