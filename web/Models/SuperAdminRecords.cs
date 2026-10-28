using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

// -----------------------------------------------------------------
// One row with every dashboard / summary count.
// Backed by the view public.super_admin_dashboard_stats
// -----------------------------------------------------------------
[Table("super_admin_dashboard_stats")]
//
// Class Name       : SuperAdminStatsRecord
// Purpose          : Represents super admin stats data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminStatsRecord object.
//
public class SuperAdminStatsRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : int
    //
    public int Id { get; set; }

    [Column("total_schools")]
    //
    // Property Name    : TotalSchools
    // Purpose          : Total schools displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalSchools { get; set; }

    [Column("schools_prev")]
    //
    // Property Name    : SchoolsPrev
    // Purpose          : Gets or sets the schools prev.
    // Data Type        : int
    //
    public int SchoolsPrev { get; set; }

    [Column("active_schools")]
    //
    // Property Name    : ActiveSchools
    // Purpose          : Gets or sets the active schools.
    // Data Type        : int
    //
    public int ActiveSchools { get; set; }

    [Column("pending_schools")]
    //
    // Property Name    : PendingSchools
    // Purpose          : Gets or sets the pending schools.
    // Data Type        : int
    //
    public int PendingSchools { get; set; }

    [Column("inactive_schools")]
    //
    // Property Name    : InactiveSchools
    // Purpose          : Gets or sets the inactive schools.
    // Data Type        : int
    //
    public int InactiveSchools { get; set; }

    [Column("total_users")]
    //
    // Property Name    : TotalUsers
    // Purpose          : Total users displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalUsers { get; set; }

    [Column("users_prev")]
    //
    // Property Name    : UsersPrev
    // Purpose          : Gets or sets the users prev.
    // Data Type        : int
    //
    public int UsersPrev { get; set; }

    [Column("total_parents")]
    //
    // Property Name    : TotalParents
    // Purpose          : Total parents displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalParents { get; set; }

    [Column("parents_prev")]
    //
    // Property Name    : ParentsPrev
    // Purpose          : Gets or sets the parents prev.
    // Data Type        : int
    //
    public int ParentsPrev { get; set; }

    [Column("total_school_admins")]
    //
    // Property Name    : TotalSchoolAdmins
    // Purpose          : Total school admins displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalSchoolAdmins { get; set; }

    [Column("school_admins_prev")]
    //
    // Property Name    : SchoolAdminsPrev
    // Purpose          : Gets or sets the school admins prev.
    // Data Type        : int
    //
    public int SchoolAdminsPrev { get; set; }

    [Column("total_teachers")]
    //
    // Property Name    : TotalTeachers
    // Purpose          : Total teachers displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalTeachers { get; set; }

    [Column("teachers_prev")]
    //
    // Property Name    : TeachersPrev
    // Purpose          : Gets or sets the teachers prev.
    // Data Type        : int
    //
    public int TeachersPrev { get; set; }

    [Column("total_learners")]
    //
    // Property Name    : TotalLearners
    // Purpose          : Total learners displayed or calculated for this record.
    // Data Type        : int
    //
    public int TotalLearners { get; set; }

    [Column("active_announcements")]
    //
    // Property Name    : ActiveAnnouncements
    // Purpose          : Gets or sets the active announcements.
    // Data Type        : int
    //
    public int ActiveAnnouncements { get; set; }

    [Column("announcements_prev")]
    //
    // Property Name    : AnnouncementsPrev
    // Purpose          : Gets or sets the announcements prev.
    // Data Type        : int
    //
    public int AnnouncementsPrev { get; set; }
}


