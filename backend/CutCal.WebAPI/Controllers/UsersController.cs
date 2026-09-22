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
}
