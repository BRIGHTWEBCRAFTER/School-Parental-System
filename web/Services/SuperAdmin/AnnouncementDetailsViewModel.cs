namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : AnnouncementDetailsViewModel
// Purpose          : Holds announcement details view model data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

public class AnnouncementDetailsViewModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public string Content { get; set; } = "";

    public string SchoolName { get; set; } = "";

    public string Priority { get; set; } = "";

    public bool AcknowledgementRequired { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }
}