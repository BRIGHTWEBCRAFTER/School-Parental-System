using System.Collections.Generic;
using SchoolParentalSystem.Web.Models;

namespace SchoolParentalSystem.Web.ViewModels;

//
// Class Name       : TopbarNotificationsViewModel
// Purpose          : Supplies real notifications and unread totals to the top bar.
// Re-use           : Notification dropdown and notification listing page.
// Input Parameters : Notifications read for the current authenticated user.
// Output Type      : View model.
//
public class TopbarNotificationsViewModel
{
    public int UnreadCount { get; set; }

    public List<TopbarNotificationRecord> Notifications { get; set; } = new();

    public bool LoadFailed { get; set; }
}
