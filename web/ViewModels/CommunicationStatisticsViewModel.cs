
using System;
using System.Collections.Generic;

namespace SchoolParentalSystem.Web.ViewModels
{
    //
    // Class Name       : CommunicationStatisticsViewModel
    // Purpose          : Supplies communication statistics view model information to MVC views and actions.
    // Re-use           : Passed between Communication controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : CommunicationStatisticsViewModel object.
    //
    public class CommunicationStatisticsViewModel
    {
        // ANNOUNCEMENTS
        //
        // Property Name    : TotalAnnouncements
        // Purpose          : Total announcements displayed or calculated for this record.
        // Data Type        : int
        //
        public int TotalAnnouncements { get; set; }
        //
        // Property Name    : ActiveAnnouncements
        // Purpose          : Gets or sets the active announcements.
        // Data Type        : int
        //
        public int ActiveAnnouncements { get; set; }
        //
        // Property Name    : ExpiredAnnouncements
        // Purpose          : Gets or sets the expired announcements.
        // Data Type        : int
        //
        public int ExpiredAnnouncements { get; set; }

        // PRIORITY BREAKDOWN
        //
        // Property Name    : NormalAnnouncements
        // Purpose          : Gets or sets the normal announcements.
        // Data Type        : int
        //
        public int NormalAnnouncements { get; set; }
        //
        // Property Name    : ImportantAnnouncements
        // Purpose          : Gets or sets the important announcements.
        // Data Type        : int
        //
        public int ImportantAnnouncements { get; set; }
        //
        // Property Name    : UrgentAnnouncements
        // Purpose          : Gets or sets the urgent announcements.
        // Data Type        : int
        //
        public int UrgentAnnouncements { get; set; }

        // RECIPIENT ENGAGEMENT
        //
        // Property Name    : TotalRecipients
        // Purpose          : Total recipients displayed or calculated for this record.
        // Data Type        : int
        //
        public int TotalRecipients { get; set; }
        //
        // Property Name    : ReadRecipients
        // Purpose          : Gets or sets the read recipients.
        // Data Type        : int
        //
        public int ReadRecipients { get; set; }
        //
        // Property Name    : UnreadRecipients
        // Purpose          : Gets or sets the unread recipients.
        // Data Type        : int
        //
        public int UnreadRecipients { get; set; }

        //
        // Property Name    : ReadRatePercent
        // Purpose          : Gets or sets the read rate percent.
        // Data Type        : double
        //
        public double ReadRatePercent =>
            TotalRecipients == 0
                ? 0
                : Math.Round(
                    ReadRecipients * 100.0 / TotalRecipients,
                    1);

        // ACKNOWLEDGEMENTS
        //
        // Property Name    : AcknowledgementRequiredRecipients
        // Purpose          : Gets or sets the acknowledgement required recipients.
        // Data Type        : int
        //
        public int AcknowledgementRequiredRecipients
        {
            get;
            set;
        }

        //
        // Property Name    : AcknowledgedRecipients
        // Purpose          : Gets or sets the acknowledged recipients.
        // Data Type        : int
        //
        public int AcknowledgedRecipients { get; set; }

        //
        // Property Name    : PendingAcknowledgements
        // Purpose          : Gets or sets the pending acknowledgements.
        // Data Type        : int
        //
        public int PendingAcknowledgements =>
            Math.Max(
                0,
                AcknowledgementRequiredRecipients
                    - AcknowledgedRecipients);

        //
        // Property Name    : AcknowledgementRatePercent
        // Purpose          : Gets or sets the acknowledgement rate percent.
        // Data Type        : double
        //
        public double AcknowledgementRatePercent =>
            AcknowledgementRequiredRecipients == 0
                ? 0
                : Math.Round(
                    AcknowledgedRecipients * 100.0
                        / AcknowledgementRequiredRecipients,
                    1);

        // EVENTS
        //
        // Property Name    : TotalEvents
        // Purpose          : Total events displayed or calculated for this record.
        // Data Type        : int
        //
        public int TotalEvents { get; set; }
        //
        // Property Name    : UpcomingEvents
        // Purpose          : Gets or sets the upcoming events.
        // Data Type        : int
        //
        public int UpcomingEvents { get; set; }
        //
        // Property Name    : PastEvents
        // Purpose          : Gets or sets the past events.
        // Data Type        : int
        //
        public int PastEvents { get; set; }

        // MONTHLY ACTIVITY
        //
        // Property Name    : MonthlyActivity
        // Purpose          : Collection of monthly activity used by this model.
        // Data Type        : List<CommunicationMonthlyActivityViewModel>
        //
        public List<CommunicationMonthlyActivityViewModel>
            MonthlyActivity
        { get; set; } = new();
    }

    //
    // Class Name       : CommunicationMonthlyActivityViewModel
    // Purpose          : Supplies communication monthly activity view model information to MVC views and actions.
    // Re-use           : Passed between Communication controllers/services and Razor views.
    // Input Parameters : Data supplied through the public properties.
    // Output Type      : CommunicationMonthlyActivityViewModel object.
    //
    public class CommunicationMonthlyActivityViewModel
    {
        //
        // Property Name    : MonthLabel
        // Purpose          : Gets or sets the month label.
        // Data Type        : string
        //
        public string MonthLabel { get; set; } = "";

        //
        // Property Name    : Announcements
        // Purpose          : Gets or sets the announcements.
        // Data Type        : int
        //
        public int Announcements { get; set; }

        //
        // Property Name    : Events
        // Purpose          : Gets or sets the events.
        // Data Type        : int
        //
        public int Events { get; set; }
    }
}
