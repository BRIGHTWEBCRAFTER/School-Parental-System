
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

// ==========================================
// TERM REPORT PAGE
// ==========================================

//
// Class Name       : SchoolAdminTermReportViewModel
// Purpose          : Supplies school admin term report view model information to MVC views and actions.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminTermReportViewModel object.
//
public class SchoolAdminTermReportViewModel
{
    [Range(2000, 2100)]
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }
        = DateTime.Today.Year;

    [Required]
    [RegularExpression(
        @"^Term [1-4]$",
        ErrorMessage = "Select a valid academic term.")]
    //
    // Property Name    : Term
    // Purpose          : Stores the term.
    // Data Type        : string
    //
    public string Term { get; set; } = "Term 1";

    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid?
    //
    public Guid? ClassId { get; set; }

    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid?
    //
    public Guid? LearnerId { get; set; }

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TermReportClassOption>
    //
    public List<TermReportClassOption>
        Classes
    { get; set; } = new();

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<TermReportLearnerOption>
    //
    public List<TermReportLearnerOption>
        Learners
    { get; set; } = new();

    //
    // Property Name    : Report
    // Purpose          : Gets or sets the report.
    // Data Type        : TermReportDocumentViewModel?
    //
    public TermReportDocumentViewModel?
        Report
    { get; set; }
}


// ==========================================
// CLASS SELECTOR
// ==========================================

//
// Class Name       : TermReportClassOption
// Purpose          : Supplies a selectable term report class option item to a Razor view.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TermReportClassOption object.
//
public class TermReportClassOption
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
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";
}


// ==========================================
// LEARNER SELECTOR
// ==========================================

//
// Class Name       : TermReportLearnerOption
// Purpose          : Supplies a selectable term report learner option item to a Razor view.
// Re-use           : Passed between Learner controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TermReportLearnerOption object.
//
public class TermReportLearnerOption
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

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
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }
}


// ==========================================
// PDF REPORT CONTENT
// ==========================================

//
// Class Name       : TermReportDocumentViewModel
// Purpose          : Supplies term report document view model information to MVC views and actions.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TermReportDocumentViewModel object.
//
public class TermReportDocumentViewModel
{
    // SCHOOL INFORMATION

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : SchoolEmail
    // Purpose          : Gets or sets the school email.
    // Data Type        : string?
    //
    public string? SchoolEmail { get; set; }


    // LEARNER INFORMATION

    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : LearnerName
    // Purpose          : Gets or sets the learner name.
    // Data Type        : string
    //
    public string LearnerName { get; set; } = "";

    //
    // Property Name    : AdmissionNumber
    // Purpose          : Gets or sets the admission number.
    // Data Type        : string
    //
    public string AdmissionNumber { get; set; } = "";

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


    // ACADEMIC PERIOD

    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    //
    // Property Name    : Term
    // Purpose          : Stores the term.
    // Data Type        : string
    //
    public string Term { get; set; } = "";

    //
    // Property Name    : GeneratedAt
    // Purpose          : Date or time associated with generated at.
    // Data Type        : DateTime
    //
    public DateTime GeneratedAt { get; set; }
        = DateTime.UtcNow;


    // SUBJECT RESULTS

    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<TermReportSubjectResult>
    //
    public List<TermReportSubjectResult>
        Subjects
    { get; set; } = new();


    // OVERALL RESULT

    //
    // Property Name    : OverallAverage
    // Purpose          : Computed or stored overall average value.
    // Data Type        : decimal?
    //
    public decimal? OverallAverage { get; set; }

    //
    // Property Name    : HasResults
    // Purpose          : Indicates whether results is available.
    // Data Type        : bool
    //
    public bool HasResults => Subjects.Count > 0;


    // AVAILABLE GUARDIANS

    //
    // Property Name    : Guardians
    // Purpose          : Collection of guardians used by this model.
    // Data Type        : List<TermReportGuardianRecipient>
    //
    public List<TermReportGuardianRecipient>
        Guardians
    { get; set; } = new();
}


// ==========================================
// SUBJECT TERM RESULT
// ==========================================

//
// Class Name       : TermReportSubjectResult
// Purpose          : Carries the result of term report subject result to a view.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TermReportSubjectResult object.
//
public class TermReportSubjectResult
{
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    //
    // Property Name    : SubjectName
    // Purpose          : Stores the subject name.
    // Data Type        : string
    //
    public string SubjectName { get; set; } = "";

    //
    // Property Name    : AssessmentCount
    // Purpose          : Computed or stored assessment count value.
    // Data Type        : int
    //
    public int AssessmentCount { get; set; }

    //
    // Property Name    : AveragePercentage
    // Purpose          : Computed or stored average percentage value.
    // Data Type        : decimal
    //
    public decimal AveragePercentage { get; set; }

    //
    // Property Name    : TeacherRemark
    // Purpose          : Gets or sets the teacher remark.
    // Data Type        : string?
    //
    public string? TeacherRemark { get; set; }
}


// ==========================================
// REGISTERED GUARDIAN
// ==========================================

//
// Class Name       : TermReportGuardianRecipient
// Purpose          : Supplies term report guardian recipient information to MVC views and actions.
// Re-use           : Passed between Parent and guardian controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TermReportGuardianRecipient object.
//
public class TermReportGuardianRecipient
{
    //
    // Property Name    : ParentId
    // Purpose          : Identifies the related parent record.
    // Data Type        : Guid
    //
    public Guid ParentId { get; set; }

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : IsPrimaryGuardian
    // Purpose          : Indicates whether primary guardian applies.
    // Data Type        : bool
    //
    public bool IsPrimaryGuardian { get; set; }
}


// ==========================================
// SEND REPORT REQUEST
// ==========================================

//
// Class Name       : SendTermReportInput
// Purpose          : Carries and validates submitted send term report input form data.
// Re-use           : Passed between Grades and reports controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SendTermReportInput object.
//
public class SendTermReportInput
{
    [Required]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Required]
    //
    // Property Name    : ParentId
    // Purpose          : Identifies the related parent record.
    // Data Type        : Guid
    //
    public Guid ParentId { get; set; }

    [Range(2000, 2100)]
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    [Required]
    [RegularExpression(@"^Term [1-4]$")]
    //
    // Property Name    : Term
    // Purpose          : Stores the term.
    // Data Type        : string
    //
    public string Term { get; set; } = "";
}
