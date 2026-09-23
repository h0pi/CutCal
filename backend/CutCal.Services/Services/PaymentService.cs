using CutCal.Messaging;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Database;
using CutCal.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;

namespace CutCal.Services.Services;

public interface IPaymentService
{
    Task<CreateOrderResponse> CreateOrderAsync(int appointmentId);
    Task<PaymentResponse> CaptureOrderAsync(string paypalOrderId, int appointmentId);
    Task<PaymentResponse> RefundAsync(int appointmentId);
}

public class PaymentService : IPaymentService
{
    private readonly CutCalDbContext _context;
    private readonly IRabbitMqPublisher _publisher;
    private readonly INotificationService _notificationService;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly ILogger<PaymentService> _logger;
    private readonly PayPalHttpClient _client;

    public PaymentService(
        CutCalDbContext context,
        IRabbitMqPublisher publisher,
        INotificationService notificationService,
        IAuthenticatedUserAccessor userAccessor,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _publisher = publisher;
        _notificationService = notificationService;
        _userAccessor = userAccessor;
        _logger = logger;

        var clientId = System.Environment.GetEnvironmentVariable("PayPal__ClientId");
        var clientSecret = System.Environment.GetEnvironmentVariable("PayPal__ClientSecret");
        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new InvalidOperationException("PayPal__ClientId / PayPal__ClientSecret are not configured. Set them in .env.");
        }
        PayPalEnvironment environment = new SandboxEnvironment(clientId, clientSecret);
        _client = new PayPalHttpClient(environment);
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.Include(x => x.Payment).FirstOrDefaultAsync(x => x.Id == appointmentId)
            ?? throw new ClientException("Appointment not found.");
        OwnershipGuard.EnsureIsSelfOrAdmin(appointment.CustomerId, _userAccessor);

        if (appointment.Payment is { Status: PaymentStatusNames.Paid })
        {
            throw new ClientException("This appointment has already been paid for.");
        }

