using CutCal.Model.Common;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

// TODO: replace polling with a SignalR NotificationHub for true real-time push.
[ApiController]
[Authorize]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public NotificationsController(INotificationService service, IAuthenticatedUserAccessor userAccessor)
    {
        _service = service;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<NotificationResponse>>> Get([FromQuery] NotificationSearchObject search)
    {
        return Ok(await _service.GetForUserAsync(_userAccessor.UserId, search));
    }

    [HttpPut("{id:int}/MarkRead")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _service.MarkReadAsync(id, _userAccessor.UserId);
        return NoContent();
    }

    [HttpPut("MarkAllRead")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _service.MarkAllReadAsync(_userAccessor.UserId);
        return NoContent();
    }
}
