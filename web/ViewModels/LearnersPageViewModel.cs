namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : LearnersPageViewModel
// Purpose          : Combines data displayed on the learners page view model page.
// Re-use           : Passed between Learner controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : LearnersPageViewModel object.
//
public class LearnersPageViewModel
{
    //
    // Property Name    : TotalLearners
    // Purpose          : Total learners displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalLearners { get; set; }
    //
    // Property Name    : ActiveLearners
    // Purpose          : Gets or sets the active learners.
    // Data Type        : int
    //
    public int ActiveLearners { get; set; }
    //
    // Property Name    : InactiveLearners
    // Purpose          : Gets or sets the inactive learners.
    // Data Type        : int
    //
    public int InactiveLearners { get; set; }

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<LearnerListItemViewModel>
    //
    public List<LearnerListItemViewModel> Learners { get; set; } = [];
}

//
// Class Name       : LearnerListItemViewModel
// Purpose          : Describes one learner list item view model row shown in a page.
// Re-use           : Passed between Learner controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : LearnerListItemViewModel object.
//
public class LearnerListItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : string
    //
    public string Id { get; set; } = "";
    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";
    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";
    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";
    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";
    //
    // Property Name    : GuardianName
    // Purpose          : Gets or sets the guardian name.
    // Data Type        : string
    //
    public string GuardianName { get; set; } = "";
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "Active";
}