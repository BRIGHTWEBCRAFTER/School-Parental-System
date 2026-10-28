
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchoolParentalSystem.Web.Models;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : SchoolAdminAcademicsViewModel
// Purpose          : Supplies school admin academics view model information to MVC views and actions.
// Re-use           : Passed between School Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolAdminAcademicsViewModel object.
//
public class SchoolAdminAcademicsViewModel
{
    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<SubjectRecord>
    //
    public List<SubjectRecord> Subjects { get; set; } = new();

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<SchoolClassRecord>
    //
    public List<SchoolClassRecord> Classes { get; set; } = new();

    //
    // Property Name    : Teachers
    // Purpose          : Collection of teachers used by this model.
    // Data Type        : List<AcademicTeacherOption>
    //
    public List<AcademicTeacherOption> Teachers { get; set; } = new();

    //
    // Property Name    : Lessons
    // Purpose          : Collection of lessons used by this model.
    // Data Type        : List<AcademicTimetableItem>
    //
    public List<AcademicTimetableItem> Lessons { get; set; } = new();

    //
    // Property Name    : NewSubject
    // Purpose          : Gets or sets the new subject.
    // Data Type        : AddAcademicSubjectInput
    //
    public AddAcademicSubjectInput NewSubject { get; set; } = new();

    //
    // Property Name    : NewLesson
    // Purpose          : Gets or sets the new lesson.
    // Data Type        : AddAcademicLessonInput
    //
    public AddAcademicLessonInput NewLesson { get; set; } = new();
}


//
// Class Name       : AddAcademicSubjectInput
// Purpose          : Holds input and validation data for creating academic subject input.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AddAcademicSubjectInput object.
//
public class AddAcademicSubjectInput
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";

    [Required]
    [StringLength(20)]
    [RegularExpression(
        @"^[A-Za-z0-9_-]+$",
        ErrorMessage = "Use letters, numbers, hyphens or underscores.")]
    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

    [StringLength(255)]
    //
    // Property Name    : Description
    // Purpose          : Stores the description.
    // Data Type        : string?
    //
    public string? Description { get; set; }
}


//
// Class Name       : AddAcademicLessonInput
// Purpose          : Holds input and validation data for creating academic lesson input.
// Re-use           : Passed between School Parental System controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AddAcademicLessonInput object.
//
public class AddAcademicLessonInput
{
    [Required]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    [Required]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid
    //
    public Guid SubjectId { get; set; }

    //
    // Property Name    : TeacherId
    // Purpose          : Identifies the related teacher record.
    // Data Type        : Guid?
    //
    public Guid? TeacherId { get; set; }

    [Range(1, 5)]
    //
    // Property Name    : DayOfWeek
    // Purpose          : Gets or sets the day of week.
    // Data Type        : int
    //
    public int DayOfWeek { get; set; } = 1;

    [Required]
    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : string
    //
    public string StartTime { get; set; } = "";

    [Required]
    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : string
    //
    public string EndTime { get; set; } = "";

    [StringLength(50)]
    //
    // Property Name    : Room
    // Purpose          : Gets or sets the room.
    // Data Type        : string?
    //
    public string? Room { get; set; }
}


//
// Class Name       : AcademicTeacherOption
// Purpose          : Supplies a selectable academic teacher option item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AcademicTeacherOption object.
//
public class AcademicTeacherOption
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
// Class Name       : AcademicTimetableItem
// Purpose          : Supplies academic timetable item information to MVC views and actions.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AcademicTimetableItem object.
//
public class AcademicTimetableItem
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

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
    // Property Name    : SubjectName
    // Purpose          : Stores the subject name.
    // Data Type        : string
    //
    public string SubjectName { get; set; } = "";

    //
    // Property Name    : TeacherName
    // Purpose          : Gets or sets the teacher name.
    // Data Type        : string
    //
    public string TeacherName { get; set; } = "";

    //
    // Property Name    : TeacherId
    // Purpose          : Identifies the assigned lesson teacher.
    // Data Type        : Guid?
    //
    public Guid? TeacherId { get; set; }

    //
    // Property Name    : DayOfWeek
    // Purpose          : Gets or sets the day of week.
    // Data Type        : int
    //
    public int DayOfWeek { get; set; }

    //
    // Property Name    : DayName
    // Purpose          : Gets or sets the day name.
    // Data Type        : string
    //
    public string DayName =>
        DayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            _ => "Unknown"
        };

    //
    // Property Name    : StartTime
    // Purpose          : Date or time associated with start time.
    // Data Type        : string
    //
    public string StartTime { get; set; } = "";

    //
    // Property Name    : EndTime
    // Purpose          : Date or time associated with end time.
    // Data Type        : string
    //
    public string EndTime { get; set; } = "";

    //
    // Property Name    : Room
    // Purpose          : Gets or sets the room.
    // Data Type        : string
    //
    public string Room { get; set; } = "";
}
