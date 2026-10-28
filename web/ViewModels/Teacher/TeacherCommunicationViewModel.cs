
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;


/* ==========================================
   TEACHER COMMUNICATION MANAGEMENT
   ========================================== */

//
// Class Name       : TeacherCommunicationViewModel
// Purpose          : Supplies teacher communication view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherCommunicationViewModel object.
//
public class TeacherCommunicationViewModel
{
    //
    // Property Name    : HasTeacherProfile
    // Purpose          : Indicates whether teacher profile is available.
    // Data Type        : bool
    //
    public bool HasTeacherProfile { get; set; }

    //
    // Property Name    : DataLoadFailed
    // Purpose          : Gets or sets the data load failed.
    // Data Type        : bool
    //
    public bool DataLoadFailed { get; set; }


    // ==========================================
    // SEARCH AND FILTERS
    // ==========================================

    //
    // Property Name    : SearchTerm
    // Purpose          : Gets or sets the search term.
    // Data Type        : string
    //
    public string SearchTerm { get; set; } = "";

    //
    // Property Name    : SelectedPriority
    // Purpose          : Gets or sets the selected priority.
    // Data Type        : string
    //
    public string SelectedPriority { get; set; } = "";

    //
    // Property Name    : SelectedTarget
    // Purpose          : Gets or sets the selected target.
    // Data Type        : string
    //
    public string SelectedTarget { get; set; } = "";

    //
    // Property Name    : SelectedStatus
    // Purpose          : Gets or sets the selected status.
    // Data Type        : string
    //
    public string SelectedStatus { get; set; } = "";


    //
    // Property Name    : Priorities
    // Purpose          : Collection of priorities used by this model.
    // Data Type        : List<string>
    //
    public List<string> Priorities { get; set; } = new()
    {
        "Normal",
        "Important",
        "Urgent"
    };


    // Teachers have only two allowed target types.

    //
    // Property Name    : TargetTypes
    // Purpose          : Collection of target types used by this model.
    // Data Type        : List<string>
    //
    public List<string> TargetTypes { get; set; } = new()
    {
        "Class",
        "Specific Users"
    };


    // ==========================================
    // ASSIGNED CLASSES
    // ==========================================

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TeacherCommunicationClassOptionViewModel>
    //
    public List<TeacherCommunicationClassOptionViewModel>
        Classes
    { get; set; } = new();


    // ==========================================
    // ELIGIBLE PARENTS
    // ==========================================

    //
    // Property Name    : Parents
    // Purpose          : Collection of parents used by this model.
    // Data Type        : List<TeacherEligibleParentViewModel>
    //
    public List<TeacherEligibleParentViewModel>
        Parents
    { get; set; } = new();


    // ==========================================
    // ANNOUNCEMENTS
    // ==========================================

    //
    // Property Name    : Announcements
    // Purpose          : Collection of announcements used by this model.
    // Data Type        : List<TeacherCommunicationAnnouncementViewModel>
    //
    public List<TeacherCommunicationAnnouncementViewModel>
        Announcements
    { get; set; } = new();


    // ==========================================
    // COMMUNICATION STATISTICS
    // ==========================================

    //
    // Property Name    : TotalAnnouncements
    // Purpose          : Total announcements displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalAnnouncements =>
        Announcements.Count;


    //
    // Property Name    : ImportantNoticeCount
    // Purpose          : Computed or stored important notice count value.
    // Data Type        : int
    //
    public int ImportantNoticeCount =>
        Announcements.Count(x =>
            x.Priority == "Important" ||
            x.Priority == "Urgent");


    //
    // Property Name    : TotalRecipients
    // Purpose          : Total recipients displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRecipients =>
        Announcements.Sum(x =>
            x.RecipientCount);


    //
    // Property Name    : TotalRead
    // Purpose          : Total read displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRead =>
        Announcements.Sum(x =>
            x.ReadCount);


    //
    // Property Name    : TotalAcknowledged
    // Purpose          : Total acknowledged displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalAcknowledged =>
        Announcements.Sum(x =>
            x.AcknowledgedCount);


    //
    // Property Name    : ReadRate
    // Purpose          : Computed or stored read rate value.
    // Data Type        : decimal
    //
    public decimal ReadRate =>
        TotalRecipients == 0
            ? 0
            : Math.Round(
                (decimal)TotalRead /
                TotalRecipients * 100,
                1);
}


/* ==========================================
   TEACHER ASSIGNED CLASS OPTION
   ========================================== */

//
// Class Name       : TeacherCommunicationClassOptionViewModel
// Purpose          : Supplies a selectable teacher communication class option view model item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherCommunicationClassOptionViewModel object.
//
public class TeacherCommunicationClassOptionViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";

    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    //
    // Property Name    : DisplayName
    // Purpose          : Gets or sets the display name.
    // Data Type        : string
    //
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Code)
            ? Name
            : $"{Name} ({Code})";
}


