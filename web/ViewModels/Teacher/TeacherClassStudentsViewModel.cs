
using System;
using System.Collections.Generic;

namespace SchoolParentalSystem.Web.ViewModels.Teacher
{
    // ==========================================
    // TEACHER CLASS STUDENTS VIEWMODEL
    // ==========================================

    //
    // Class Name       : TeacherClassStudentsViewModel
    // Purpose          : Supplies teacher class students view model information to MVC views and actions.
    // Re-use           : Passed between Teacher controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : TeacherClassStudentsViewModel object.
    //
    public class TeacherClassStudentsViewModel
    {
        // ======================================
        // CLASS INFORMATION
        // ======================================

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
        // Property Name    : Grade
        // Purpose          : Stores the grade.
        // Data Type        : string
        //
        public string Grade { get; set; } = "";

        //
        // Property Name    : RoomNumber
        // Purpose          : Gets or sets the room number.
        // Data Type        : string?
        //
        public string? RoomNumber { get; set; }


        // ======================================
        // STUDENT INFORMATION
        // ======================================

        //
        // Property Name    : Students
        // Purpose          : Collection of students used by this model.
        // Data Type        : List<TeacherClassStudentItemViewModel>
        //
        public List<TeacherClassStudentItemViewModel>
            Students
        { get; set; } = new();


        // ======================================
        // DISPLAY HELPERS
        // ======================================

        //
        // Property Name    : TotalStudents
        // Purpose          : Total students displayed or calculated for this record.
        // Data Type        : int
        //
        public int TotalStudents => Students.Count;

        //
        // Property Name    : HasStudents
        // Purpose          : Indicates whether students is available.
        // Data Type        : bool
        //
        public bool HasStudents => Students.Count > 0;
    }


    // ==========================================
    // INDIVIDUAL STUDENT VIEWMODEL
    // ==========================================

    //
    // Class Name       : TeacherClassStudentItemViewModel
    // Purpose          : Describes one teacher class student item view model row shown in a page.
    // Re-use           : Passed between Teacher controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : TeacherClassStudentItemViewModel object.
    //
    public class TeacherClassStudentItemViewModel
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
    }
}
