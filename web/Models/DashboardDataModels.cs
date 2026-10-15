using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

[Table("schools")]
public class DashboardSchoolRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";
}


[Table("teachers")]
public class DashboardTeacherRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}


[Table("attendance")]
public class DashboardAttendanceRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("learner_id")]
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("attendance_date")]
    public DateTime AttendanceDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}


[Table("announcements")]
public class DashboardAnnouncementRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("content")]
    public string Content { get; set; } = "";

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}


[Table("events")]
public class DashboardEventRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("event_name")]
    public string EventName { get; set; } = "";

    [Column("event_date")]
    public DateTime EventDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}


[Table("grades")]
public class DashboardGradeRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("learner_id")]
    public Guid LearnerId { get; set; }

    [Column("class_id")]
    public Guid ClassId { get; set; }

    [Column("score")]
    public decimal Score { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}


[Table("learners")]
public class DashboardLearnerActivityRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = "";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}