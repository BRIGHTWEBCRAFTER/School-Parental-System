
namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherAttendanceSaveViewModel
// Purpose          : Carries and validates submitted teacher attendance save view model form data.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAttendanceSaveViewModel object.
//
public class TeacherAttendanceSaveViewModel
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
    // Data Type        : Guid?
    //
    public Guid? SubjectId { get; set; }


    //
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; }

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<TeacherAttendanceSaveLearnerViewModel>
    //
    public List<TeacherAttendanceSaveLearnerViewModel>
        Learners
    { get; set; } = new();
}

//
// Class Name       : TeacherAttendanceSaveLearnerViewModel
// Purpose          : Carries and validates submitted teacher attendance save learner view model form data.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAttendanceSaveLearnerViewModel object.
//
public class TeacherAttendanceSaveLearnerViewModel
{
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string?
    //
    public string? Status { get; set; }

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string?
    //
    public string? Remarks { get; set; }
}
