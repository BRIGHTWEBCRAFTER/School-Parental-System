namespace SchoolParentalSystem.Web.Services;

public class SupabaseSessionService
{
    private const string AccessTokenKey =
        "SupabaseAccessToken";

    private const string RefreshTokenKey =
        "SupabaseRefreshToken";

    private readonly Supabase.Client _supabase;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public SupabaseSessionService(
        Supabase.Client supabase,
        IHttpContextAccessor httpContextAccessor)
    {
        _supabase = supabase;
        _httpContextAccessor = httpContextAccessor;
    }


    private ISession? Session =>
        _httpContextAccessor.HttpContext?.Session;


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


    public void Clear()
    {
        Session?.Remove(AccessTokenKey);

        Session?.Remove(RefreshTokenKey);
    }
}