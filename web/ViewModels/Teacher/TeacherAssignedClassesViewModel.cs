
using System;
using System.Collections.Generic;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherAssignedClassesViewModel
// Purpose          : Supplies teacher assigned classes view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedClassesViewModel object.
//
public class TeacherAssignedClassesViewModel
{
    // ======================================
    // TEACHER PROFILE AND ACADEMIC YEAR
    // ======================================

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
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; } =
        DateTime.Today.Year;

    // ======================================
    // ASSIGNED CLASS STATISTICS
    // ======================================

    //
    // Property Name    : HomeroomClassCount
    // Purpose          : Computed or stored homeroom class count value.
    // Data Type        : int
    //
    public int HomeroomClassCount { get; set; }

    //
    // Property Name    : ClassesTaughtCount
    // Purpose          : Computed or stored classes taught count value.
    // Data Type        : int
    //
    public int ClassesTaughtCount { get; set; }

    //
    // Property Name    : SubjectCount
    // Purpose          : Computed or stored subject count value.
    // Data Type        : int
    //
    public int SubjectCount { get; set; }

    // ======================================
    // ASSIGNED CLASS DATA
    // ======================================

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TeacherAssignedClassItemViewModel>
    //
    public List<TeacherAssignedClassItemViewModel>
        Classes
    { get; set; } = new();

    //
    // Property Name    : AssignedClass
    // Purpose          : Gets or sets the assigned class.
    // Data Type        : TeacherAssignedClassSummaryViewModel?
    //
    public TeacherAssignedClassSummaryViewModel?
        AssignedClass
    { get; set; }

    //
    // Property Name    : ClassesITeach
    // Purpose          : Collection of classes i teach used by this model.
    // Data Type        : List<TeacherClassListItemViewModel>
    //
    public List<TeacherClassListItemViewModel>
        ClassesITeach
    { get; set; } = new();

    // ======================================
    // SEARCH RESULTS
    // ======================================

    //
    // Property Name    : SearchQuery
    // Purpose          : Gets or sets the search query.
    // Data Type        : string
    //
    public string SearchQuery { get; set; } = "";

    //
    // Property Name    : LearnerMatches
    // Purpose          : Collection of learner matches used by this model.
    // Data Type        : List<TeacherLearnerSearchResultViewModel>
    //
    public List<TeacherLearnerSearchResultViewModel>
        LearnerMatches
    { get; set; } = new();

    // ======================================
    // TERM FILTER
    // ======================================

    //
    // Property Name    : SelectedTerm
    // Purpose          : Gets or sets the selected term.
    // Data Type        : string
    //
    public string SelectedTerm { get; set; } =
        "All Terms";

    //
    // Property Name    : Terms
    // Purpose          : Collection of terms used by this model.
    // Data Type        : List<string>
    //
    public List<string> Terms { get; set; } =
        new()
        {
            "All Terms",
            "Term 1",
            "Term 2",
            "Term 3",
            "Term 4"
        };
}

// ==========================================
// CLASS DATABASE DISPLAY
// ==========================================

//
// Class Name       : TeacherAssignedClassItemViewModel
// Purpose          : Describes one teacher assigned class item view model row shown in a page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedClassItemViewModel object.
//
public class TeacherAssignedClassItemViewModel
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
    // Property Name    : SubjectOptions
    // Purpose          : Collection of subject options used by this model.
    // Data Type        : List<TeacherAssignedSubjectOptionViewModel>
    //
    public List<TeacherAssignedSubjectOptionViewModel>
        SubjectOptions
    { get; set; } = new();


    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";

    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string?
    //
    public string? RoomNumber { get; set; }

    //
    // Property Name    : IsClassTeacher
    // Purpose          : Indicates whether class teacher applies.
    // Data Type        : bool
    //
    public bool IsClassTeacher { get; set; }

    //
    // Property Name    : TeachesSubjects
    // Purpose          : Gets or sets the teaches subjects.
    // Data Type        : bool
    //
    public bool TeachesSubjects { get; set; }

    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<string>
    //
    public List<string> Subjects { get; set; } =
        new();
}

// ==========================================
// ASSIGNED CLASS SUMMARY
// ==========================================

//
// Class Name       : TeacherAssignedClassSummaryViewModel
// Purpose          : Supplies teacher assigned class summary view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedClassSummaryViewModel object.
//
public class TeacherAssignedClassSummaryViewModel
{
    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int?
    //
    public int? LearnerCount { get; set; }

    //
    // Property Name    : ClassTeacherName
    // Purpose          : Gets or sets the class teacher name.
    // Data Type        : string
    //
    public string ClassTeacherName { get; set; } = "";

    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string
    //
    public string Description { get; set; } = "";
}

// ==========================================
// CLASSES I TEACH
// ==========================================

//
// Class Name       : TeacherClassListItemViewModel
// Purpose          : Describes one teacher class list item view model row shown in a page.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherClassListItemViewModel object.
//
public class TeacherClassListItemViewModel
{
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    //
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";

    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string
    //
    public string RoomNumber { get; set; } = "";

    //
    // Property Name    : SubjectName
    // Purpose          : Stores the subject name.
    // Data Type        : string
    //
    public string SubjectName { get; set; } = "";

    //
    // Property Name    : StudentCount
    // Purpose          : Computed or stored student count value.
    // Data Type        : int?
    //
    public int? StudentCount { get; set; }

    //
    // Property Name    : Schedule
    // Purpose          : Gets or sets the schedule.
    // Data Type        : string
    //
    public string Schedule { get; set; } =
        "Not available";
}

// ==========================================
// LEARNER SEARCH RESULT
// ==========================================

//
// Class Name       : TeacherLearnerSearchResultViewModel
// Purpose          : Carries the result of teacher learner search result view model to a view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherLearnerSearchResultViewModel object.
//
public class TeacherLearnerSearchResultViewModel
{
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

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
    // Property Name    : ClassName
    // Purpose          : Stores the class name.
    // Data Type        : string
    //
    public string ClassName { get; set; } = "";
}
