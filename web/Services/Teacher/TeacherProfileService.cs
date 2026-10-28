
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.ViewModels.Teacher;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SchoolParentalSystem.Web.Services.Teacher;

//
// Class Name       : TeacherProfileService
// Purpose          : Encapsulates teacher profile operations for the School
//                    Parental System.
// Re-use           : Used by Teacher controllers and dashboard actions.
// Input Parameters : Service dependencies or model property values.
// Output Type      : Class instance.
//

public class TeacherProfileService
{
    // =========================================
    // CONSTANTS
    // =========================================

    private const string AvatarBucket = "avatars";

    private const long MaxPictureBytes =
        2 * 1024 * 1024;


    // =========================================
    // DEPENDENCIES
    // =========================================

    private readonly Supabase.Client _supabase;

    private readonly SupabaseSessionService
        _sessionService;

    private readonly ILogger<TeacherProfileService>
        _logger;


    // =========================================
    // CONSTRUCTOR
    // =========================================

    //
    // Method Name      : TeacherProfileService
    // Purpose          : Initializes the service with its required
    //                    dependencies.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Supabase.Client supabase; SupabaseSessionService
    //                    sessionService; ILogger<TeacherProfileService>
    //                    logger
    // Output Type      : Constructor.
    //

    public TeacherProfileService(
        Supabase.Client supabase,
        SupabaseSessionService sessionService,
        ILogger<TeacherProfileService> logger)
    {
        _supabase = supabase;

        _sessionService = sessionService;

        _logger = logger;
    }


    // =========================================
    // GET TEACHER PROFILE
    // =========================================

    //
    // Method Name      : GetProfileAsync
    // Purpose          : Retrieves profile.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<TeacherProfileViewModel>.
    //

    public async Task<TeacherProfileViewModel>
        GetProfileAsync(
            Guid userId,
            Guid schoolId)
    {
        await RequireSessionAsync(
            userId,
            schoolId);


        // GET USER INFORMATION

        var userResponse = await _supabase
            .From<TeacherProfileUserRecord>()
            .Where(x => x.Id == userId)
            .Get();

        var user = userResponse.Models
            .FirstOrDefault();


        // VERIFY ROLE AND SCHOOL

        if (user == null ||
            !string.Equals(
                user.Role,
                "Teacher",
                StringComparison.Ordinal) ||
            user.SchoolId != schoolId)
        {
            throw new UnauthorizedAccessException(
                "Teacher account was not found " +
                "for this school.");
        }


        // GET TEACHER RECORD

        var teacherResponse = await _supabase
            .From<TeacherProfileTeacherRecord>()
            .Where(x => x.UserId == userId)
            .Get();

        var teacher = teacherResponse.Models
            .FirstOrDefault(x =>
                x.SchoolId == schoolId);


        if (teacher == null)
        {
            throw new UnauthorizedAccessException(
                "A Teacher profile is required.");
        }


        // GET SCHOOL DETAILS

        var schoolResponse = await _supabase
            .From<TeacherProfileSchoolRecord>()
            .Where(x => x.Id == schoolId)
            .Get();

        var school = schoolResponse.Models
            .FirstOrDefault();


        if (school == null)
        {
            throw new InvalidOperationException(
                "Assigned school was not found.");
        }


        // BUILD PROFILE VIEWMODEL

        return new TeacherProfileViewModel
        {
            UserId = user.Id,

            SchoolId = schoolId,

            FullName = user.FullName ?? "",

            Email = user.Email ?? "",

            Phone = user.Phone,

            Role = "Teacher",

            ProfilePicture = GetDisplayPictureUrl(
                user.ProfilePicture,
                userId),

            SchoolName = school.Name,

            SchoolAddress = school.Address,

            IsActive =
                user.IsActive &&
                teacher.IsActive,

            MemberSince = user.CreatedAt,

            LastLogin = null
        };
    }


    // =========================================
    // GET PROFILE FOR EDITING
    // =========================================

    //
    // Method Name      : GetEditProfileAsync
    // Purpose          : Retrieves edit Profile.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task<TeacherEditProfileViewModel>.
    //

