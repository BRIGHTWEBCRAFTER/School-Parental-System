using Microsoft.AspNetCore.Http;
using Supabase.Storage;
using SchoolParentalSystem.Web.Models;
using SchoolParentalSystem.Web.ViewModels;

namespace SchoolParentalSystem.Web.Services;

//
// Class Name       : SchoolAdminProfileService
// Purpose          : Encapsulates school admin profile operations for the
//                    School Parental System.
// Re-use           : Called by School Admin MVC controllers and related
//                    services.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class SchoolAdminProfileService
{
    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;


    //
    // Method Name      : SchoolAdminProfileService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService
    // Output Type      : Constructor.
    //

    public SchoolAdminProfileService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService)
    {
        _supabase = supabase;

        _sessionService = sessionService;
    }


    // =========================================
    // GET SCHOOL ADMIN PROFILE
    // =========================================

    //
    // Method Name      : GetProfileAsync
    // Purpose          : Retrieves profile.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<SchoolAdminProfileViewModel>.
    //

    public async Task<SchoolAdminProfileViewModel>
        GetProfileAsync(
            Guid userId,
            Guid schoolId)
    {
        // -------------------------------------
        // VALIDATE IDs
        // -------------------------------------

        if (userId == Guid.Empty ||
            schoolId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Invalid administrator account.");
        }


        // -------------------------------------
        // RESTORE SUPABASE SESSION
        // -------------------------------------

        var sessionRestored =
            await _sessionService.RestoreAsync();


        if (!sessionRestored)
        {
            throw new UnauthorizedAccessException(
                "Supabase session could not be restored.");
        }


        // -------------------------------------
        // LOAD ADMINISTRATOR INFORMATION
        // -------------------------------------

        var userResponse =
            await _supabase
                .From<UserProfile>()
                .Where(x => x.Id == userId)
                .Get();


        var admin =
            userResponse.Models
                .SingleOrDefault();


        if (admin == null ||
            admin.SchoolId != schoolId ||
            admin.Role != "School Admin" ||
            !admin.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Administrator profile is unavailable.");
        }


        // -------------------------------------
        // LOAD SCHOOL INFORMATION
        // -------------------------------------

        var schoolResponse =
            await _supabase
                .From<DashboardSchoolRecord>()
                .Where(x => x.Id == schoolId)
                .Get();


        var school =
            schoolResponse.Models
                .SingleOrDefault();


        if (school == null)
        {
            throw new InvalidOperationException(
                "School information could not be found.");
        }


        // -------------------------------------
        // BUILD PROFILE VIEW MODEL
        // -------------------------------------

        return new SchoolAdminProfileViewModel
        {
            UserId =
                admin.Id,

            SchoolId =
                schoolId,

            FullName =
                admin.FullName,

            Email =
                admin.Email ?? "",

            Phone =
                admin.Phone,

            Role =
                admin.Role,

            ProfilePicture =
                admin.ProfilePicture,

            IsActive =
                admin.IsActive,

            SchoolName =
                school.Name
        };
    }


    //
    // Method Name      : GetEditProfileAsync
    // Purpose          : Retrieves edit Profile.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<EditSchoolAdminProfileViewModel>.
    //

    public async Task<EditSchoolAdminProfileViewModel>
        GetEditProfileAsync(
            Guid userId,
            Guid schoolId)
    {
        var profile =
            await GetProfileAsync(
                userId,
                schoolId);


        return new EditSchoolAdminProfileViewModel
        {
            FullName =
                profile.FullName,

            Phone =
                profile.Phone
        };
    }


    //
    // Method Name      : UpdateProfileAsync
    // Purpose          : Updates profile.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : EditSchoolAdminProfileViewModel model; Guid
    //                    userId; Guid schoolId
    // Output Type      : Task.
    //

    public async Task UpdateProfileAsync(
        EditSchoolAdminProfileViewModel model,
        Guid userId,
        Guid schoolId)
    {
        // -----------------------------------------
        // VALIDATE PROFILE INFORMATION
        // -----------------------------------------

        if (model == null)
        {
            throw new InvalidOperationException(
                "Profile information is required.");
        }


        var fullName =
            model.FullName?.Trim() ?? "";


        var phone =
            string.IsNullOrWhiteSpace(model.Phone)
                ? null
                : model.Phone.Trim();


        if (fullName.Length < 2 ||
            fullName.Length > 120)
        {
            throw new InvalidOperationException(
                "Full name must be between 2 and 120 characters.");
        }


        if (phone != null && phone.Length > 30)
        {
            throw new InvalidOperationException(
                "Phone number cannot exceed 30 characters.");
        }


        // -----------------------------------------
        // VERIFY LOGGED-IN ADMINISTRATOR
        // -----------------------------------------

        await GetProfileAsync(
            userId,
            schoolId);


        // -----------------------------------------
        // UPDATE ONLY ALLOWED FIELDS
        // -----------------------------------------

        await _supabase
            .From<UserProfile>()
            .Where(x => x.Id == userId)
            .Where(x => x.SchoolId == schoolId)
            .Set(x => x.FullName, fullName)
            .Set(x => x.Phone, phone)
            .Update();
    }


    //
    // Method Name      : UploadProfilePictureAsync
    // Purpose          : Uploads profile Picture.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : IFormFile picture; Guid userId; Guid schoolId
    // Output Type      : Task.
    //

    public async Task UploadProfilePictureAsync(
        IFormFile picture,
        Guid userId,
        Guid schoolId)
    {
        // =========================================
        // VALIDATE FILE
        // =========================================

        const long maxFileSize = 2 * 1024 * 1024;

        if (picture == null || picture.Length == 0)
        {
            throw new InvalidOperationException(
                "Please select a profile picture.");
        }

        if (picture.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "The profile picture cannot exceed 2 MB.");
        }

        // =========================================
        // VERIFY ADMINISTRATOR
        // =========================================

        await GetProfileAsync(userId, schoolId);

        // =========================================
        // READ IMAGE
        // =========================================

        using var stream = new MemoryStream();

        await picture.CopyToAsync(stream);

        var imageBytes = stream.ToArray();

        if (imageBytes.Length == 0 ||
            imageBytes.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "The selected image is invalid or too large.");
        }

        // =========================================
        // VERIFY IMAGE TYPE
        // =========================================

        var contentType =
            picture.ContentType?.Trim().ToLowerInvariant();

        var isJpeg =
            imageBytes.Length >= 3 &&
            imageBytes[0] == 0xFF &&
            imageBytes[1] == 0xD8 &&
            imageBytes[2] == 0xFF;

        var isPng =
            imageBytes.Length >= 8 &&
            imageBytes[0] == 0x89 &&
            imageBytes[1] == 0x50 &&
            imageBytes[2] == 0x4E &&
            imageBytes[3] == 0x47 &&
            imageBytes[4] == 0x0D &&
            imageBytes[5] == 0x0A &&
            imageBytes[6] == 0x1A &&
            imageBytes[7] == 0x0A;

        var isWebp =
            imageBytes.Length >= 12 &&
            imageBytes[0] == 0x52 &&
            imageBytes[1] == 0x49 &&
            imageBytes[2] == 0x46 &&
            imageBytes[3] == 0x46 &&
            imageBytes[8] == 0x57 &&
            imageBytes[9] == 0x45 &&
            imageBytes[10] == 0x42 &&
            imageBytes[11] == 0x50;

        string extension;

        if (contentType == "image/jpeg" && isJpeg)
        {
            extension = ".jpg";
        }
        else if (contentType == "image/png" && isPng)
        {
            extension = ".png";
        }
        else if (contentType == "image/webp" && isWebp)
        {
            extension = ".webp";
        }
        else
        {
            throw new InvalidOperationException(
                "Only valid JPEG, PNG or WebP images are allowed.");
        }

        // =========================================
        // GENERATE UNIQUE STORAGE PATH
        // =========================================

        var storagePath =
            $"{userId}/profile_{Guid.NewGuid():N}{extension}";

        var bucket =
            _supabase.Storage.From("avatars");

        var uploaded = false;

        try
        {
            // =====================================
            // UPLOAD IMAGE TO SUPABASE STORAGE
            // =====================================

            await bucket.Upload(
                imageBytes,
                storagePath,

                new Supabase.Storage.FileOptions
                {
                    ContentType = contentType,
                    Upsert = false,
                    CacheControl = "3600"
                },

                inferContentType: false);

            uploaded = true;

            // =====================================
            // GENERATE PUBLIC IMAGE URL
            // =====================================

            var pictureUrl =
                bucket.GetPublicUrl(storagePath);

            // =====================================
            // UPDATE USER PROFILE
            // =====================================

            var response =
                await _supabase
                    .From<UserProfile>()
                    .Where(x => x.Id == userId)
                    .Where(x => x.SchoolId == schoolId)
                    .Set(
                        x => x.ProfilePicture,
                        pictureUrl)
                    .Update();

            if (!response.Models.Any())
            {
                throw new InvalidOperationException(
                    "The profile picture could not be saved.");
            }
        }
        catch
        {
            // Remove the newly uploaded file if
            // updating the database did not succeed.

            if (uploaded)
            {
                try
                {
                    await bucket.Remove(storagePath);
                }
                catch
                {
                    // Preserve the original error.
                    // Storage cleanup can be retried later.
                }
            }

            throw;
        }
    }


    //
    // Method Name      : RemoveProfilePictureAsync
    // Purpose          : Removes profile Picture.
    // Re-use           : Called from School Admin pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<bool>.
    //

    public async Task<bool> RemoveProfilePictureAsync(
        Guid userId,
        Guid schoolId)
    {
        // =========================================
        // VERIFY ADMINISTRATOR
        // =========================================

        var profile =
            await GetProfileAsync(
                userId,
                schoolId);

        if (string.IsNullOrWhiteSpace(
            profile.ProfilePicture))
        {
            return true;
        }

        // =========================================
        // IDENTIFY EXISTING STORAGE FILE
        // =========================================

        var bucket =
            _supabase.Storage.From("avatars");

        var folderPrefix =
            bucket.GetPublicUrl($"{userId}/");

        string? storagePath = null;

        if (!string.IsNullOrEmpty(folderPrefix) &&
            profile.ProfilePicture.StartsWith(
                folderPrefix,
                StringComparison.Ordinal))
        {
            var fileName =
                profile.ProfilePicture[
                    folderPrefix.Length..];

            // Only delete images created by our
            // School Admin profile upload feature.
            var validFileName =
                System.Text.RegularExpressions.Regex.IsMatch(
                    fileName,
                    @"^profile_[0-9a-f]{32}\.(jpg|png|webp)$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (validFileName)
            {
                storagePath =
                    $"{userId}/{fileName}";
            }
        }

        // =========================================
        // CLEAR PROFILE PICTURE IN DATABASE
        // =========================================

        var response =
            await _supabase
                .From<UserProfile>()
                .Where(x => x.Id == userId)
                .Where(x => x.SchoolId == schoolId)
                .Set(
                    x => x.ProfilePicture,
                    (string?)null)
                .Update();

        if (response.Models.Count == 0)
        {
            throw new InvalidOperationException(
                "The profile picture could not be removed.");
        }

        // =========================================
        // REMOVE OLD FILE FROM STORAGE
        // =========================================

        if (storagePath == null)
        {
            // The database was updated, but the URL
            // wasn't recognized as an upload owned
            // by this feature.
            return false;
        }

        try
        {
            await bucket.Remove(storagePath);

            return true;
        }
        catch
        {
            // The profile picture has already been
            // cleared from the database.
            // Signal that storage cleanup failed.
            return false;
        }
    }



}
