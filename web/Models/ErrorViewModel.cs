namespace SchoolParentalSystem.Web.Models;

//
// Class Name       : ErrorViewModel
// Purpose          : Represents error view model data within the application.
// Re-use           : Mapped by Supabase Postgrest services and domain logic.
// Input Parameters : Data supplied through the public properties.
// Output Type      : ErrorViewModel object.
//
public class ErrorViewModel
{
    //
    // Property Name    : RequestId
    // Purpose          : Identifies the related request record.
    // Data Type        : string?
    //
    public string? RequestId { get; set; }

    //
    // Property Name    : ShowRequestId
    // Purpose          : Identifies the related show request record.
    // Data Type        : bool
    //
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
