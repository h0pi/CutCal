using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _service;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public FavoritesController(IFavoriteService service, IAuthenticatedUserAccessor userAccessor)
    {
        _service = service;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<List<FavoriteResponse>>> Get()
    {
        return Ok(await _service.GetForUserAsync(_userAccessor.UserId));
    }

    [HttpPost("{salonId:int}")]
    public async Task<ActionResult<FavoriteResponse>> Add(int salonId)
    {
        return Ok(await _service.AddAsync(_userAccessor.UserId, salonId));
    }

    [HttpDelete("{salonId:int}")]
    public async Task<IActionResult> Remove(int salonId)
    {
        await _service.RemoveAsync(_userAccessor.UserId, salonId);
        return NoContent();
    }
}
