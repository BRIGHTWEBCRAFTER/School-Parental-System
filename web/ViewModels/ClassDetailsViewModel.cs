namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ClassDetailsViewModel
// Purpose          : Supplies class details view model information to MVC views and actions.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassDetailsViewModel object.
//
public class ClassDetailsViewModel
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
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";

    //
    // Property Name    : Section
    // Purpose          : Gets or sets the section.
    // Data Type        : string
    //
    public string Section { get; set; } = "";

    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string
    //
    public string RoomNumber { get; set; } = "";

    //
    // Property Name    : Capacity
    // Purpose          : Gets or sets the capacity.
    // Data Type        : int?
    //
    public int? Capacity { get; set; }

    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : ClassTeacherName
    // Purpose          : Gets or sets the class teacher name.
    // Data Type        : string
    //
    public string ClassTeacherName { get; set; } =
        "Not assigned";

    //
    // Property Name    : ClassTeacherEmployeeId
    // Purpose          : Identifies the related class teacher employee record.
    // Data Type        : string
    //
    public string ClassTeacherEmployeeId { get; set; } = "";

    //
    // Property Name    : EnrolledLearners
    // Purpose          : Collection of enrolled learners used by this model.
    // Data Type        : List<ClassEnrolledLearnerViewModel>
    //
    public List<ClassEnrolledLearnerViewModel>
    EnrolledLearners
    { get; set; } = new();

    //
    // Property Name    : EnrolledLearnerCount
    // Purpose          : Computed or stored enrolled learner count value.
    // Data Type        : int
    //
    public int EnrolledLearnerCount =>
        EnrolledLearners.Count;

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    //
    // Property Name    : UpdatedAt
    // Purpose          : Date or time associated with updated at.
    // Data Type        : DateTime
    //
    public DateTime UpdatedAt { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}

//
// Class Name       : ClassEnrolledLearnerViewModel
// Purpose          : Supplies class enrolled learner view model information to MVC views and actions.
// Re-use           : Passed between Learner controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassEnrolledLearnerViewModel object.
//
public class ClassEnrolledLearnerViewModel
{
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
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";

    //
    // Property Name    : Gender
    // Purpose          : Gets or sets the gender.
    // Data Type        : string
    //
    public string Gender { get; set; } = "";

    //
    // Property Name    : EnrollmentDate
    // Purpose          : Date or time associated with enrollment date.
    // Data Type        : DateTime
    //
    public DateTime EnrollmentDate { get; set; }

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}