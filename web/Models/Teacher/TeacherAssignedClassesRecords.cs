using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models.Teacher;

// Read-only DTOs mapped to tables that already exist in Supabase.
[Table("classes")]
//
// Class Name       : TeacherAssignedClassRecord
// Purpose          : Represents teacher assigned class data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedClassRecord object.
//
public class TeacherAssignedClassRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = string.Empty;

    [Column("grade")]
    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = string.Empty;

    [Column("room_number")]
    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string?
    //
    public string? RoomNumber { get; set; }

    [Column("class_teacher_id")]
    //
    // Property Name    : ClassTeacherId
    // Purpose          : Identifies the related class teacher record.
    // Data Type        : Guid?
    //
    public Guid? ClassTeacherId { get; set; }

    [Column("academic_year")]
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }
}

[Table("subjects")]
//
// Class Name       : TeacherAssignedSubjectRecord
// Purpose          : Represents teacher assigned subject data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedSubjectRecord object.
//
public class TeacherAssignedSubjectRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = string.Empty;

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }
}