    public async Task<TeacherEditProfileViewModel>
        GetEditProfileAsync(
            Guid userId,
            Guid schoolId)
    {
        var profile = await GetProfileAsync(
            userId,
            schoolId);


        return new TeacherEditProfileViewModel
        {
            FullName = profile.FullName,

            Phone = profile.Phone,

            Email = profile.Email,

            Role = profile.Role,

            SchoolName = profile.SchoolName,

            SchoolAddress = profile.SchoolAddress,

            IsActive = profile.IsActive
        };
    }


    // =========================================
    // UPDATE TEACHER PROFILE
    // =========================================

    //
    // Method Name      : UpdateProfileAsync
    // Purpose          : Updates profile.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId;
    //                    TeacherEditProfileViewModel input
    // Output Type      : Task.
    //

    public async Task UpdateProfileAsync(
        Guid userId,
        Guid schoolId,
        TeacherEditProfileViewModel input)
    {
        var profile = await GetProfileAsync(
            userId,
            schoolId);


        if (!profile.IsActive)
        {
            throw new UnauthorizedAccessException(
                "An active Teacher account is required.");
        }


        var fullName =
            (input.FullName ?? "").Trim();

        var phone =
            string.IsNullOrWhiteSpace(input.Phone)
                ? null
                : input.Phone.Trim();


        // VALIDATE NAME

        if (fullName.Length < 2 ||
            fullName.Length > 150)
        {
            throw new ArgumentException(
                "Full name must be between " +
                "2 and 150 characters.");
        }


        // VALIDATE PHONE

        if (phone != null &&
            phone.Length > 20)
        {
            throw new ArgumentException(
                "Phone number must be " +
                "20 characters or fewer.");
        }


        // A USER MUST HAVE EMAIL OR PHONE

        if (string.IsNullOrWhiteSpace(profile.Email) &&
            phone == null)
        {
            throw new ArgumentException(
                "A phone number is required " +
                "when no email is registered.");
        }


        // UPDATE THROUGH SECURE RPC
        //
        // Do not use submitted values to update:
        // email, role, school_id or is_active.
        //
        // The database also independently
        // verifies the authenticated Teacher.

        await _supabase.Rpc(
            "update_teacher_own_profile",
            new Dictionary<string, object>
            {
                ["p_full_name"] = fullName,
                ["p_phone"] = phone ?? ""
            });
    }


    // =========================================
    // UPLOAD PROFILE PICTURE
    // =========================================

    //
    // Method Name      : UploadProfilePictureAsync
    // Purpose          : Uploads profile Picture.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId; IFormFile? picture
    // Output Type      : Task.
    //

    public async Task UploadProfilePictureAsync(
        Guid userId,
        Guid schoolId,
        IFormFile? picture)
    {
        var profile = await GetProfileAsync(
            userId,
            schoolId);


        if (!profile.IsActive)
        {
            throw new UnauthorizedAccessException(
                "An active Teacher account is required.");
        }


        // VALIDATE FILE SIZE

        if (picture == null ||
            picture.Length == 0 ||
            picture.Length > MaxPictureBytes)
        {
            throw new ArgumentException(
                "Select a JPG, PNG or WebP " +
                "picture under 2 MB.");
        }


        // READ PICTURE BYTES

        byte[] bytes;

        using (var stream = new MemoryStream())
        {
            await picture.CopyToAsync(stream);

            bytes = stream.ToArray();
        }


        if (bytes.LongLength == 0 ||
            bytes.LongLength > MaxPictureBytes)
        {
            throw new ArgumentException(
                "The selected picture is too large.");
        }


        // VALIDATE REAL IMAGE FILE SIGNATURE

        var detectedType = DetectImageType(bytes);

        if (detectedType == null ||
            !string.Equals(
                picture.ContentType,
                detectedType.Value.Mime,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "The file must be a valid " +
                "JPG, PNG or WebP image.");
        }


        // GENERATE UNIQUE FILE NAME

        var path =
            $"{userId:D}/" +
            $"{Guid.NewGuid():N}." +
            detectedType.Value.Extension;


        // GET PREVIOUS PROFILE PICTURE PATH

        var previousPath =
            await GetSavedPicturePathAsync(userId);


        // UPLOAD IMAGE TO SUPABASE STORAGE

        await _supabase.Storage
            .From(AvatarBucket)
            .Upload(
                bytes,
                path,
                new Supabase.Storage.FileOptions
                {
                    Upsert = false,

                    CacheControl = "3600",

                    ContentType =
                        detectedType.Value.Mime
                },
                inferContentType: false);


        try
        {
            // SAVE PICTURE PATH IN public.users

            await _supabase.Rpc(
                "set_teacher_own_profile_picture",
                new Dictionary<string, object>
                {
                    ["p_storage_path"] = path
                });
        }
        catch
        {
            // REMOVE NEW FILE IF DATABASE
            // SAVE FAILS

            await TryRemoveOwnedAvatarAsync(
                userId,
                path);

            throw;
        }


        // REMOVE OLD PICTURE AFTER
        // NEW PICTURE WAS SAVED

        if (IsOwnedAvatarPath(
            previousPath,
            userId))
        {
            await TryRemoveOwnedAvatarAsync(
                userId,
                previousPath!);
        }
    }


