using CutCal.Model.Constants;
using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize]
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
    [Authorize(Roles = RoleNames.Customer)]
    public async Task<ActionResult<List<RecommendationResponse>>> Get([FromQuery] double? lat, [FromQuery] double? lng)
    {
        return Ok(await _service.GetRecommendationsAsync(_userAccessor.UserId, lat, lng));
    }

    /// <summary>Queues a retrain; the worker trains in the background and the new model is used once it is done.</summary>
    [HttpPost("Retrain")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Retrain()
    {
        await _service.RequestRetrainAsync();
        return Accepted();
    }
}
