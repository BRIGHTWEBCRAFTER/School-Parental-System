
namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherGradeDraftSaveViewModel
// Purpose          : Carries and validates submitted teacher grade draft save view model form data.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeDraftSaveViewModel object.
//
public class TeacherGradeDraftSaveViewModel
{
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    //
    // Property Name    : AssessmentName
    // Purpose          : Gets or sets the assessment name.
    // Data Type        : string
    //
    public string AssessmentName { get; set; } = "";

    //
    // Property Name    : AssessmentType
    // Purpose          : Gets or sets the assessment type.
    // Data Type        : string
    //
    public string AssessmentType { get; set; } = "";

    //
    // Property Name    : AcademicTerm
    // Purpose          : Gets or sets the academic term.
    // Data Type        : string
    //
    public string AcademicTerm { get; set; } = "";

    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    //
    // Property Name    : MaximumMarks
    // Purpose          : Gets or sets the maximum marks.
    // Data Type        : decimal
    //
    public decimal MaximumMarks { get; set; }

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<TeacherGradeDraftLearnerViewModel>
    //
    public List<TeacherGradeDraftLearnerViewModel>
        Learners
    { get; set; } = new();
}

//
// Class Name       : TeacherGradeDraftLearnerViewModel
// Purpose          : Supplies teacher grade draft learner view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeDraftLearnerViewModel object.
//
public class TeacherGradeDraftLearnerViewModel
{
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    // Null means marks have not yet been entered.
    //
    // Property Name    : MarksObtained
    // Purpose          : Gets or sets the marks obtained.
    // Data Type        : decimal?
    //
    public decimal? MarksObtained { get; set; }

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string?
    //
    public string? Remarks { get; set; }
}
