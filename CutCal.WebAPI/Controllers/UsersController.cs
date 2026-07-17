using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : BaseCRUDController<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest, IUserService>
{
    public UsersController(IUserService service) : base(service)
    {
    }

    [HttpPut("{id:int}/ChangePassword")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        await Service.ChangePasswordAsync(id, request);
        return NoContent();
    }
}
