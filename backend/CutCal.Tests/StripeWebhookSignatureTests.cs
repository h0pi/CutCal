using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CutCal.Model.Exceptions;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

/// <summary>The real signature check of the Stripe library, run offline with a signature made the way Stripe makes it.</summary>
public class StripeWebhookSignatureTests
{
    private const string Secret = "whsec_test_secret";

    // An event from an API version older than the library's, to show that only the signature decides.
    private const string CheckoutCompleted =
        """{"id":"evt_1","object":"event","api_version":"2020-08-27","created":1700000000,"livemode":false,"type":"checkout.session.completed","data":{"object":{"id":"cs_test_1","object":"checkout.session","client_reference_id":"6003","amount_total":2000,"currency":"usd","payment_status":"paid","status":"complete","payment_intent":"pi_test_1"}}}""";

    private const string PaymentIntentCreated =
        """{"id":"evt_2","object":"event","api_version":"2020-08-27","created":1700000000,"livemode":false,"type":"payment_intent.created","data":{"object":{"id":"pi_1","object":"payment_intent"}}}""";

    public StripeWebhookSignatureTests()
    {
        Environment.SetEnvironmentVariable("Stripe__WebhookSecret", Secret);
    }

    private static StripeGateway Gateway() => new(NullLogger<StripeGateway>.Instance);

    private static string Sign(string payload, string secret = Secret, long? timestamp = null)
    {
        var time = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes($"{time}.{payload}"));
        return $"t={time},v1={Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    [Fact]
    public void Reads_a_correctly_signed_checkout_event_including_the_session()
    {
        var webhook = Gateway().ReadWebhookEvent(CheckoutCompleted, Sign(CheckoutCompleted));

        Assert.Equal(StripeWebhookEventTypes.CheckoutSessionCompleted, webhook.Type);
        var session = webhook.Session!;
        Assert.Equal("cs_test_1", session.Id);
        Assert.Equal("6003", session.ClientReferenceId);
        Assert.Equal(2000, session.AmountTotal);
        Assert.Equal(StripePaymentStatuses.Paid, session.PaymentStatus);
        Assert.Equal("pi_test_1", session.PaymentIntentId);
    }

    [Fact]
    public void Reads_a_signed_event_that_is_not_about_a_session_without_a_session()
    {
        var webhook = Gateway().ReadWebhookEvent(PaymentIntentCreated, Sign(PaymentIntentCreated));

        Assert.Equal("payment_intent.created", webhook.Type);
        Assert.Null(webhook.Session);
    }

    [Fact]
    public void Rejects_a_body_that_was_changed_after_it_was_signed()
    {
        var signature = Sign(CheckoutCompleted);

        Assert.Throws<ClientException>(() => Gateway().ReadWebhookEvent(CheckoutCompleted.Replace("2000", "1"), signature));
    }

    [Fact]
    public void Rejects_a_signature_made_with_another_secret()
    {
        Assert.Throws<ClientException>(() => Gateway().ReadWebhookEvent(CheckoutCompleted, Sign(CheckoutCompleted, secret: "whsec_someone_else")));
    }

    [Fact]
    public void Rejects_an_old_signature_so_a_captured_call_cannot_be_replayed()
    {
        var anHourAgo = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds();

        Assert.Throws<ClientException>(() => Gateway().ReadWebhookEvent(CheckoutCompleted, Sign(CheckoutCompleted, timestamp: anHourAgo)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("garbage")]
    public void Rejects_a_missing_or_malformed_signature(string header)
    {
        Assert.Throws<ClientException>(() => Gateway().ReadWebhookEvent(CheckoutCompleted, header));
    }

    [Fact]
    public void A_missing_webhook_secret_is_a_server_configuration_error_not_a_bad_request()
    {
        Environment.SetEnvironmentVariable("Stripe__WebhookSecret", null);

        Assert.Throws<InvalidOperationException>(() => Gateway().ReadWebhookEvent(CheckoutCompleted, Sign(CheckoutCompleted)));
    }

    [Fact]
    public void The_webhook_is_the_only_payments_action_open_to_callers_that_are_not_signed_in()
    {
        var controller = typeof(PaymentsController);
        var anonymous = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.GetCustomAttribute<AllowAnonymousAttribute>() is not null)
            .Select(x => x.Name);

        Assert.NotNull(controller.GetCustomAttribute<AuthorizeAttribute>());
        Assert.Equal(new[] { nameof(PaymentsController.StripeWebhook) }, anonymous);
        Assert.NotNull(controller.GetMethod(nameof(PaymentsController.StripeWebhook))!.GetCustomAttribute<HttpPostAttribute>());
    }
}