        var orderRequest = new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new()
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = appointment.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                    },
                    ReferenceId = appointmentId.ToString()
                }
            },
            // The mobile app's WebView watches for navigation to these URLs to know when the
            // customer has approved or cancelled the payment on PayPal's own approval page.
            ApplicationContext = new ApplicationContext
            {
                BrandName = "CutCal",
                UserAction = "PAY_NOW",
                ReturnUrl = "https://cutcal.app/paypal/return",
                CancelUrl = "https://cutcal.app/paypal/cancel"
            }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(orderRequest);

        try
        {
            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            var approvalUrl = result.Links?.FirstOrDefault(l => l.Rel == "approve")?.Href ?? string.Empty;

            var payment = appointment.Payment ?? new Payment { AppointmentId = appointmentId, CreatedAt = DateTime.UtcNow };
            payment.PaypalOrderId = result.Id;
            payment.Amount = appointment.Price;
            payment.Currency = "USD";
            payment.Status = PaymentStatusNames.Created;
            if (appointment.Payment is null)
            {
                _context.Payments.Add(payment);
            }
            appointment.PaypalOrderId = result.Id;
            await _context.SaveChangesAsync();

            return new CreateOrderResponse { OrderId = result.Id, ApprovalUrl = approvalUrl };
        }
        catch (HttpException ex)
        {
            _logger.LogError(ex, "PayPal CreateOrder failed for appointment {AppointmentId}", appointmentId);
            throw new ClientException("Unable to create PayPal order.");
        }
    }

    public async Task<PaymentResponse> CaptureOrderAsync(string paypalOrderId, int appointmentId)
    {
        var appointment = await _context.Appointments.Include(x => x.Payment).Include(x => x.Salon).Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == appointmentId)
            ?? throw new ClientException("Appointment not found.");
        OwnershipGuard.EnsureIsSelfOrAdmin(appointment.CustomerId, _userAccessor);

        if (appointment.Payment is { Status: PaymentStatusNames.Paid })
        {
            return MapPaymentResponse(appointment.Payment);
        }

        if (appointment.PaypalOrderId != paypalOrderId)
        {
            throw new ClientException("This PayPal order does not belong to this appointment.");
        }

        var request = new OrdersCaptureRequest(paypalOrderId);
        request.RequestBody(new OrderActionRequest());

        try
        {
            var response = await _client.Execute(request);
            var result = response.Result<Order>();
            var captureId = result.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault()?.Id;

            var payment = appointment.Payment ?? new Payment { AppointmentId = appointmentId, CreatedAt = DateTime.UtcNow };
            payment.PaypalOrderId = paypalOrderId;
            payment.PaypalCaptureId = captureId;
            payment.Amount = appointment.Price;
            payment.Currency = "USD";
            payment.Status = PaymentStatusNames.Paid;
            if (appointment.Payment is null)
            {
                _context.Payments.Add(payment);
            }

            appointment.PaypalCaptureId = captureId;
            appointment.PaymentStatus = PaymentStatusNames.Paid;

            _notificationService.Add(appointment.CustomerId, "Payment received",
                $"Your payment of {payment.Amount:C} for the appointment at {appointment.Salon.Name} was received.",
                nameof(CutCal.Model.Enums.NotificationType.PaymentReceived));
            await _context.SaveChangesAsync();

            await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.PaymentReceived, appointment, payment.Amount));

            return MapPaymentResponse(payment);
        }
        catch (HttpException ex)
        {
            _logger.LogError(ex, "PayPal CaptureOrder failed for appointment {AppointmentId}", appointmentId);
            throw new ClientException("Unable to capture PayPal payment.");
        }
    }

    public async Task<PaymentResponse> RefundAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.Include(x => x.Payment).Include(x => x.Salon).Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == appointmentId)
            ?? throw new ClientException("Appointment not found.");
        var payment = appointment.Payment ?? throw new ClientException("No payment found for this appointment.");
        if (payment.Status != PaymentStatusNames.Paid)
        {
            throw new ClientException("Only paid appointments can be refunded.");
        }

        var request = new CapturesRefundRequest(payment.PaypalCaptureId);
        request.RequestBody(new PayPalCheckoutSdk.Payments.RefundRequest
        {
            Amount = new PayPalCheckoutSdk.Payments.Money { CurrencyCode = payment.Currency, Value = payment.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) }
        });

        try
        {
            await _client.Execute(request);

            payment.Status = PaymentStatusNames.Refunded;
            appointment.PaymentStatus = PaymentStatusNames.Refunded;

            _notificationService.Add(appointment.CustomerId, "Payment refunded",
                $"Your payment of {payment.Amount:C} for the appointment at {appointment.Salon.Name} was refunded.",
                nameof(CutCal.Model.Enums.NotificationType.PaymentRefunded));
            await _context.SaveChangesAsync();

            await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.PaymentRefunded, appointment, payment.Amount));

            return MapPaymentResponse(payment);
        }
        catch (HttpException ex)
        {
            _logger.LogError(ex, "PayPal Refund failed for appointment {AppointmentId}", appointmentId);
            throw new ClientException("Unable to refund PayPal payment.");
        }
    }

    private static NotificationMessage BuildMessage(string type, Appointment appointment, decimal amount) => new()
    {
        Type = type,
        AppointmentId = appointment.Id,
        CustomerEmail = appointment.Customer.Email,
        CustomerName = appointment.Customer.FirstName,
        SalonName = appointment.Salon.Name,
        ScheduledAt = appointment.ScheduledAt,
        Amount = amount
    };

    private static PaymentResponse MapPaymentResponse(Payment payment) => new()
    {
        Id = payment.Id,
        AppointmentId = payment.AppointmentId,
        PaypalOrderId = payment.PaypalOrderId,
        PaypalCaptureId = payment.PaypalCaptureId,
        Amount = payment.Amount,
        Currency = payment.Currency,
        Status = payment.Status,
        CreatedAt = payment.CreatedAt
    };
}
