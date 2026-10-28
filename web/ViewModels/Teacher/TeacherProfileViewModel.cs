
using System;
using System.Linq;

namespace SchoolParentalSystem.Web.ViewModels.Teacher;

//
// Class Name       : TeacherProfileViewModel
// Purpose          : Supplies teacher profile view model information to MVC views and actions.
// Re-use           : Passed between Teacher controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : TeacherProfileViewModel object.
//
public class TeacherProfileViewModel
{
    // TEACHER INFORMATION

    //
    // Property Name    : UserId
    // Purpose          : Identifies the related user record.
    // Data Type        : Guid
    //
    public Guid UserId { get; set; }

    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string
    //
    public string Email { get; set; } = "";

    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }

    //
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "Teacher";

    //
    // Property Name    : ProfilePicture
    // Purpose          : Gets or sets the profile picture.
    // Data Type        : string?
    //
    public string? ProfilePicture { get; set; }


    // SCHOOL INFORMATION

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : SchoolAddress
    // Purpose          : Gets or sets the school address.
    // Data Type        : string?
    //
    public string? SchoolAddress { get; set; }


    // ACCOUNT INFORMATION

    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    //
    // Property Name    : MemberSince
    // Purpose          : Gets or sets the member since.
    // Data Type        : DateTime?
    //
    public DateTime? MemberSince { get; set; }

    //
    // Property Name    : LastLogin
    // Purpose          : Gets or sets the last login.
    // Data Type        : DateTime?
    //
    public DateTime? LastLogin { get; set; }


    // DISPLAY HELPERS

    //
    // Property Name    : Initials
    // Purpose          : Gets or sets the initials.
    // Data Type        : string
    //
    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                return "T";
            }

            var parts = FullName.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            return string.Concat(
                parts.Take(2)
                     .Select(x => x[0]))
                .ToUpperInvariant();
        }
    }

    //
    // Property Name    : AccountStatus
    // Purpose          : Gets or sets the account status.
    // Data Type        : string
    //
    public string AccountStatus =>
        IsActive ? "Active" : "Inactive";
}
