using CutCal.Model.Responses;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("Salons/{salonId:int}/Availability")]
public class AvailabilityController : ControllerBase
{
    private const int DefaultDaysRange = 31;

    private readonly IAvailabilityService _service;

    public AvailabilityController(IAvailabilityService service)
    {
        _service = service;
    }

    [HttpGet("Days")]
    public async Task<ActionResult<List<AvailabilityDayResponse>>> GetDays(
        int salonId,
        [FromQuery] int serviceId,
        [FromQuery] int? staffId,
        [FromQuery] DateTime? from,
        [FromQuery] int? days,
        [FromQuery] DateTime? nowLocal)
    {
        var start = from ?? nowLocal ?? DateTime.UtcNow;
        return Ok(await _service.GetDaysAsync(salonId, serviceId, staffId, start, days ?? DefaultDaysRange, nowLocal));
    }

    [HttpGet("Slots")]
    public async Task<ActionResult<List<AvailabilitySlotResponse>>> GetSlots(
        int salonId,
        [FromQuery] int serviceId,
        [FromQuery] int? staffId,
        [FromQuery] DateTime date,
        [FromQuery] int? excludeAppointmentId,
        [FromQuery] DateTime? nowLocal)
    {
        return Ok(await _service.GetSlotsAsync(salonId, serviceId, staffId, date, excludeAppointmentId, nowLocal));
    }
}
