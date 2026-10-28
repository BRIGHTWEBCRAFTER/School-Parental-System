using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Models;

//
// Class Name       : TopbarNotificationRecord
// Purpose          : Maps the existing Supabase notifications table.
// Re-use           : Teacher and School Admin notification screens.
// Input Parameters : Data returned by Supabase.
// Output Type      : Notification database model.
//
[Table("notifications")]
public class TopbarNotificationRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("title")]
    public string Title { get; set; } = "";

    [Column("message")]
    public string Message { get; set; } = "";

    [Column("notification_type")]
    public string NotificationType { get; set; } = "";

    [Column("is_read")]
    public bool IsRead { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
