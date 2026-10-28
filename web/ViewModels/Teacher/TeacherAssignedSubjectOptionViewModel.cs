
namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherAssignedSubjectOptionViewModel
// Purpose          : Supplies a selectable teacher assigned subject option view model item to a Razor view.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherAssignedSubjectOptionViewModel object.
//
public class TeacherAssignedSubjectOptionViewModel
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
    public string Name { get; set; } = string.Empty;
}
