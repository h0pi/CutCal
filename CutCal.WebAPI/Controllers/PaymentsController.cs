using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpPost("CreateOrder")]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        return Ok(await _service.CreateOrderAsync(request.AppointmentId));
    }

    [HttpPost("CaptureOrder")]
    public async Task<ActionResult<PaymentResponse>> CaptureOrder([FromBody] CaptureOrderRequest request)
    {
        return Ok(await _service.CaptureOrderAsync(request.PaypalOrderId, request.AppointmentId));
    }

    [HttpPost("Refund/{id:int}")]
    [Authorize(Roles = "Admin,SalonManager")]
    public async Task<ActionResult<PaymentResponse>> Refund(int id)
    {
        return Ok(await _service.RefundAsync(id));
    }
}
