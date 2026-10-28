namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : AttendanceManagementViewModel
// Purpose          : Supplies attendance management view model information to MVC views and actions.
// Re-use           : Passed between Attendance controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceManagementViewModel object.
//
public class AttendanceManagementViewModel
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
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; } =
        DateTime.Today;


    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<AttendanceClassOptionViewModel>
    //
    public List<AttendanceClassOptionViewModel>
        Classes
    { get; set; } = [];

    //
    // Property Name    : Subjects
    // Purpose          : Collection of subjects used by this model.
    // Data Type        : List<AttendanceSubjectOptionViewModel>
    //
    public List<AttendanceSubjectOptionViewModel>
        Subjects
    { get; set; } = [];

    //
    // Property Name    : Records
    // Purpose          : Collection of records used by this model.
    // Data Type        : List<AttendanceListItemViewModel>
    //
    public List<AttendanceListItemViewModel>
        Records
    { get; set; } = [];


    //
    // Property Name    : TotalRecords
    // Purpose          : Total records displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalRecords =>
        Records.Count;

    //
    // Property Name    : PresentCount
    // Purpose          : Computed or stored present count value.
    // Data Type        : int
    //
    public int PresentCount =>
        Records.Count(x =>
            x.Status == "Present");

    //
    // Property Name    : AbsentCount
    // Purpose          : Computed or stored absent count value.
    // Data Type        : int
    //
    public int AbsentCount =>
        Records.Count(x =>
            x.Status == "Absent");

    //
    // Property Name    : LateCount
    // Purpose          : Computed or stored late count value.
    // Data Type        : int
    //
    public int LateCount =>
        Records.Count(x =>
            x.Status == "Late");

    //
    // Property Name    : ExcusedCount
    // Purpose          : Computed or stored excused count value.
    // Data Type        : int
    //
    public int ExcusedCount =>
        Records.Count(x =>
            x.Status == "Excused");
}


//
// Class Name       : AttendanceListItemViewModel
// Purpose          : Describes one attendance list item view model row shown in a page.
// Re-use           : Passed between Attendance controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceListItemViewModel object.
//
public class AttendanceListItemViewModel
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
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    //
    // Property Name    : CheckInTime
    // Purpose          : Date or time associated with check in time.
    // Data Type        : TimeSpan?
    //
    public TimeSpan? CheckInTime { get; set; }

    //
    // Property Name    : Remarks
    // Purpose          : Gets or sets the remarks.
    // Data Type        : string
    //
    public string Remarks { get; set; } = "";

    //
    // Property Name    : MarkedByName
    // Purpose          : Gets or sets the marked by name.
    // Data Type        : string
    //
    public string MarkedByName { get; set; } = "";

    //
    // Property Name    : UpdatedAt
    // Purpose          : Date or time associated with updated at.
    // Data Type        : DateTime
    //
    public DateTime UpdatedAt { get; set; }
}


//
// Class Name       : AttendanceClassOptionViewModel
// Purpose          : Supplies a selectable attendance class option view model item to a Razor view.
// Re-use           : Passed between Attendance controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceClassOptionViewModel object.
//
public class AttendanceClassOptionViewModel
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
// Class Name       : AttendanceSubjectOptionViewModel
// Purpose          : Supplies a selectable attendance subject option view model item to a Razor view.
// Re-use           : Passed between Attendance controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AttendanceSubjectOptionViewModel object.
//
public class AttendanceSubjectOptionViewModel
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