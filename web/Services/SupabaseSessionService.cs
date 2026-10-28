namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SupabaseSessionService
// Purpose          : Encapsulates supabase session operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SupabaseSessionService
{
    private const string AccessTokenKey =
        "SupabaseAccessToken";

    private const string RefreshTokenKey =
        "SupabaseRefreshToken";

    private readonly Supabase.Client _supabase;
    private readonly IHttpContextAccessor _httpContextAccessor;


    //
    // Method Name      : SupabaseSessionService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; IHttpContextAccessor
    //                    httpContextAccessor
    // Output Type      : Constructor.
    //

    public SupabaseSessionService(
        Supabase.Client supabase,
        IHttpContextAccessor httpContextAccessor)
    {
        _supabase = supabase;
        _httpContextAccessor = httpContextAccessor;
    }


    private ISession? Session =>
        _httpContextAccessor.HttpContext?.Session;


    //
    // Method Name      : Store
    // Purpose          : Handles store operations.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : string accessToken; string refreshToken
    // Output Type      : void.
    //

    public void Store(
        string accessToken,
        string refreshToken)
    {
        Session?.SetString(
            AccessTokenKey,
            accessToken);

        Session?.SetString(
            RefreshTokenKey,
            refreshToken);
    }


    //
    // Method Name      : RestoreAsync
    // Purpose          : Handles restore operations.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : None.
    // Output Type      : Task<bool>.
    //

    public async Task<bool> RestoreAsync()
    {
        var session = Session;

        if (session == null)
        {
            return false;
        }

        var accessToken =
            session.GetString(AccessTokenKey);

        var refreshToken =
            session.GetString(RefreshTokenKey);


        if (string.IsNullOrWhiteSpace(accessToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }


        try
        {
            var restoredSession =
                await _supabase.Auth.SetSession(
                    accessToken,
                    refreshToken);


            if (restoredSession == null)
            {
                Clear();

                return false;
            }


            // SetSession can refresh expired tokens.
            // Always store the newest pair.
            if (!string.IsNullOrWhiteSpace(
                    restoredSession.AccessToken) &&
                !string.IsNullOrWhiteSpace(
                    restoredSession.RefreshToken))
            {
                Store(
                    restoredSession.AccessToken,
                    restoredSession.RefreshToken);
            }


            return true;
        }
        catch
        {
            Clear();

            return false;
        }
    }


    //
    // Method Name      : Clear
    // Purpose          : Handles clear operations.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : None.
    // Output Type      : void.
    //

    public void Clear()
    {
        Session?.Remove(AccessTokenKey);

        Session?.Remove(RefreshTokenKey);
    }
}