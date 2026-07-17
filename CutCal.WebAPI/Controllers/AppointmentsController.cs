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
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public AppointmentsController(IAppointmentService service, IAuthenticatedUserAccessor userAccessor)
    {
        _service = service;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<AppointmentResponse>>> Get([FromQuery] AppointmentSearchObject search)
    {
        return Ok(await _service.GetPagedAsync(search));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<AppointmentResponse>> Insert([FromBody] AppointmentInsertRequest request)
    {
        return Ok(await _service.InsertAsync(request, _userAccessor.UserId));
    }

    [HttpPut("{id:int}/Confirm")]
    [Authorize(Roles = "Admin,SalonManager")]
    public async Task<ActionResult<AppointmentResponse>> Confirm(int id)
    {
        return Ok(await _service.ConfirmAsync(id, _userAccessor.UserId));
    }

    [HttpPut("{id:int}/Cancel")]
    public async Task<ActionResult<AppointmentResponse>> Cancel(int id, [FromBody] AppointmentCancelRequest request)
    {
        return Ok(await _service.CancelAsync(id, request.Reason, _userAccessor.UserId));
    }

    [HttpPut("{id:int}/Complete")]
    [Authorize(Roles = "Admin,SalonManager,Staff")]
    public async Task<ActionResult<AppointmentResponse>> Complete(int id)
    {
        return Ok(await _service.CompleteAsync(id));
    }
}
