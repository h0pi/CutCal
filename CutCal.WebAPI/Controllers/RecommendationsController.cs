using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _service;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public RecommendationsController(IRecommendationService service, IAuthenticatedUserAccessor userAccessor)
    {
        _service = service;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecommendationResponse>>> Get([FromQuery] double? lat, [FromQuery] double? lng)
    {
        return Ok(await _service.GetRecommendationsAsync(_userAccessor.UserId, lat, lng));
    }
}
