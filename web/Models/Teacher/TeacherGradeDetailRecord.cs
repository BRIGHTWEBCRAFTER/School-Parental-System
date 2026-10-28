

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models.Teacher;

[Table("grades")]
//
// Class Name       : TeacherGradeDetailRecord
// Purpose          : Represents teacher grade detail data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherGradeDetailRecord object.
//
public class TeacherGradeDetailRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("learner_id")]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    [Column("subject_id")]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    [Column("teacher_id")]
    //
    // Property Name    : TeacherId
    // Purpose          : Identifies the related teacher record.
    // Data Type        : Guid
    //
    public Guid TeacherId { get; set; }

    [Column("assessment_name")]
    //
    // Property Name    : AssessmentName
    // Purpose          : Gets or sets the assessment name.
    // Data Type        : string
    //
    public string AssessmentName { get; set; } = "";

    [Column("assessment_type")]
    //
    // Property Name    : AssessmentType
    // Purpose          : Gets or sets the assessment type.
    // Data Type        : string
    //
    public string AssessmentType { get; set; } = "";

    [Column("academic_term")]
    //
    // Property Name    : AcademicTerm
    // Purpose          : Gets or sets the academic term.
    // Data Type        : string
    //
    public string AcademicTerm { get; set; } = "";

    [Column("academic_year")]
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    [Column("score")]
    //
    // Property Name    : Score
    // Purpose          : Gets or sets the score.
    // Data Type        : decimal
    //
    public decimal Score { get; set; }

    [Column("marks_obtained")]
    //
    // Property Name    : MarksObtained
    // Purpose          : Gets or sets the marks obtained.
    // Data Type        : decimal?
    //
    public decimal? MarksObtained { get; set; }

    [Column("maximum_marks")]
    //
    // Property Name    : MaximumMarks
    // Purpose          : Gets or sets the maximum marks.
    // Data Type        : decimal?
    //
    public decimal? MaximumMarks { get; set; }

    [Column("grade_letter")]
    //
    // Property Name    : GradeLetter
    // Purpose          : Gets or sets the grade letter.
    // Data Type        : string?
    //
    public string? GradeLetter { get; set; }

    [Column("remarks")]
    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string?
    //
    public string? Remarks { get; set; }

    [Column("publication_status")]
    //
    // Property Name    : PublicationStatus
    // Purpose          : Gets or sets the publication status.
    // Data Type        : string
    //
    public string PublicationStatus { get; set; } = "Draft";

    [Column("published_at")]
    //
    // Property Name    : PublishedAt
    // Purpose          : Date or time associated with published at.
    // Data Type        : DateTime?
    //
    public DateTime? PublishedAt { get; set; }
}
