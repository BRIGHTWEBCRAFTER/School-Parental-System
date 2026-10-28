using System.ComponentModel.DataAnnotations;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : EditClassViewModel
// Purpose          : Holds editable fields and validation for class view model.
// Re-use           : Passed between Classes and timetables controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : EditClassViewModel object.
//
public class EditClassViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }


    [Required(ErrorMessage = "Class name is required.")]
    [Display(Name = "Class Name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";


    [Required(ErrorMessage = "Class code is required.")]
    [Display(Name = "Class Code")]
    //
    // Property Name    : Code
    // Purpose          : Gets or sets the code.
    // Data Type        : string
    //
    public string Code { get; set; } = "";


    [Required(ErrorMessage = "Grade is required.")]
    //
    // Property Name    : Grade
    // Purpose          : Stores the grade.
    // Data Type        : string
    //
    public string Grade { get; set; } = "";


    //
    // Property Name    : Section
    // Purpose          : Gets or sets the section.
    // Data Type        : string?
    //
    public string? Section { get; set; }


    [Display(Name = "Class Teacher")]
    //
    // Property Name    : ClassTeacherId
    // Purpose          : Identifies the related class teacher record.
    // Data Type        : Guid?
    //
    public Guid? ClassTeacherId { get; set; }


    [Display(Name = "Room Number")]
    //
    // Property Name    : RoomNumber
    // Purpose          : Gets or sets the room number.
    // Data Type        : string?
    //
    public string? RoomNumber { get; set; }


    [Range(
        1,
        200,
        ErrorMessage = "Capacity must be between 1 and 200.")]
    //
    // Property Name    : Capacity
    // Purpose          : Gets or sets the capacity.
    // Data Type        : int?
    //
    public int? Capacity { get; set; }


    [Required]
    [Display(Name = "Academic Year")]
    //
    // Property Name    : AcademicYear
    // Purpose          : Gets or sets the academic year.
    // Data Type        : int
    //
    public int AcademicYear { get; set; }


    //
    // Property Name    : Teachers
    // Purpose          : Collection of teachers used by this model.
    // Data Type        : List<ClassTeacherOptionViewModel>
    //
    public List<ClassTeacherOptionViewModel>
        Teachers
    { get; set; } = new();
}