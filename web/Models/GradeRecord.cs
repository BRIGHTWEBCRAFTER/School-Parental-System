using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("grades")]
//
// Class Name       : GradeRecord
// Purpose          : Represents grade data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : GradeRecord object.
//
public class GradeRecord : BaseModel
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

    [Column("subject_id")]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

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

    [Column("score")]
    //
    // Property Name    : Score
    // Purpose          : Gets or sets the score.
    // Data Type        : decimal
    //
    public decimal Score { get; set; }

    [Column("grade_letter")]
    //
    // Property Name    : GradeLetter
    // Purpose          : Gets or sets the grade letter.
    // Data Type        : string?
    //
    public string? GradeLetter { get; set; }

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

    [Column("remarks")]
    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string?
    //
    public string? Remarks { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    //
    // Property Name    : UpdatedAt
    // Purpose          : Date or time associated with updated at.
    // Data Type        : DateTime
    //
    public DateTime UpdatedAt { get; set; }
}