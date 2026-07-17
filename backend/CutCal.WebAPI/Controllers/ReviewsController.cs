using CutCal.Model.Common;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public ReviewsController(IReviewService service, IAuthenticatedUserAccessor userAccessor)
    {
        _service = service;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<ReviewResponse>>> Get([FromQuery] ReviewSearchObject search)
    {
        return Ok(await _service.GetPagedAsync(search));
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<ReviewResponse>> Insert([FromBody] ReviewInsertRequest request)
    {
        return Ok(await _service.InsertAsync(request, _userAccessor.UserId));
    }

    [HttpPut("{id:int}/Reply")]
    [Authorize(Roles = "Admin,SalonManager")]
    public async Task<ActionResult<ReviewResponse>> Reply(int id, [FromBody] ReviewReplyRequest request)
    {
        return Ok(await _service.ReplyAsync(id, request, _userAccessor.UserId));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.RemoveAsync(id, _userAccessor.UserId);
        return NoContent();
    }
}