    // =========================================
    // REMOVE PROFILE PICTURE
    // =========================================

    //
    // Method Name      : RemoveProfilePictureAsync
    // Purpose          : Removes profile Picture.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task.
    //

    public async Task RemoveProfilePictureAsync(
        Guid userId,
        Guid schoolId)
    {
        var profile = await GetProfileAsync(
            userId,
            schoolId);


        if (!profile.IsActive)
        {
            throw new UnauthorizedAccessException(
                "An active Teacher account is required.");
        }


        var previousPath =
            await GetSavedPicturePathAsync(userId);


        // CLEAR THE SAVED PICTURE PATH

        await _supabase.Rpc(
            "set_teacher_own_profile_picture",
            new Dictionary<string, object>
            {
                ["p_storage_path"] = ""
            });


        // REMOVE OWNED STORAGE IMAGE

        if (IsOwnedAvatarPath(
            previousPath,
            userId))
        {
            await TryRemoveOwnedAvatarAsync(
                userId,
                previousPath!);
        }
    }


    // =========================================
    // VALIDATE IDENTITY AND RESTORE SESSION
    // =========================================

    //
    // Method Name      : RequireSessionAsync
    // Purpose          : Requires session.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; Guid schoolId
    // Output Type      : Task.
    //

    private async Task RequireSessionAsync(
        Guid userId,
        Guid schoolId)
    {
        if (userId == Guid.Empty ||
            schoolId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Invalid Teacher identity.");
        }


        if (!await _sessionService.RestoreAsync())
        {
            throw new UnauthorizedAccessException(
                "Teacher session expired.");
        }
    }


    // =========================================
    // GET CURRENT SAVED PICTURE
    // =========================================

    //
    // Method Name      : GetSavedPicturePathAsync
    // Purpose          : Retrieves saved Picture Path.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId
    // Output Type      : Task<string?>.
    //

    private async Task<string?>
        GetSavedPicturePathAsync(Guid userId)
    {
        var response = await _supabase
            .From<TeacherProfileUserRecord>()
            .Where(x => x.Id == userId)
            .Get();


        return response.Models
            .FirstOrDefault()?
            .ProfilePicture;
    }


    // =========================================
    // RESOLVE PICTURE URL
    // =========================================

    //
    // Method Name      : GetDisplayPictureUrl
    // Purpose          : Retrieves display Picture Url.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : string? storedValue; Guid userId
    // Output Type      : string?.
    //

    private string? GetDisplayPictureUrl(
        string? storedValue,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
        {
            return null;
        }


        // SUPPORT EXISTING HTTPS PICTURE URLS

        if (Uri.TryCreate(
                storedValue,
                UriKind.Absolute,
                out var existingUrl) &&
            existingUrl.Scheme ==
                Uri.UriSchemeHttps)
        {
            return storedValue;
        }


        // NEW PICTURES ARE SAVED AS
        // USER-ID/FILE-NAME.EXT

        if (IsOwnedAvatarPath(
            storedValue,
            userId))
        {
            return _supabase.Storage
                .From(AvatarBucket)
                .GetPublicUrl(storedValue);
        }


        return null;
    }


    // =========================================
    // VALIDATE STORAGE FILE PATH
    // =========================================