/* ==========================================
   TEACHER ANNOUNCEMENT RECORD
   ========================================== */

//
// Class Name       : TeacherCommunicationAnnouncementViewModel
// Purpose          : Supplies teacher communication announcement view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherCommunicationAnnouncementViewModel object.
//
public class TeacherCommunicationAnnouncementViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";

    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";


    // ==========================================
    // TARGET AUDIENCE
    // ==========================================

    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "Class";

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : TargetClassId
    // Purpose          : Identifies the related target class record.
    // Data Type        : Guid
    //
    public Guid TargetClassId { get; set; }


    //
    // Property Name    : TargetDisplay
    // Purpose          : Gets or sets the target display.
    // Data Type        : string
    //
    public string TargetDisplay =>
        TargetType switch
        {
            "Class" =>
                string.IsNullOrWhiteSpace(ClassName)
                    ? "Assigned Class"
                    : ClassName,

            "Specific Users" =>
                string.IsNullOrWhiteSpace(ClassName)
                    ? "Specific Parent"
                    : $"Specific Parent — {ClassName}",

            _ => TargetType
        };


    // ==========================================
    // ANNOUNCEMENT DETAILS
    // ==========================================

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }

    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }


    // ==========================================
    // RECIPIENT ENGAGEMENT
    // ==========================================

    //
    // Property Name    : RecipientCount
    // Purpose          : Computed or stored recipient count value.
    // Data Type        : int
    //
    public int RecipientCount { get; set; }

    //
    // Property Name    : ReadCount
    // Purpose          : Computed or stored read count value.
    // Data Type        : int
    //
    public int ReadCount { get; set; }

    //
    // Property Name    : AcknowledgedCount
    // Purpose          : Computed or stored acknowledged count value.
    // Data Type        : int
    //
    public int AcknowledgedCount { get; set; }


    // ==========================================
    // ANNOUNCEMENT STATUS
    // ==========================================

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        ExpiresAt.HasValue &&
        ExpiresAt.Value <= DateTime.UtcNow
            ? "Expired"
            : "Active";
}


/* ==========================================
   CREATE TEACHER ANNOUNCEMENT
   ========================================== */

//
// Class Name       : TeacherCreateClassAnnouncementViewModel
// Purpose          : Supplies teacher create class announcement view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherCreateClassAnnouncementViewModel object.
//
public class TeacherCreateClassAnnouncementViewModel
{
    // ==========================================
    // ANNOUNCEMENT DETAILS
    // ==========================================

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, MinimumLength = 1)]
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";


    [Required(ErrorMessage = "Message is required.")]
    [StringLength(5000, MinimumLength = 1)]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";


    [Required(ErrorMessage = "Priority is required.")]
    [RegularExpression(
        "^(Normal|Important|Urgent)$",
        ErrorMessage = "Select a valid priority.")]
    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "Normal";


    // ==========================================
    // TARGET TYPE
    // ==========================================

    [Required(ErrorMessage = "Target type is required.")]
    [RegularExpression(
        "^(Class|Specific Users)$",
        ErrorMessage = "Select a valid target type.")]
    //
    // Property Name    : TargetType
    // Purpose          : Gets or sets the target type.
    // Data Type        : string
    //
    public string TargetType { get; set; } = "Class";


    // ==========================================
    // TARGET CLASS
    // ==========================================

    [Required(ErrorMessage = "Select a class.")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid?
    //
    public Guid? ClassId { get; set; }


    // ==========================================
    // SPECIFIC PARENT
    // ==========================================

    // Required only when TargetType is
    // "Specific Users".
    //
    // The service and Supabase RPC validate
    // the selected parent before sending.

    //
    // Property Name    : ParentUserId
    // Purpose          : Identifies the related parent user record.
    // Data Type        : Guid?
    //
    public Guid? ParentUserId { get; set; }


    // ==========================================
    // ACKNOWLEDGEMENT
    // ==========================================

    [Display(Name = "Require parent acknowledgement")]
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
}


/* ==========================================
   ELIGIBLE PARENT OPTION
   ========================================== */

//
// Class Name       : TeacherEligibleParentViewModel
// Purpose          : Supplies teacher eligible parent view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherEligibleParentViewModel object.
//
public class TeacherEligibleParentViewModel
{
    [JsonPropertyName("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }


    [JsonPropertyName("parent_user_id")]
    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }


    [JsonPropertyName("parent_name")]
    //
    // Property Name    : ParentName
    // Purpose          : Gets or sets the parent name.
    // Data Type        : string
    //
    public string ParentName { get; set; } = "";


    [JsonPropertyName("learner_names")]
    //
    // Property Name    : LearnerNames
    // Purpose          : Gets or sets the learner names.
    // Data Type        : string
    //
    public string LearnerNames { get; set; } = "";
}
