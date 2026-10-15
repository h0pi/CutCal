using CutCal.Model.Exceptions;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace CutCal.Services.Services;

public static class StripeSessionStatuses
{
    public const string Open = "open";
}

public static class StripePaymentStatuses
{
    public const string Paid = "paid";
}

public static class StripeWebhookEventTypes
{
    public const string CheckoutSessionCompleted = "checkout.session.completed";
}

/// <summary>What the payment code needs to know about a Stripe Checkout session.</summary>
public sealed record StripeCheckoutSession(
    string Id, string? Url, string Status, string PaymentStatus, long? AmountTotal, string? Currency, string? PaymentIntentId, string? ClientReferenceId);

public sealed record StripeCheckoutRequest(
    int AppointmentId, string ProductName, long AmountInMinorUnits, string Currency, string SuccessUrl, string CancelUrl);

/// <summary>An event Stripe sent us; <see cref="Session"/> is set only for events about a Checkout session.</summary>
public sealed record StripeWebhookEvent(string Type, StripeCheckoutSession? Session);

/// <summary>The only place that talks to Stripe, so the payment rules can be tested without the network.</summary>
public interface IStripeGateway
{
    Task<StripeCheckoutSession> CreateCheckoutSessionAsync(StripeCheckoutRequest request);
    Task<StripeCheckoutSession> GetCheckoutSessionAsync(string sessionId);

    /// <summary>Reads a webhook call, but only when its signature proves it came from Stripe; throws <see cref="ClientException"/> otherwise.</summary>
    StripeWebhookEvent ReadWebhookEvent(string payload, string signatureHeader);
}

/// <summary>
/// The secret key and the webhook secret are read once from the environment (.env) but only checked when they are first
/// needed, so a missing value breaks card payments and nothing else.
/// </summary>
public sealed class StripeGateway : IStripeGateway
{
    private readonly string? _secretKey = Environment.GetEnvironmentVariable("Stripe__SecretKey");
    private readonly string? _webhookSecret = Environment.GetEnvironmentVariable("Stripe__WebhookSecret");
    private readonly ILogger<StripeGateway> _logger;
    private StripeClient? _client;

    public StripeGateway(ILogger<StripeGateway> logger)
    {
        _logger = logger;
    }

    private StripeClient Client => _client ??= new StripeClient(
        string.IsNullOrWhiteSpace(_secretKey) ? throw new InvalidOperationException("Stripe__SecretKey is not configured. Set it in .env.") : _secretKey);

    public Task<StripeCheckoutSession> CreateCheckoutSessionAsync(StripeCheckoutRequest request)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            // Charge exactly the appointment's price in its own currency; otherwise Stripe may offer the customer a converted
            // amount in their local currency, which would never match the price the server expects.
            AdaptivePricing = new SessionAdaptivePricingOptions { Enabled = false },
            ClientReferenceId = request.AppointmentId.ToString(),
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency,
                        UnitAmount = request.AmountInMinorUnits,
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = request.ProductName }
                    }
                }
            }
        };
        return CallAsync(() => new SessionService(Client).CreateAsync(options));
    }

    public Task<StripeCheckoutSession> GetCheckoutSessionAsync(string sessionId) =>
        CallAsync(() => new SessionService(Client).GetAsync(sessionId));

    public StripeWebhookEvent ReadWebhookEvent(string payload, string signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_webhookSecret))
        {
            throw new InvalidOperationException("Stripe__WebhookSecret is not configured. Set it in .env.");
        }

        try
        {
            // Only the signature proves the call is from Stripe; the event's API version may differ from the library's.
            var stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _webhookSecret, throwOnApiVersionMismatch: false);
            return new StripeWebhookEvent(stripeEvent.Type, stripeEvent.Data.Object is Session session ? ToCheckoutSession(session) : null);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning("A Stripe webhook call was rejected: {Reason}", ex.Message);
            throw new ClientException("The Stripe signature is not valid.");
        }
    }

    private static StripeCheckoutSession ToCheckoutSession(Session session) => new(
        session.Id, session.Url, session.Status, session.PaymentStatus, session.AmountTotal, session.Currency, session.PaymentIntentId, session.ClientReferenceId);

    private async Task<StripeCheckoutSession> CallAsync(Func<Task<Session>> call)
    {
        try
        {
            return ToCheckoutSession(await call());
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe request failed.");
            throw new ServiceUnavailableException("Stripe could not process the request. Please try again shortly.");
        }
    }
}
