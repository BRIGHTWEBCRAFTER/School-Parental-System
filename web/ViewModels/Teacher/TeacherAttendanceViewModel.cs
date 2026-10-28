
namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherAttendanceViewModel
// Purpose          : Supplies teacher attendance view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAttendanceViewModel object.
//
public class TeacherAttendanceViewModel
{
    //
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; }
        = DateTime.Today;

    //
    // Property Name    : SelectedClassId
    // Purpose          : Identifies the related selected class record.
    // Data Type        : Guid?
    //
    public Guid? SelectedClassId { get; set; }

    // Null means Daily Attendance.
    // A valid subject ID means Subject Attendance.
    //
    // Property Name    : SelectedSubjectId
    // Purpose          : Identifies the related selected subject record.
    // Data Type        : Guid?
    //
    public Guid? SelectedSubjectId { get; set; }

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
    // Property Name    : IsClassTeacher
    // Purpose          : Indicates whether class teacher applies.
    // Data Type        : bool
    //
    public bool IsClassTeacher { get; set; }

    // True only after the requested register
    // has passed the Teacher assignment checks.
    //
    // Property Name    : CanLoadRegister
    // Purpose          : Indicates permission to load register.
    // Data Type        : bool
    //
    public bool CanLoadRegister { get; set; }

    //
    // Property Name    : InvalidSubjectSelection
    // Purpose          : Gets or sets the invalid subject selection.
    // Data Type        : bool
    //
    public bool InvalidSubjectSelection { get; set; }

    //
    // Property Name    : IsSubjectAttendance
    // Purpose          : Indicates whether subject attendance applies.
    // Data Type        : bool
    //
    public bool IsSubjectAttendance =>
        SelectedSubjectId.HasValue;

    //
    // Property Name    : SelectedSubjectName
    // Purpose          : Gets or sets the selected subject name.
    // Data Type        : string
    //
    public string SelectedSubjectName =>
        SubjectOptions
            .FirstOrDefault(s =>
                s.Id == SelectedSubjectId)
            ?.Name ?? "";

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<TeacherAttendanceClassOptionViewModel>
    //
    public List<TeacherAttendanceClassOptionViewModel>
        Classes
    { get; set; } = new();

    // Authorized subjects for the selected class.
    //
    // Property Name    : SubjectOptions
    // Purpose          : Collection of subject options used by this model.
    // Data Type        : List<TeacherAssignedSubjectOptionViewModel>
    //
    public List<TeacherAssignedSubjectOptionViewModel>
        SubjectOptions
    { get; set; } = new();

    //
    // Property Name    : Learners
    // Purpose          : Collection of learners used by this model.
    // Data Type        : List<TeacherAttendanceLearnerViewModel>
    //
    public List<TeacherAttendanceLearnerViewModel>
        Learners
    { get; set; } = new();

    //
    // Property Name    : TotalLearners
    // Purpose          : Total learners displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalLearners => Learners.Count;

    //
    // Property Name    : PresentCount
    // Purpose          : Computed or stored present count value.
    // Data Type        : int
    //
    public int PresentCount =>
        Learners.Count(x => x.Status == "Present");

    //
    // Property Name    : AbsentCount
    // Purpose          : Computed or stored absent count value.
    // Data Type        : int
    //
    public int AbsentCount =>
        Learners.Count(x => x.Status == "Absent");

    //
    // Property Name    : LateCount
    // Purpose          : Computed or stored late count value.
    // Data Type        : int
    //
    public int LateCount =>
        Learners.Count(x => x.Status == "Late");

    //
    // Property Name    : ExcusedCount
    // Purpose          : Computed or stored excused count value.
    // Data Type        : int
    //
    public int ExcusedCount =>
        Learners.Count(x => x.Status == "Excused");

    //
    // Property Name    : NotMarkedCount
    // Purpose          : Computed or stored not marked count value.
    // Data Type        : int
    //
    public int NotMarkedCount =>
        Learners.Count(x => x.Status == "Not marked");
}

//
// Class Name       : TeacherAttendanceClassOptionViewModel
// Purpose          : Supplies a selectable teacher attendance class option view model item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAttendanceClassOptionViewModel object.
//
public class TeacherAttendanceClassOptionViewModel
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
    // Property Name    : IsClassTeacher
    // Purpose          : Indicates whether class teacher applies.
    // Data Type        : bool
    //
    public bool IsClassTeacher { get; set; }
}

//
// Class Name       : TeacherAttendanceLearnerViewModel
// Purpose          : Supplies teacher attendance learner view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAttendanceLearnerViewModel object.
//
public class TeacherAttendanceLearnerViewModel
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
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "Not marked";

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string
    //
    public string Remarks { get; set; } = "";

    //
    // Property Name    : AttendanceRecordId
    // Purpose          : Identifies the related attendance record record.
    // Data Type        : Guid?
    //
    public Guid? AttendanceRecordId { get; set; }
}
