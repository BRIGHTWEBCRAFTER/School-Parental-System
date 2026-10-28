
namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherGradesViewModel
// Purpose          : Supplies teacher grades view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradesViewModel object.
//
public class TeacherGradesViewModel
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
    //
    // Property Name    : InvalidSelection
    // Purpose          : Gets or sets the invalid selection.
    // Data Type        : bool
    //
    public bool InvalidSelection { get; set; }

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
    // Property Name    : SearchTerm
    // Purpose          : Gets or sets the search term.
    // Data Type        : string
    //
    public string SearchTerm { get; set; } = "";

    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TeacherGradeClassOptionViewModel>
    //
    public List<TeacherGradeClassOptionViewModel>
        Classes
    { get; set; } = new();

    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<TeacherGradeSubjectOptionViewModel>
    //
    public List<TeacherGradeSubjectOptionViewModel>
        Subjects
    { get; set; } = new();

    //
    // Property Name    : Records
    // Purpose          : Collection of records used by this model.
    // Data Type        : List<TeacherGradeRecordViewModel>
    //
    public List<TeacherGradeRecordViewModel>
        Records
    { get; set; } = new();

    //
    // Property Name    : Terms
    // Purpose          : Collection of terms used by this model.
    // Data Type        : List<string>
    //
    public List<string> Terms { get; set; } =
        new()
        {
            "Term 1",
            "Term 2",
            "Term 3",
            "Term 4"
        };

    // Statistics based on the filtered records.
    //
    // Property Name    : TotalGradeRecords
    // Purpose          : Total grade records displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalGradeRecords => Records.Count;

    //
    // Property Name    : AssessmentCount
    // Purpose          : Computed or stored assessment count value.
    // Data Type        : int
    //
    public int AssessmentCount =>
        Records
            .Select(x => new
            {
                x.SubjectId,
                x.AssessmentName,
                x.AcademicTerm,
                x.AcademicYear
            })
            .Distinct()
            .Count();

    //
    // Property Name    : LearnersGraded
    // Purpose          : Gets or sets the learners graded.
    // Data Type        : int
    //
    public int LearnersGraded =>
        Records.Select(x => x.LearnerId)
            .Distinct()
            .Count();

    //
    // Property Name    : AssignedSubjectCount
    // Purpose          : Computed or stored assigned subject count value.
    // Data Type        : int
    //
    public int AssignedSubjectCount => Subjects.Count;


    /* ==========================================
       GRADE ENTRY REGISTER
       ========================================== */

    //
    // Property Name    : EntryLearners
    // Purpose          : Collection of entry learners used by this model.
    // Data Type        : List<TeacherGradeEntryLearnerViewModel>
    //
    public List<TeacherGradeEntryLearnerViewModel>
        EntryLearners
    { get; set; } = new();

    //
    // Property Name    : CanRecordGrades
    // Purpose          : Indicates permission to record grades.
    // Data Type        : bool
    //
    public bool CanRecordGrades =>
        HasTeacherProfile &&
        !DataLoadFailed &&
        !InvalidSelection &&
        SelectedClassId.HasValue &&
        SelectedSubjectId.HasValue;


    //
    // Property Name    : EditingAssessmentName
    // Purpose          : Gets or sets the editing assessment name.
    // Data Type        : string
    //
    public string EditingAssessmentName { get; set; } = "";

    //
    // Property Name    : EditingAssessmentType
    // Purpose          : Gets or sets the editing assessment type.
    // Data Type        : string
    //
    public string EditingAssessmentType { get; set; } = "";

    //
    // Property Name    : EditingTerm
    // Purpose          : Gets or sets the editing term.
    // Data Type        : string
    //
    public string EditingTerm { get; set; } = "";

    //
    // Property Name    : EditingMaximumMarks
    // Purpose          : Gets or sets the editing maximum marks.
    // Data Type        : decimal?
    //
    public decimal? EditingMaximumMarks { get; set; }

    //
    // Property Name    : IsEditingDraft
    // Purpose          : Indicates whether editing draft applies.
    // Data Type        : bool
    //
    public bool IsEditingDraft =>
        !string.IsNullOrWhiteSpace(EditingAssessmentName);


}

//
// Class Name       : TeacherGradeClassOptionViewModel
// Purpose          : Supplies a selectable teacher grade class option view model item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeClassOptionViewModel object.
//
public class TeacherGradeClassOptionViewModel
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

//
// Class Name       : TeacherGradeSubjectOptionViewModel
// Purpose          : Supplies a selectable teacher grade subject option view model item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeSubjectOptionViewModel object.
//
public class TeacherGradeSubjectOptionViewModel
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

//
// Class Name       : TeacherGradeRecordViewModel
// Purpose          : Supplies teacher grade record view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeRecordViewModel object.
//
public class TeacherGradeRecordViewModel
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
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

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
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string
    //
    public string Remarks { get; set; } = "";


    //
    // Property Name    : PublicationStatus
    // Purpose          : Gets or sets the publication status.
    // Data Type        : string
    //
    public string PublicationStatus { get; set; } = "Draft";

    //
    // Property Name    : MarksObtained
    // Purpose          : Gets or sets the marks obtained.
    // Data Type        : decimal?
    //
    public decimal? MarksObtained { get; set; }

    //
    // Property Name    : MaximumMarks
    // Purpose          : Gets or sets the maximum marks.
    // Data Type        : decimal?
    //
    public decimal? MaximumMarks { get; set; }

    //
    // Property Name    : CanEditDraft
    // Purpose          : Indicates permission to edit draft.
    // Data Type        : bool
    //
    public bool CanEditDraft { get; set; }

}


/* ==========================================
   LEARNER GRADE ENTRY
   ========================================== */

//
// Class Name       : TeacherGradeEntryLearnerViewModel
// Purpose          : Supplies teacher grade entry learner view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeEntryLearnerViewModel object.
//
public class TeacherGradeEntryLearnerViewModel
{
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";


    //
    // Property Name    : MarksObtained
    // Purpose          : Gets or sets the marks obtained.
    // Data Type        : decimal?
    //
    public decimal? MarksObtained { get; set; }

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string
    //
    public string Remarks { get; set; } = "";

}


//
// Class Name       : TeacherGradeDashboardStatsViewModel
// Purpose          : Combines data displayed on the teacher grade dashboard stats view model page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeDashboardStatsViewModel object.
//
public class TeacherGradeDashboardStatsViewModel
{
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    //
    // Property Name    : TotalEntries
    // Purpose          : Total entries displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalEntries { get; set; }

    //
    // Property Name    : PublishedEntries
    // Purpose          : Gets or sets the published entries.
    // Data Type        : int
    //
    public int PublishedEntries { get; set; }

    //
    // Property Name    : DraftEntries
    // Purpose          : Gets or sets the draft entries.
    // Data Type        : int
    //
    public int DraftEntries { get; set; }

    //
    // Property Name    : AssessmentCount
    // Purpose          : Computed or stored assessment count value.
    // Data Type        : int
    //
    public int AssessmentCount { get; set; }

    //
    // Property Name    : AverageScore
    // Purpose          : Gets or sets the average score.
    // Data Type        : decimal?
    //
    public decimal? AverageScore { get; set; }
}