    //
    // Method Name      : IsOwnedAvatarPath
    // Purpose          : Checks whether owned Avatar Path.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : string? value; Guid userId
    // Output Type      : bool.
    //

    private static bool IsOwnedAvatarPath(
        string? value,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }


        var prefix = $"{userId:D}/";


        if (!value.StartsWith(
                prefix,
                StringComparison.Ordinal))
        {
            return false;
        }


        var file = value.Substring(
            prefix.Length);


        if (file.Length < 36 ||
            file.Length > 37)
        {
            return false;
        }


        var dotIndex = file.IndexOf('.');


        if (dotIndex != 32)
        {
            return false;
        }


        if (!Guid.TryParseExact(
                file.Substring(0, 32),
                "N",
                out _))
        {
            return false;
        }


        var extension =
            file.Substring(dotIndex + 1);


        return extension is
            "jpg" or "png" or "webp";
    }


    // =========================================
    // REMOVE PREVIOUS AVATAR FILE
    // =========================================

    //
    // Method Name      : TryRemoveOwnedAvatarAsync
    // Purpose          : Tries to remove Owned Avatar.
    // Re-use           : Called from Teacher pages or related service
    //                    operations.
    // Input Parameters : Guid userId; string path
    // Output Type      : Task.
    //

    private async Task TryRemoveOwnedAvatarAsync(
        Guid userId,
        string path)
    {
        if (!IsOwnedAvatarPath(path, userId))
        {
            return;
        }


        try
        {
            await _supabase.Storage
                .From(AvatarBucket)
                .Remove(new List<string>
                {
                    path
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Could not clean up Teacher " +
                "avatar for {TeacherId}",
                userId);
        }
    }


    // =========================================
    // VALIDATE IMAGE SIGNATURE
    // =========================================

    private static (string Mime, string Extension)?
        DetectImageType(byte[] bytes)
    {
        // JPEG SIGNATURE

        if (bytes.Length >= 3 &&
            bytes[0] == 0xFF &&
            bytes[1] == 0xD8 &&
            bytes[2] == 0xFF)
        {
            return ("image/jpeg", "jpg");
        }


        // PNG SIGNATURE

        if (bytes.Length >= 8 &&
            bytes[0] == 137 &&
            bytes[1] == 80 &&
            bytes[2] == 78 &&
            bytes[3] == 71 &&
            bytes[4] == 13 &&
            bytes[5] == 10 &&
            bytes[6] == 26 &&
            bytes[7] == 10)
        {
            return ("image/png", "png");
        }


        // WEBP SIGNATURE

        if (bytes.Length >= 12 &&
            bytes[0] == 82 &&
            bytes[1] == 73 &&
            bytes[2] == 70 &&
            bytes[3] == 70 &&
            bytes[8] == 87 &&
            bytes[9] == 69 &&
            bytes[10] == 66 &&
            bytes[11] == 80)
        {
            return ("image/webp", "webp");
        }


        return null;
    }
}


// ============================================
// DATABASE MODEL: public.users
// ============================================

//
// Class Name       : TeacherProfileUserRecord
// Purpose          : Holds teacher profile user record data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("users")]
public class TeacherProfileUserRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = "";

    [Column("email")]
    public string? Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("role")]
    public string Role { get; set; } = "";

    [Column("school_id")]
    public Guid? SchoolId { get; set; }

    [Column("profile_picture")]
    public string? ProfilePicture { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}


// ============================================
// DATABASE MODEL: public.teachers
// ============================================

//
// Class Name       : TeacherProfileTeacherRecord
// Purpose          : Holds teacher profile teacher record data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("teachers")]
public class TeacherProfileTeacherRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("school_id")]
    public Guid SchoolId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }
}


// ============================================
// DATABASE MODEL: public.schools
// ============================================

//
// Class Name       : TeacherProfileSchoolRecord
// Purpose          : Holds teacher profile school record data used by the
//                    application.
// Re-use           : Used as typed data by related services and MVC pages.
// Input Parameters : Property values supplied by callers.
// Output Type      : Class instance.
//

[Table("schools")]
public class TeacherProfileSchoolRecord : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("address")]
    public string Address { get; set; } = "";
}
