using CutCal.Model.Constants;
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

    [HttpPost("CreateStripeSession")]
    public async Task<ActionResult<CreateCheckoutSessionResponse>> CreateStripeSession([FromBody] StripePaymentRequest request)
    {
        return Ok(await _service.CreateStripeSessionAsync(request.AppointmentId));
    }

    [HttpPost("ConfirmStripePayment")]
    public async Task<ActionResult<PaymentResponse>> ConfirmStripePayment([FromBody] StripePaymentRequest request)
    {
        return Ok(await _service.ConfirmStripePaymentAsync(request.AppointmentId));
    }

    /// <summary>
    /// Called by Stripe, not by a signed-in user, so it cannot ask for a login: the signature in the header, checked against
    /// the webhook secret, is what proves the call is genuine. The signature covers the exact bytes, so the body is read raw.
    /// </summary>
    [HttpPost("StripeWebhook")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        await _service.HandleStripeWebhookAsync(await reader.ReadToEndAsync(), Request.Headers["Stripe-Signature"].ToString());
        return Ok();
    }

    [HttpPost("Refund/{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrManager)]
    public async Task<ActionResult<PaymentResponse>> Refund(int id)
    {
        return Ok(await _service.RefundAsync(id));
    }
}
