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
    Task<CreateCheckoutSessionResponse> CreateStripeSessionAsync(int appointmentId);
    Task<PaymentResponse> ConfirmStripePaymentAsync(int appointmentId);
    Task HandleStripeWebhookAsync(string payload, string signatureHeader);
    Task<PaymentResponse> RefundAsync(int appointmentId);
}

public class PaymentService : IPaymentService
{
    private const string PaymentCurrency = "USD";
    private const int MinorUnitsPerCurrencyUnit = 100;
    private const string AlreadyPaidMessage = "This appointment has already been paid for.";

    // The mobile app's WebView watches for navigation to these addresses to know the customer has finished
    // (or left) Stripe's payment page; nothing is ever served from them.
    private const string StripeSuccessUrl = "https://cutcal.app/stripe/success";
    private const string StripeCancelUrl = "https://cutcal.app/stripe/cancel";

    private readonly CutCalDbContext _context;
    private readonly IRabbitMqPublisher _publisher;
    private readonly INotificationService _notificationService;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IStripeGateway _stripe;
    private readonly ILogger<PaymentService> _logger;
    private readonly PayPalHttpClient _client;

    public PaymentService(
        CutCalDbContext context,
        IRabbitMqPublisher publisher,
        INotificationService notificationService,
        IAuthenticatedUserAccessor userAccessor,
        IStripeGateway stripe,
        ILogger<PaymentService> logger)
    {
        _context = context;
        _publisher = publisher;
        _notificationService = notificationService;
        _userAccessor = userAccessor;
        _stripe = stripe;
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
            throw new ClientException(AlreadyPaidMessage);
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

            await NotifyPaymentReceivedAsync(appointment, payment);

            return MapPaymentResponse(payment);
        }
        catch (HttpException ex)
        {
            _logger.LogError(ex, "PayPal CaptureOrder failed for appointment {AppointmentId}", appointmentId);
            throw new ClientException("Unable to capture PayPal payment.");
        }
    }

    /// <summary>
    /// Starts card payment for the appointment. The amount is the appointment's own price, never something the client sends.
    /// While a Stripe session for it is still open the same one is returned, so the same visit cannot be paid twice.
    /// </summary>
    public async Task<CreateCheckoutSessionResponse> CreateStripeSessionAsync(int appointmentId)
    {
        var appointment = await LoadAppointmentForPaymentAsync(appointmentId);
        if (appointment.Payment is { Status: PaymentStatusNames.Paid })
        {
            throw new ClientException(AlreadyPaidMessage);
        }
        if (appointment.PaymentMethod != PaymentMethodNames.Stripe)
        {
            throw new ClientException("This appointment is not set to be paid by card.");
        }
        if (appointment.StateName == AppointmentStateNames.Cancelled)
        {
            throw new ClientException("A cancelled appointment cannot be paid.");
        }

        if (appointment.Payment?.StripeSessionId is { } existingSessionId)
        {
            var existing = await _stripe.GetCheckoutSessionAsync(existingSessionId);
            if (existing.PaymentStatus == StripePaymentStatuses.Paid)
            {
                // The customer paid but the app never confirmed (for example it was closed first): record it now.
                await RecordStripePaymentAsync(appointment, appointment.Payment, existing);
                throw new ClientException(AlreadyPaidMessage);
            }
            if (existing.Status == StripeSessionStatuses.Open)
            {
                return new CreateCheckoutSessionResponse { SessionId = existing.Id, CheckoutUrl = existing.Url! };
            }
        }

        var session = await _stripe.CreateCheckoutSessionAsync(new StripeCheckoutRequest(
            appointmentId,
            $"{appointment.Service.Name} at {appointment.Salon.Name}",
            ToMinorUnits(appointment.Price),
            PaymentCurrency.ToLowerInvariant(),
            StripeSuccessUrl,
            StripeCancelUrl));

        var payment = appointment.Payment ?? new Payment { AppointmentId = appointmentId, CreatedAt = DateTime.UtcNow };
        payment.StripeSessionId = session.Id;
        payment.Amount = appointment.Price;
        payment.Currency = PaymentCurrency;
        payment.Status = PaymentStatusNames.Created;
        if (appointment.Payment is null)
        {
            _context.Payments.Add(payment);
        }
        await _context.SaveChangesAsync();

        return new CreateCheckoutSessionResponse { SessionId = session.Id, CheckoutUrl = session.Url! };
    }

    /// <summary>
    /// Finishes card payment by asking Stripe itself whether the session was paid; the client only says which appointment.
    /// Calling it again for a paid appointment changes and sends nothing.
    /// </summary>
    public async Task<PaymentResponse> ConfirmStripePaymentAsync(int appointmentId)
    {
        var appointment = await LoadAppointmentForPaymentAsync(appointmentId);
        if (appointment.Payment is { Status: PaymentStatusNames.Paid } paid)
        {
            return MapPaymentResponse(paid);
        }

        var payment = appointment.Payment;
        if (payment?.StripeSessionId is null)
        {
            throw new ClientException("No card payment was started for this appointment.");
        }

        var session = await _stripe.GetCheckoutSessionAsync(payment.StripeSessionId);
        if (session.PaymentStatus != StripePaymentStatuses.Paid)
        {
            throw new ClientException("The card payment has not been completed.");
        }

        return await RecordStripePaymentAsync(appointment, payment, session);
    }

    /// <summary>
    /// Stripe tells us on its own that a card payment went through, which covers the customer closing the app before it could
    /// confirm. Only a call signed by Stripe is believed, and it is recorded exactly like a confirmation (so never twice).
    /// Events that are not about one of our unpaid card payments are acknowledged and ignored.
    /// </summary>
    public async Task HandleStripeWebhookAsync(string payload, string signatureHeader)
    {
        var webhook = _stripe.ReadWebhookEvent(payload, signatureHeader);
        if (webhook is not { Type: StripeWebhookEventTypes.CheckoutSessionCompleted, Session: { } session })
        {
            return;
        }

        var appointment = int.TryParse(session.ClientReferenceId, out var appointmentId)
            ? await AppointmentsWithPayment().FirstOrDefaultAsync(x => x.Id == appointmentId)
            : null;
        if (appointment?.Payment is not { } payment
            || payment.StripeSessionId != session.Id
            || payment.Status == PaymentStatusNames.Paid
            || session.PaymentStatus != StripePaymentStatuses.Paid)
        {
            _logger.LogInformation("Ignored the Stripe event for session {SessionId}: it is not a new paid card payment of ours.", session.Id);
            return;
        }

        await RecordStripePaymentAsync(appointment, payment, session);
    }

    private IQueryable<Appointment> AppointmentsWithPayment() =>
        _context.Appointments.Include(x => x.Payment).Include(x => x.Salon).Include(x => x.Service).Include(x => x.Customer);

    private async Task<Appointment> LoadAppointmentForPaymentAsync(int appointmentId)
    {
        var appointment = await AppointmentsWithPayment().FirstOrDefaultAsync(x => x.Id == appointmentId)
            ?? throw new ClientException("Appointment not found.");
        OwnershipGuard.EnsureIsSelfOrAdmin(appointment.CustomerId, _userAccessor);
        return appointment;
    }

    private async Task<PaymentResponse> RecordStripePaymentAsync(Appointment appointment, Payment payment, StripeCheckoutSession session)
    {
        var matches = session.ClientReferenceId == appointment.Id.ToString()
            && session.AmountTotal == ToMinorUnits(payment.Amount)
            && string.Equals(session.Currency, PaymentCurrency, StringComparison.OrdinalIgnoreCase);
        if (!matches)
        {
            _logger.LogWarning("Stripe session {SessionId} does not match appointment {AppointmentId}.", session.Id, appointment.Id);
            throw new ClientException("The card payment does not match this appointment.");
        }

        payment.StripePaymentIntentId = session.PaymentIntentId;
        payment.Status = PaymentStatusNames.Paid;
        appointment.PaymentStatus = PaymentStatusNames.Paid;

        await NotifyPaymentReceivedAsync(appointment, payment);

        return MapPaymentResponse(payment);
    }

    private async Task NotifyPaymentReceivedAsync(Appointment appointment, Payment payment)
    {
        _notificationService.Add(appointment.CustomerId, "Payment received",
            $"Your payment of {payment.Amount:C} for the appointment at {appointment.Salon.Name} was received.",
            nameof(CutCal.Model.Enums.NotificationType.PaymentReceived));
        await _context.SaveChangesAsync();

        await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.PaymentReceived, appointment, payment.Amount));
    }

    private static long ToMinorUnits(decimal amount) => (long)Math.Round(amount * MinorUnitsPerCurrencyUnit, MidpointRounding.AwayFromZero);

    public async Task<PaymentResponse> RefundAsync(int appointmentId)
    {
        var appointment = await _context.Appointments.Include(x => x.Payment).Include(x => x.Salon).Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == appointmentId)
            ?? throw new ClientException("Appointment not found.");
        var payment = appointment.Payment ?? throw new ClientException("No payment found for this appointment.");
        if (payment.Status != PaymentStatusNames.Paid)
        {
            throw new ClientException("Only paid appointments can be refunded.");
        }

        if (payment.StripePaymentIntentId is not null)
        {
            await _stripe.RefundAsync(payment.StripePaymentIntentId, ToMinorUnits(payment.Amount));
        }
        else
        {
            await RefundPayPalAsync(payment);
        }

        payment.Status = PaymentStatusNames.Refunded;
        appointment.PaymentStatus = PaymentStatusNames.Refunded;

        _notificationService.Add(appointment.CustomerId, "Payment refunded",
            $"Your payment of {payment.Amount:C} for the appointment at {appointment.Salon.Name} was refunded.",
            nameof(CutCal.Model.Enums.NotificationType.PaymentRefunded));
        await _context.SaveChangesAsync();

        await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.PaymentRefunded, appointment, payment.Amount));

        return MapPaymentResponse(payment);
    }

    private async Task RefundPayPalAsync(Payment payment)
    {
        var request = new CapturesRefundRequest(payment.PaypalCaptureId);
        request.RequestBody(new PayPalCheckoutSdk.Payments.RefundRequest
        {
            Amount = new PayPalCheckoutSdk.Payments.Money { CurrencyCode = payment.Currency, Value = payment.Amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) }
        });

        try
        {
            await _client.Execute(request);
        }
        catch (HttpException ex)
        {
            _logger.LogError(ex, "PayPal Refund failed for appointment {AppointmentId}", payment.AppointmentId);
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
