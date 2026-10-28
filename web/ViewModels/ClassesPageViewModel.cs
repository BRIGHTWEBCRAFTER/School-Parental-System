namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : ClassesPageViewModel
// Purpose          : Combines data displayed on the classes page view model page.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassesPageViewModel object.
//
public class ClassesPageViewModel
{
    //
    // Property Name    : TotalClasses
    // Purpose          : Total classes displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalClasses { get; set; }

    //
    // Property Name    : ActiveClasses
    // Purpose          : Gets or sets the active classes.
    // Data Type        : int
    //
    public int ActiveClasses { get; set; }

    //
    // Property Name    : InactiveClasses
    // Purpose          : Gets or sets the inactive classes.
    // Data Type        : int
    //
    public int InactiveClasses { get; set; }

    //
    // Property Name    : Classes
    // Purpose          : Collection of classes used by this model.
    // Data Type        : List<ClassListItemViewModel>
    //
    public List<ClassListItemViewModel>
        Classes
    { get; set; } = new();
}


//
// Class Name       : ClassListItemViewModel
// Purpose          : Describes one class list item view model row shown in a page.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ClassListItemViewModel object.
//
public class ClassListItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";

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

    //
    // Property Name    : Section
    // Purpose          : Gets or sets the section.
    // Data Type        : string
    //
    public string Section { get; set; } = "";

    //
    // Property Name    : ClassTeacherName
    // Purpose          : Gets or sets the class teacher name.
    // Data Type        : string
    //
    public string ClassTeacherName { get; set; } =
        "Not assigned";

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
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status =>
        IsActive
            ? "Active"
            : "Inactive";
}