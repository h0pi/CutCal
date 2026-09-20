using CutCal.Model.Responses;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Admin,SalonManager")]
[Route("[controller]")]
public class GeocodingController : ControllerBase
{
    private readonly IGeocodingService _service;

    public GeocodingController(IGeocodingService service)
    {
        _service = service;
    }

    [HttpGet("Search")]
    public async Task<ActionResult<List<GeocodeResultResponse>>> Search([FromQuery] string query)
    {
        return Ok(await _service.SearchAsync(query));
    }
}
