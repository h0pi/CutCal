using CutCal.Model.Responses;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize(Roles = "Admin,SalonManager")]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<object>> Get([FromQuery] int? salonId, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        AppointmentsReportResponse appointmentsReport = await _service.GetAppointmentsReportAsync(salonId, dateFrom, dateTo);
        ServicesReportResponse servicesReport = await _service.GetServicesReportAsync(salonId, null, null);
        return Ok(new { appointmentsReport, servicesReport });
    }

    [HttpGet("Pdf/Appointments")]
    public async Task<IActionResult> AppointmentsPdf([FromQuery] int? salonId, [FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo)
    {
        var pdf = await _service.GenerateAppointmentsPdfAsync(salonId, dateFrom, dateTo);
        return File(pdf, "application/pdf", "appointments-report.pdf");
    }

    [HttpGet("Pdf/Services")]
    public async Task<IActionResult> ServicesPdf([FromQuery] int? salonId, [FromQuery] int? month, [FromQuery] int? year)
    {
        var pdf = await _service.GenerateServicesPdfAsync(salonId, month, year);
        return File(pdf, "application/pdf", "services-report.pdf");
    }
}