// -----------------------------------------------------------------
// Every school with its learner / teacher counts.
// Backed by the view public.school_overview
// -----------------------------------------------------------------
[Table("school_overview")]
//
// Class Name       : SchoolOverviewRecord
// Purpose          : Represents school overview data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolOverviewRecord object.
//
public class SchoolOverviewRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("name")]
    //
    // Property Name    : Name
    // Purpose          : Stores the name.
    // Data Type        : string
    //
    public string Name { get; set; } = "";

    [Column("school_code")]
    //
    // Property Name    : SchoolCode
    // Purpose          : Gets or sets the school code.
    // Data Type        : string?
    //
    public string? SchoolCode { get; set; }

    [Column("school_type")]
    //
    // Property Name    : SchoolType
    // Purpose          : Gets or sets the school type.
    // Data Type        : string?
    //
    public string? SchoolType { get; set; }

    [Column("province")]
    //
    // Property Name    : Province
    // Purpose          : Gets or sets the province.
    // Data Type        : string?
    //
    public string? Province { get; set; }

    [Column("town_city")]
    //
    // Property Name    : TownCity
    // Purpose          : Gets or sets the town city.
    // Data Type        : string?
    //
    public string? TownCity { get; set; }

    [Column("address")]
    //
    // Property Name    : Address
    // Purpose          : Stores the address.
    // Data Type        : string?
    //
    public string? Address { get; set; }

    [Column("phone")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }

    [Column("email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }

    [Column("principal_name")]
    //
    // Property Name    : PrincipalName
    // Purpose          : Gets or sets the principal name.
    // Data Type        : string?
    //
    public string? PrincipalName { get; set; }

    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string?
    //
    public string? Status { get; set; }

    [Column("max_learners")]
    //
    // Property Name    : MaxLearners
    // Purpose          : Gets or sets the max learners.
    // Data Type        : int?
    //
    public int? MaxLearners { get; set; }

    [Column("notes")]
    //
    // Property Name    : Notes
    // Purpose          : Gets or sets the notes.
    // Data Type        : string?
    //
    public string? Notes { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    [Column("learner_count")]
    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }

    [Column("teacher_count")]
    //
    // Property Name    : TeacherCount
    // Purpose          : Computed or stored teacher count value.
    // Data Type        : int
    //
    public int TeacherCount { get; set; }
}


// -----------------------------------------------------------------
// Learner totals for the last 4 months (dashboard bar chart).
// Backed by the view public.monthly_learner_enrollment
// -----------------------------------------------------------------
[Table("monthly_learner_enrollment")]
//
// Class Name       : MonthlyEnrollmentRecord
// Purpose          : Represents monthly enrollment data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : MonthlyEnrollmentRecord object.
//
public class MonthlyEnrollmentRecord : BaseModel
{
    [PrimaryKey("month_start", false)]
    //
    // Property Name    : MonthStart
    // Purpose          : Gets or sets the month start.
    // Data Type        : DateTime
    //
    public DateTime MonthStart { get; set; }

    [Column("learner_count")]
    //
    // Property Name    : LearnerCount
    // Purpose          : Computed or stored learner count value.
    // Data Type        : int
    //
    public int LearnerCount { get; set; }
}


// -----------------------------------------------------------------
// users table, including created_at (UserProfile does not have it)
// -----------------------------------------------------------------
[Table("users")]
//
// Class Name       : SuperAdminUserRecord
// Purpose          : Represents super admin user data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminUserRecord object.
//
public class SuperAdminUserRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("email")]
    //
    // Property Name    : Email
    // Purpose          : Stores the email.
    // Data Type        : string?
    //
    public string? Email { get; set; }

    [Column("full_name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    [Column("role")]
    //
    // Property Name    : Role
    // Purpose          : Stores the role.
    // Data Type        : string
    //
    public string Role { get; set; } = "";

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid?
    //
    public Guid? SchoolId { get; set; }

    [Column("phone")]
    //
    // Property Name    : Phone
    // Purpose          : Stores the phone.
    // Data Type        : string?
    //
    public string? Phone { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


// -----------------------------------------------------------------
// Used at login to block users of a deactivated school
// -----------------------------------------------------------------
[Table("schools")]
//
// Class Name       : SchoolStatusRecord
// Purpose          : Represents school status data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SchoolStatusRecord object.
//
public class SchoolStatusRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string?
    //
    public string? Status { get; set; }
}
