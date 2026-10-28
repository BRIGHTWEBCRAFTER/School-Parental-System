using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("schools")]
//
// Class Name       : DashboardSchoolRecord
// Purpose          : Represents dashboard school data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardSchoolRecord object.
//
public class DashboardSchoolRecord : BaseModel
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
}


[Table("teachers")]
//
// Class Name       : DashboardTeacherRecord
// Purpose          : Represents dashboard teacher data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardTeacherRecord object.
//
public class DashboardTeacherRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("is_active")]
    //
    // Property Name    : IsActive
    // Purpose          : Indicates whether the record is active.
    // Data Type        : bool
    //
    public bool IsActive { get; set; }
}


[Table("attendance")]
//
// Class Name       : DashboardAttendanceRecord
// Purpose          : Represents dashboard attendance data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardAttendanceRecord object.
//
public class DashboardAttendanceRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("learner_id")]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }


    [Column("subject_id")]
    //
    // Property Name    : SubjectId
    // Purpose          : Identifies the related subject record.
    // Data Type        : Guid?
    //
    public Guid? SubjectId { get; set; }


    [Column("attendance_date")]
    //
    // Property Name    : AttendanceDate
    // Purpose          : Date or time associated with attendance date.
    // Data Type        : DateTime
    //
    public DateTime AttendanceDate { get; set; }

    [Column("status")]
    //
    // Property Name    : Status
    // Purpose          : Stores the status.
    // Data Type        : string
    //
    public string Status { get; set; } = "";

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


[Table("announcements")]
//
// Class Name       : DashboardAnnouncementRecord
// Purpose          : Represents dashboard announcement data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardAnnouncementRecord object.
//
public class DashboardAnnouncementRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("title")]
    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    [Column("content")]
    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";

    [Column("expires_at")]
    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


[Table("events")]
//
// Class Name       : DashboardEventRecord
// Purpose          : Represents dashboard event data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardEventRecord object.
//
public class DashboardEventRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("event_name")]
    //
    // Property Name    : EventName
    // Purpose          : Gets or sets the event name.
    // Data Type        : string
    //
    public string EventName { get; set; } = "";

    [Column("event_date")]
    //
    // Property Name    : EventDate
    // Purpose          : Date or time associated with event date.
    // Data Type        : DateTime
    //
    public DateTime EventDate { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


[Table("grades")]
//
// Class Name       : DashboardGradeRecord
// Purpose          : Represents dashboard grade data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardGradeRecord object.
//
public class DashboardGradeRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("learner_id")]
    //
    // Property Name    : LearnerId
    // Purpose          : Identifies the related learner record.
    // Data Type        : Guid
    //
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    //
    // Property Name    : ClassId
    // Purpose          : Identifies the related class record.
    // Data Type        : Guid
    //
    public Guid ClassId { get; set; }

    [Column("score")]
    //
    // Property Name    : Score
    // Purpose          : Gets or sets the score.
    // Data Type        : decimal
    //
    public decimal Score { get; set; }

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}


[Table("learners")]
//
// Class Name       : DashboardLearnerActivityRecord
// Purpose          : Represents dashboard learner activity data retrieved from or sent to Supabase.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : DashboardLearnerActivityRecord object.
//
public class DashboardLearnerActivityRecord : BaseModel
{
    [PrimaryKey("id", false)]
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    [Column("school_id")]
    //
    // Property Name    : SchoolId
    // Purpose          : Identifies the related school record.
    // Data Type        : Guid
    //
    public Guid SchoolId { get; set; }

    [Column("full_name")]
    //
    // Property Name    : FullName
    // Purpose          : Stores the full name.
    // Data Type        : string
    //
    public string FullName { get; set; } = "";

    [Column("created_at")]
    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }
}