using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SupabaseSessionService = SchoolParentalSystem.Web.Services.SupabaseSessionService;
using SchoolParentalSystem.Web.ViewModels.SuperAdmin;

using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace SchoolParentalSystem.Web.Services.SuperAdmin;


public class SuperAdminProfileService
{
    private readonly Supabase.Client
        _supabase;

    private readonly SupabaseSessionService
        _sessionService;

    private readonly ILogger<SuperAdminProfileService> _logger;
    private const string AvatarBucket = "avatars";
    private const long MaxPictureBytes = 2 * 1024 * 1024;



    public SuperAdminProfileService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        ILogger<SuperAdminProfileService> logger)
    {
        _supabase =
            supabase;

        _sessionService =
            sessionService;

        _logger = logger;
    }


    // =====================================================
    // GET PROFILE
    // =====================================================

    public async Task<SuperAdminProfileViewModel>
        GetProfileAsync(
            Guid userId)
    {
        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        var response =
            await _supabase
                .From<UserRecord>()
                .Where(
                    user =>
                        user.Id == userId)
                .Get();


        var user =
            response.Models
                .FirstOrDefault();


        if (user == null)
        {
            throw new InvalidOperationException(
                "User profile could not be found.");
        }


        return new SuperAdminProfileViewModel
        {
            UserId =
                user.Id,

            FullName =
                user.FullName ?? "",

            Email =
                user.Email ?? "",

            Phone =
                user.Phone ?? "",

            ProfilePicture = GetPictureUrl(user.ProfilePicture, userId),

            Role =
                FormatRole(
                    user.Role),

            IsActive =
                user.IsActive,

            CreatedAt =
                user.CreatedAt,

            UpdatedFullName =
                user.FullName ?? "",

            UpdatedPhone =
                user.Phone ?? ""
        };
    }


    // =====================================================
    // UPDATE PROFILE
    // =====================================================

    public async Task UpdateProfileAsync(
        Guid userId,
        string fullName,
        string? phone)
    {
        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        fullName =
            fullName?.Trim() ?? "";

        phone =
            phone?.Trim() ?? "";


        if (string.IsNullOrWhiteSpace(
            fullName))
        {
            throw new ArgumentException(
                "Full name is required.");
        }


        var response =
            await _supabase
                .From<UserRecord>()
                .Where(
                    user =>
                        user.Id == userId)
                .Get();


        var user =
            response.Models
                .FirstOrDefault();


        if (user == null)
        {
            throw new InvalidOperationException(
                "User profile could not be found.");
        }


        await _supabase
            .From<UserRecord>()
            .Where(
                user =>
                    user.Id == userId)
            .Set(
                user =>
                    user.FullName!,
                fullName)
            .Set(
                user =>
                    user.Phone!,
                phone)
            .Update();
    }


    // =====================================================
    // SEND PASSWORD RESET
    // =====================================================

    public async Task SendPasswordResetAsync(
        Guid userId,
        string redirectUrl)
    {
        if (!await _sessionService
            .RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Super Admin session expired.");
        }


        if (string.IsNullOrWhiteSpace(
            redirectUrl))
        {
            throw new ArgumentException(
                "Password reset redirect URL is required.");
        }


        var response =
            await _supabase
                .From<UserRecord>()
                .Where(
                    user =>
                        user.Id == userId)
                .Get();


        var user =
            response.Models
                .FirstOrDefault();


        if (user == null)
        {
            throw new InvalidOperationException(
                "User profile could not be found.");
        }


        if (string.IsNullOrWhiteSpace(
            user.Email))
        {
            throw new InvalidOperationException(
                "No email address is linked to this account.");
        }


        var options =
            new ResetPasswordForEmailOptions(
                user.Email)
            {
                RedirectTo =
                    redirectUrl,

                FlowType =
                    Constants.OAuthFlowType.Implicit
            };


        await _supabase.Auth
            .ResetPasswordForEmail(
                options);
    }



    // =====================================================
    // UPLOAD PROFILE PICTURE
    // Purpose : Store an owned Super Admin avatar in Supabase
    //           Storage and its path in public.users.
    // Re-use  : Super Admin profile Upload Picture form.
    // Input   : Logged-in user ID and validated image file.
    // Output  : Task
    // =====================================================

    public async Task UploadProfilePictureAsync(Guid userId, IFormFile? picture)
    {
        await RequireActiveSuperAdminAsync(userId);

        if (picture == null || picture.Length == 0 || picture.Length > MaxPictureBytes)
            throw new ArgumentException("Choose a JPG, PNG or WebP image under 2 MB.");

        byte[] bytes;
        using (var stream = new MemoryStream())
        {
            await picture.CopyToAsync(stream);
            bytes = stream.ToArray();
        }

        var imageType = DetectImageType(bytes);
        if (imageType == null ||
            !string.Equals(picture.ContentType, imageType.Value.Mime,
                StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Please select a valid JPG, PNG or WebP image.");

        var oldPath = await GetStoredPicturePathAsync(userId);
        var newPath = $"{userId:D}/{Guid.NewGuid():N}.{imageType.Value.Extension}";

        await _supabase.Storage.From(AvatarBucket).Upload(
            bytes,
            newPath,
            new Supabase.Storage.FileOptions
            {
                Upsert = false,
                CacheControl = "3600",
                ContentType = imageType.Value.Mime
            },
            inferContentType: false);

        try
        {
            await _supabase.From<UserRecord>()
                .Where(x => x.Id == userId)
                .Set(x => x.ProfilePicture!, newPath)
                .Update();

            var savedPath = await GetStoredPicturePathAsync(userId);
            if (savedPath != newPath)
                throw new InvalidOperationException("Profile picture could not be saved to the user record.");
        }
        catch
        {
            await TryRemoveAvatarAsync(userId, newPath);
            throw;
        }

        if (IsOwnedPicturePath(oldPath, userId))
            await TryRemoveAvatarAsync(userId, oldPath!);
    }


    // =====================================================
    // REMOVE PROFILE PICTURE
    // Purpose : Clear only the logged-in Super Admin's avatar.
    // Re-use  : Super Admin Remove Picture button.
    // Input   : Logged-in user ID.
    // Output  : Task
    // =====================================================

    public async Task RemoveProfilePictureAsync(Guid userId)
    {
        await RequireActiveSuperAdminAsync(userId);

        var oldPath = await GetStoredPicturePathAsync(userId);

        await _supabase.From<UserRecord>()
            .Where(x => x.Id == userId)
            .Set(x => x.ProfilePicture!, "")
            .Update();

        if (!string.IsNullOrEmpty(await GetStoredPicturePathAsync(userId)))
            throw new InvalidOperationException("Profile picture could not be removed from the user record.");

        if (IsOwnedPicturePath(oldPath, userId))
            await TryRemoveAvatarAsync(userId, oldPath!);
    }


    private async Task RequireActiveSuperAdminAsync(Guid userId)
    {
        var profile = await GetProfileAsync(userId);
        if (!profile.IsActive || profile.Role != "Super Admin")
            throw new UnauthorizedAccessException("An active Super Admin account is required.");
    }


    private async Task<string?> GetStoredPicturePathAsync(Guid userId)
    {
        var response = await _supabase.From<UserRecord>()
            .Where(x => x.Id == userId).Get();
        return response.Models.FirstOrDefault()?.ProfilePicture;
    }


    private string? GetPictureUrl(string? saved, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(saved)) return null;

        if (Uri.TryCreate(saved, UriKind.Absolute, out var url)
            && url.Scheme == Uri.UriSchemeHttps)
            return saved;

        return IsOwnedPicturePath(saved, userId)
            ? _supabase.Storage.From(AvatarBucket).GetPublicUrl(saved)
            : null;
    }


    private static bool IsOwnedPicturePath(string? path, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(path) ||
            !path.StartsWith($"{userId:D}/", StringComparison.Ordinal))
            return false;

        var filename = path.Substring(userId.ToString("D").Length + 1);
        var extension = Path.GetExtension(filename);
        var identifier = Path.GetFileNameWithoutExtension(filename);

        return Guid.TryParseExact(identifier, "N", out _) &&
            (extension == ".jpg" || extension == ".png" || extension == ".webp");
    }


    private async Task TryRemoveAvatarAsync(Guid userId, string path)
    {
        if (!IsOwnedPicturePath(path, userId)) return;
        try
        {
            await _supabase.Storage.From(AvatarBucket)
                .Remove(new List<string> { path });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not remove old Super Admin avatar.");
        }
    }


    private static (string Mime, string Extension)? DetectImageType(byte[] bytes)
    {
        if (bytes.Length >= 3 &&
            bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return ("image/jpeg", "jpg");

        if (bytes.Length >= 8 &&
            bytes[0] == 137 && bytes[1] == 80 && bytes[2] == 78 &&
            bytes[3] == 71 && bytes[4] == 13 && bytes[5] == 10 &&
            bytes[6] == 26 && bytes[7] == 10)
            return ("image/png", "png");

        if (bytes.Length >= 12 &&
            bytes[0] == 82 && bytes[1] == 73 && bytes[2] == 70 &&
            bytes[3] == 70 && bytes[8] == 87 && bytes[9] == 69 &&
            bytes[10] == 66 && bytes[11] == 80)
            return ("image/webp", "webp");

        return null;
    }


    // =====================================================
    // ROLE FORMAT
    // =====================================================

    private static string FormatRole(
        string? role)
    {
        if (string.IsNullOrWhiteSpace(
            role))
        {
            return "";
        }


        var normalized =
            role
                .Trim()
                .Replace(
                    "_",
                    " ")
                .Replace(
                    "-",
                    " ");


        if (string.Equals(
            normalized,
            "super admin",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Super Admin";
        }


        if (string.Equals(
            normalized,
            "school admin",
            StringComparison.OrdinalIgnoreCase))
        {
            return "School Admin";
        }


        if (string.Equals(
            normalized,
            "teacher",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Teacher";
        }


        if (string.Equals(
            normalized,
            "parent",
            StringComparison.OrdinalIgnoreCase))
        {
            return "Parent";
        }


        return normalized;
    }


    // =====================================================
    // USERS TABLE
    // =====================================================

    [Table("users")]
    private class UserRecord : BaseModel
    {
        [PrimaryKey(
            "id",
            false)]
        public Guid Id { get; set; }


        [Column("full_name")]
        public string? FullName { get; set; }


        [Column("email")]
        public string? Email { get; set; }


        [Column("phone")]
        public string? Phone { get; set; }

        [Column("profile_picture")]
        public string? ProfilePicture { get; set; }


        [Column("role")]
        public string? Role { get; set; }


        [Column("is_active")]
        public bool IsActive { get; set; }


        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}