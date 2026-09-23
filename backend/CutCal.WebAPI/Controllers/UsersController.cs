using CutCal.Model.Common;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

// Class-level [Authorize] only: any logged-in user may view/edit their OWN account (GetById, Update,
// ChangePassword, Avatar), enforced in UserService/OwnershipGuard. Listing, creating and deleting
// other accounts stays Admin-only via the overrides below.
[Authorize]
public class UsersController : BaseCRUDController<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest, IUserService>
{
    private const long MaxAvatarBytes = 3 * 1024 * 1024;
    private static readonly Dictionary<string, string> AllowedAvatarTypes = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IWebHostEnvironment _env;

    public UsersController(IUserService service, IAuthenticatedUserAccessor userAccessor, IWebHostEnvironment env) : base(service)
    {
        _userAccessor = userAccessor;
        _env = env;
    }

    [Authorize(Roles = RoleNames.Admin)]
    public override Task<ActionResult<PageResult<UserResponse>>> Get([FromQuery] UserSearchObject search) => base.Get(search);

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public override Task<ActionResult<UserResponse>> Insert([FromBody] UserInsertRequest request) => base.Insert(request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);

    [HttpPut("{id:int}/Role")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<UserResponse>> SetRole(int id, [FromBody] UserRoleUpdateRequest request)
    {
        return Ok(await Service.SetRoleAsync(id, request.Role));
    }

    [HttpPut("{id:int}/ChangePassword")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        await Service.ChangePasswordAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:int}/Avatar")]
    public async Task<ActionResult<UserResponse>> UploadAvatar(int id, IFormFile file)
    {
        OwnershipGuard.EnsureIsSelfOrAdmin(id, _userAccessor);

        if (file.Length == 0)
        {
            throw new ClientException("No file uploaded.");
        }
        if (file.Length > MaxAvatarBytes)
        {
            throw new ClientException("Image must be smaller than 3 MB.");
        }
        if (!AllowedAvatarTypes.TryGetValue(file.ContentType, out var extension))
        {
            throw new ClientException("Only JPEG, PNG or WEBP images are allowed.");
        }
        if (!await HasValidImageSignatureAsync(file, extension))
        {
            throw new ClientException("The uploaded file is not a valid JPEG, PNG or WEBP image.");
        }

        var avatarsDir = Path.Combine(_env.WebRootPath, "images", "avatars");
        Directory.CreateDirectory(avatarsDir);
        foreach (var existing in Directory.EnumerateFiles(avatarsDir, $"{id}.*"))
        {
            System.IO.File.Delete(existing);
        }

        var filePath = Path.Combine(avatarsDir, $"{id}{extension}");
        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(await Service.SetAvatarUrlAsync(id, $"/images/avatars/{id}{extension}"));
    }

    /// <summary>
    /// The Content-Type header is client-supplied and trivially spoofable, so this checks the
    /// file's actual magic bytes against the format its extension claims to be.
    /// </summary>
    private static async Task<bool> HasValidImageSignatureAsync(IFormFile file, string extension)
    {
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAsync(header.AsMemory(0, header.Length));

        return extension switch
        {
            ".jpg" => read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                                 && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A,
            ".webp" => read >= 12 && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                                   && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
            _ => false
        };
    }
}
