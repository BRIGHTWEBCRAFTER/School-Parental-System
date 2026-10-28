namespace SchoolParentalSystem.Web.ViewModels.SuperAdmin;

//
// Class Name       : AnnouncementManagementViewModel
// Purpose          : Supplies announcement management view model information to MVC views and actions.
// Re-use           : Passed between Communication controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : AnnouncementManagementViewModel object.
//
public class AnnouncementManagementViewModel
{
    //
    // Property Name    : Announcements
    // Purpose          : Collection of announcements used by this model.
    // Data Type        : List<SuperAdminAnnouncementItemViewModel>
    //
    public List<SuperAdminAnnouncementItemViewModel>
        Announcements
    { get; set; } = [];
}


//
// Class Name       : SuperAdminAnnouncementItemViewModel
// Purpose          : Describes one super admin announcement item view model row shown in a page.
// Re-use           : Passed between Super Admin controllers/services and Razor views.
// Input Parameters : Data supplied through the public properties.
// Output Type      : SuperAdminAnnouncementItemViewModel object.
//
public class SuperAdminAnnouncementItemViewModel
{
    //
    // Property Name    : Id
    // Purpose          : Unique identifier of this record.
    // Data Type        : Guid
    //
    public Guid Id { get; set; }

    //
    // Property Name    : Title
    // Purpose          : Stores the title.
    // Data Type        : string
    //
    public string Title { get; set; } = "";

    //
    // Property Name    : SchoolName
    // Purpose          : Stores the school name.
    // Data Type        : string
    //
    public string SchoolName { get; set; } = "";

    //
    // Property Name    : Priority
    // Purpose          : Gets or sets the priority.
    // Data Type        : string
    //
    public string Priority { get; set; } = "";

    //
    // Property Name    : Content
    // Purpose          : Stores the content.
    // Data Type        : string
    //
    public string Content { get; set; } = "";

    //
    // Property Name    : AcknowledgementRequired
    // Purpose          : Gets or sets the acknowledgement required.
    // Data Type        : bool
    //
    public bool AcknowledgementRequired { get; set; }

    //
    // Property Name    : CreatedAt
    // Purpose          : Date or time associated with created at.
    // Data Type        : DateTime
    //
    public DateTime CreatedAt { get; set; }

    //
    // Property Name    : ExpiresAt
    // Purpose          : Date or time associated with expires at.
    // Data Type        : DateTime?
    //
    public DateTime? ExpiresAt { get; set; }
}