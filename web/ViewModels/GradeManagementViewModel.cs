namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : GradeManagementViewModel
// Purpose          : Supplies grade management view model information to MVC views and actions.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GradeManagementViewModel object.
//
public class GradeManagementViewModel
{
    //
    // Property Name    : SearchTerm
    // Purpose          : Gets or sets the search term.
    // Data Type        : string
    //
    public string SearchTerm { get; set; } = "";

    //
    // Property Name    : SelectedClassId
    // Purpose          : Identifies the related selected class record.
    // Data Type        : Guid?
    //
    public Guid? SelectedClassId { get; set; }

    //
    // Property Name    : SelectedSubjectId
    // Purpose          : Identifies the related selected subject record.
    // Data Type        : Guid?
    //
    public Guid? SelectedSubjectId { get; set; }

    //
    // Property Name    : SelectedTerm
    // Purpose          : Gets or sets the selected term.
    // Data Type        : string
    //
    public string SelectedTerm { get; set; } = "";

    //
    // Property Name    : SelectedAcademicYear
    // Purpose          : Gets or sets the selected academic year.
    // Data Type        : int?
    //
    public int? SelectedAcademicYear { get; set; }

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<GradeClassOptionViewModel>
    //
    public List<GradeClassOptionViewModel> Classes { get; set; } = [];

    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<GradeSubjectOptionViewModel>
    //
    public List<GradeSubjectOptionViewModel> Subjects { get; set; } = [];

    //
    // Property Name    : Terms
    // Purpose          : Collection of terms used by this model.
    // Data Type        : List<string>
    //
    public List<string> Terms { get; set; } = [];

    //
    // Property Name    : AcademicYears
    // Purpose          : Collection of academic years used by this model.
    // Data Type        : List<int>
    //
    public List<int> AcademicYears { get; set; } = [];

    //
    // Property Name    : Records
    // Purpose          : Collection of records used by this model.
    // Data Type        : List<GradeListItemViewModel>
    //
    public List<GradeListItemViewModel> Records { get; set; } = [];


    //
    // Property Name    : TotalRecords
    // Purpose          : Total records displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRecords =>
        Records.Count;


    //
    // Property Name    : AverageScore
    // Purpose          : Gets or sets the average score.
    // Data Type        : decimal
    //
    public decimal AverageScore =>
        Records.Count == 0
            ? 0
            : Math.Round(
                Records.Average(x => x.Score),
                1);


    //
    // Property Name    : HighestScore
    // Purpose          : Gets or sets the highest score.
    // Data Type        : decimal
    //
    public decimal HighestScore =>
        Records.Count == 0
            ? 0
            : Records.Max(x => x.Score);


    //
    // Property Name    : LowestScore
    // Purpose          : Gets or sets the lowest score.
    // Data Type        : decimal
    //
    public decimal LowestScore =>
        Records.Count == 0
            ? 0
            : Records.Min(x => x.Score);
}


//
// Class Name       : GradeListItemViewModel
// Purpose          : Describes one grade list item view model row shown in a page.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GradeListItemViewModel object.
//
public class GradeListItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";

    //
    // Property Name    : LearnerName
    // Purpose          : Gets or sets the learner name.
    // Data Type        : string
    //
    public string LearnerName { get; set; } = "";

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : ClassCode
    // Purpose          : Gets or sets the class code.
    // Data Type        : string
    //
    public string ClassCode { get; set; } = "";

    //
    // Property Name    : SubjectName
    // Purpose          : Stores the subject name.
    // Data Type        : string
    //
    public string SubjectName { get; set; } = "";

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
    // Property Name    : Score
    // Purpose          : Gets or sets the score.
    // Data Type        : decimal
    //
    public decimal Score { get; set; }

    //
    // Property Name    : GradeLetter
    // Purpose          : Gets or sets the grade letter.
    // Data Type        : string
    //
    public string GradeLetter { get; set; } = "";

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
    // Property Name    : TeacherName
    // Purpose          : Gets or sets the teacher name.
    // Data Type        : string
    //
    public string TeacherName { get; set; } = "";

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string
    //
    public string Remarks { get; set; } = "";

    //
    // Property Name    : UpdatedAt
    // Purpose          : Date or time associated with updated at.
    // Data Type        : DateTime
    //
    public DateTime UpdatedAt { get; set; }
}


//
// Class Name       : GradeClassOptionViewModel
// Purpose          : Supplies a selectable grade class option view model item to a Razor view.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GradeClassOptionViewModel object.
//
public class GradeClassOptionViewModel
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


//
// Class Name       : GradeSubjectOptionViewModel
// Purpose          : Supplies a selectable grade subject option view model item to a Razor view.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GradeSubjectOptionViewModel object.
//
public class GradeSubjectOptionViewModel
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
}