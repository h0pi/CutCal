using CutCal.Messaging;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Services.Database;
using CutCal.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class StripePaymentServiceTests : IDisposable
{
    private const int AppointmentId = 1;
    private const decimal Price = 45.50m;
    private const decimal Tax = 7.74m; // 17% of the price
    private const decimal Total = Price + Tax;
    private const long TotalInCents = 5324;
    private const string AuthenticSignature = "signed-by-stripe";

    private sealed class FakeStripeGateway : IStripeGateway
    {
        public List<StripeCheckoutRequest> Created { get; } = new();
        private readonly Dictionary<string, StripeCheckoutSession> _sessions = new();

        public Task<StripeCheckoutSession> CreateCheckoutSessionAsync(StripeCheckoutRequest request)
        {
            Created.Add(request);
            var session = new StripeCheckoutSession(
                $"cs_test_{Created.Count}", $"https://checkout.stripe.test/{Created.Count}", StripeSessionStatuses.Open, "unpaid",
                request.AmountInMinorUnits, request.Currency, null, request.AppointmentId.ToString());
            _sessions[session.Id] = session;
            return Task.FromResult(session);
        }

        public Task<StripeCheckoutSession> GetCheckoutSessionAsync(string sessionId) => Task.FromResult(_sessions[sessionId]);

        public List<string> Expired { get; } = new();

        public Task ExpireCheckoutSessionAsync(string sessionId)
        {
            Expired.Add(sessionId);
            Change(sessionId, x => x with { Status = "expired" });
            return Task.CompletedTask;
        }

        public StripeCheckoutSession Session(string sessionId) => _sessions[sessionId];

        public List<(string PaymentIntentId, long Amount)> Refunds { get; } = new();
        public bool RefundFails { get; set; }

        public Task RefundAsync(string paymentIntentId, long amountInMinorUnits)
        {
            if (RefundFails)
            {
                throw new ServiceUnavailableException("Stripe could not refund the payment. Please try again shortly.");
            }

            Refunds.Add((paymentIntentId, amountInMinorUnits));
            return Task.CompletedTask;
        }

        public StripeWebhookEvent? NextWebhookEvent { get; set; }

        public StripeWebhookEvent ReadWebhookEvent(string payload, string signatureHeader) =>
            signatureHeader == AuthenticSignature ? NextWebhookEvent! : throw new ClientException("The Stripe signature is not valid.");

        public void Change(string sessionId, Func<StripeCheckoutSession, StripeCheckoutSession> change) => _sessions[sessionId] = change(_sessions[sessionId]);

        public void Pay(string sessionId) =>
            Change(sessionId, s => s with { Status = "complete", PaymentStatus = StripePaymentStatuses.Paid, PaymentIntentId = "pi_test_1" });
    }

    private readonly CutCalDbContext _context;
    private readonly FakeStripeGateway _stripe = new();
    private readonly RecordingPublisher _publisher = new();
    private readonly int _customerId;

    public StripePaymentServiceTests()
    {
        // PaymentService also sets up its PayPal client, which only needs these two values to exist; nothing is sent to PayPal.
        Environment.SetEnvironmentVariable("PayPal__ClientId", "test-client-id");
        Environment.SetEnvironmentVariable("PayPal__ClientSecret", "test-client-secret");

        _context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Database.EnsureCreated();

        var appointment = _context.Appointments.Single(x => x.Id == AppointmentId);
        appointment.StateName = AppointmentStateNames.Confirmed;
        appointment.PaymentMethod = PaymentMethodNames.Stripe;
        appointment.PaymentStatus = PaymentStatusNames.Unpaid;
        appointment.Price = Price;
        _context.SaveChanges();
        _customerId = appointment.CustomerId;
    }

    public void Dispose() => _context.Dispose();

    private PaymentService ServiceFor(int userId, string role) =>
        new(_context, _publisher, new NotificationService(_context), new SignedInUser(userId, role), _stripe, NullLogger<PaymentService>.Instance);

    private PaymentService CustomerService() => ServiceFor(_customerId, RoleNames.Customer);

    private static PaymentQuoteRequest Quote(decimal? tipPercent = null, decimal? tipAmount = null) =>
        new() { AppointmentId = AppointmentId, TipPercent = tipPercent, TipAmount = tipAmount };

    private async Task<string> StartPaymentAsync() => (await CustomerService().CreateStripeSessionAsync(Quote())).SessionId;

    private Appointment Appointment => _context.Appointments.Include(x => x.Payment).Single(x => x.Id == AppointmentId);

    [Fact]
    public async Task Prices_the_session_from_the_appointment_and_remembers_it()
    {
        var response = await CustomerService().CreateStripeSessionAsync(Quote());

        var request = Assert.Single(_stripe.Created);
        Assert.Equal(TotalInCents, request.AmountInMinorUnits);
        Assert.Equal("usd", request.Currency);
        Assert.Equal(AppointmentId, request.AppointmentId);
        Assert.Equal("https://checkout.stripe.test/1", response.CheckoutUrl);
        var payment = Appointment.Payment!;
        Assert.Equal(response.SessionId, payment.StripeSessionId);
        Assert.Equal(PaymentStatusNames.Created, payment.Status);
        Assert.Equal(Total, payment.Amount);
        Assert.Equal("USD", payment.Currency);
    }

    [Fact]
    public async Task Hands_out_the_same_open_session_instead_of_starting_a_second_payment()
    {
        var first = await StartPaymentAsync();
        var second = await StartPaymentAsync();

        Assert.Equal(first, second);
        Assert.Single(_stripe.Created);
    }

    [Fact]
    public async Task Starts_a_new_session_when_the_previous_one_has_expired()
    {
        var first = await StartPaymentAsync();
        _stripe.Change(first, s => s with { Status = "expired" });

        var second = await StartPaymentAsync();

        Assert.NotEqual(first, second);
        Assert.Equal(second, Appointment.Payment!.StripeSessionId);
    }

    [Fact]
    public async Task Records_a_payment_that_was_made_but_never_confirmed_instead_of_charging_twice()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);

        var error = await Assert.ThrowsAsync<ClientException>(StartPaymentAsync);

        Assert.Equal("This appointment has already been paid for.", error.Message);
        Assert.Equal(PaymentStatusNames.Paid, Appointment.Payment!.Status);
        Assert.Single(_publisher.Sent);
    }

    [Fact]
    public async Task Refuses_to_start_a_payment_for_an_appointment_that_is_already_paid()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);
        await CustomerService().ConfirmStripePaymentAsync(AppointmentId);
        _stripe.Created.Clear();

        await Assert.ThrowsAsync<ClientException>(StartPaymentAsync);

        Assert.Empty(_stripe.Created);
    }

    [Theory]
    [InlineData(PaymentMethodNames.Cash)]
    [InlineData(PaymentMethodNames.PayPal)]
    public async Task Only_appointments_booked_for_card_payment_can_be_paid_by_card(string paymentMethod)
    {
        Appointment.PaymentMethod = paymentMethod;
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<ClientException>(StartPaymentAsync);

        Assert.Empty(_stripe.Created);
    }

    [Fact]
    public async Task A_cancelled_appointment_cannot_be_paid()
    {
        Appointment.StateName = AppointmentStateNames.Cancelled;
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<ClientException>(StartPaymentAsync);

        Assert.Empty(_stripe.Created);
    }

    [Fact]
    public async Task Only_the_customer_or_an_admin_can_start_a_payment()
    {
        await Assert.ThrowsAsync<ClientException>(() => ServiceFor(_customerId + 1, RoleNames.Customer).CreateStripeSessionAsync(Quote()));
        Assert.Empty(_stripe.Created);

        await ServiceFor(_customerId + 1, RoleNames.Admin).CreateStripeSessionAsync(Quote());
        Assert.Single(_stripe.Created);
    }

    [Fact]
    public async Task Confirming_asks_Stripe_then_marks_the_appointment_paid_and_tells_the_customer()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);

        var response = await CustomerService().ConfirmStripePaymentAsync(AppointmentId);

        Assert.Equal(PaymentStatusNames.Paid, response.Status);
        var appointment = Appointment;
        Assert.Equal(PaymentStatusNames.Paid, appointment.PaymentStatus);
        Assert.Equal("pi_test_1", appointment.Payment!.StripePaymentIntentId);
        Assert.Single(_context.Notifications.Where(x => x.UserId == _customerId && x.Type == nameof(CutCal.Model.Enums.NotificationType.PaymentReceived)));
        var message = Assert.Single(_publisher.Sent);
        Assert.Equal(NotificationMessageTypes.PaymentReceived, message.Type);
        Assert.Equal(Total, message.Amount);
    }

    [Fact]
    public async Task Confirming_twice_changes_and_sends_nothing_the_second_time()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);
        await CustomerService().ConfirmStripePaymentAsync(AppointmentId);

        var again = await CustomerService().ConfirmStripePaymentAsync(AppointmentId);

        Assert.Equal(PaymentStatusNames.Paid, again.Status);
        Assert.Single(_publisher.Sent);
        Assert.Single(_context.Notifications.Where(x => x.Type == nameof(CutCal.Model.Enums.NotificationType.PaymentReceived) && x.UserId == _customerId));
    }

    [Fact]
    public async Task Refuses_to_confirm_a_payment_Stripe_does_not_report_as_paid()
    {
        await StartPaymentAsync();

        await Assert.ThrowsAsync<ClientException>(() => CustomerService().ConfirmStripePaymentAsync(AppointmentId));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
        Assert.Empty(_publisher.Sent);
    }

    [Fact]
    public async Task Refuses_a_paid_session_whose_amount_is_not_the_appointments()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);
        _stripe.Change(session, s => s with { AmountTotal = 100 });

        await Assert.ThrowsAsync<ClientException>(() => CustomerService().ConfirmStripePaymentAsync(AppointmentId));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
        Assert.Empty(_publisher.Sent);
    }

    [Fact]
    public async Task Refuses_a_paid_session_that_was_made_for_another_appointment()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);
        _stripe.Change(session, s => s with { ClientReferenceId = "2" });

        await Assert.ThrowsAsync<ClientException>(() => CustomerService().ConfirmStripePaymentAsync(AppointmentId));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
    }

    [Fact]
    public async Task Cannot_confirm_a_payment_that_was_never_started()
    {
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().ConfirmStripePaymentAsync(AppointmentId));
    }

    [Fact]
    public async Task Only_the_customer_or_an_admin_can_confirm_a_payment()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);

        await Assert.ThrowsAsync<ClientException>(() => ServiceFor(_customerId + 1, RoleNames.Customer).ConfirmStripePaymentAsync(AppointmentId));
        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);

        await ServiceFor(_customerId + 1, RoleNames.Admin).ConfirmStripePaymentAsync(AppointmentId);
        Assert.Equal(PaymentStatusNames.Paid, Appointment.PaymentStatus);
    }

    private Task Webhook(StripeWebhookEvent webhookEvent, string signature = AuthenticSignature)
    {
        _stripe.NextWebhookEvent = webhookEvent;
        // Nobody is signed in on Stripe's behalf, so the caller here is just some other customer.
        return ServiceFor(_customerId + 1, RoleNames.Customer).HandleStripeWebhookAsync("{}", signature);
    }

    private StripeWebhookEvent Completed(StripeCheckoutSession session) => new(StripeWebhookEventTypes.CheckoutSessionCompleted, session);

    private async Task<StripeCheckoutSession> PaidSessionAsync()
    {
        var id = await StartPaymentAsync();
        _stripe.Pay(id);
        return _stripe.Session(id);
    }

    [Fact]
    public async Task A_webhook_for_a_paid_session_records_the_payment_without_anyone_being_signed_in()
    {
        await Webhook(Completed(await PaidSessionAsync()));

        Assert.Equal(PaymentStatusNames.Paid, Appointment.PaymentStatus);
        Assert.Equal("pi_test_1", Appointment.Payment!.StripePaymentIntentId);
        Assert.Single(_context.Notifications.Where(x => x.UserId == _customerId && x.Type == nameof(CutCal.Model.Enums.NotificationType.PaymentReceived)));
        Assert.Equal(NotificationMessageTypes.PaymentReceived, Assert.Single(_publisher.Sent).Type);
    }

    [Fact]
    public async Task The_same_webhook_twice_records_the_payment_once()
    {
        var webhookEvent = Completed(await PaidSessionAsync());

        await Webhook(webhookEvent);
        await Webhook(webhookEvent);

        Assert.Single(_publisher.Sent);
    }

    [Fact]
    public async Task A_webhook_after_the_app_already_confirmed_changes_nothing_more()
    {
        var session = await PaidSessionAsync();
        await CustomerService().ConfirmStripePaymentAsync(AppointmentId);

        await Webhook(Completed(session));

        Assert.Single(_publisher.Sent);
    }

    [Fact]
    public async Task Ignores_events_that_are_not_checkout_completions()
    {
        await PaidSessionAsync();

        await Webhook(new StripeWebhookEvent("payment_intent.created", null));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
    }

    [Fact]
    public async Task Ignores_a_session_that_is_not_the_one_stored_for_the_appointment()
    {
        var session = await PaidSessionAsync();

        await Webhook(Completed(session with { Id = "cs_test_someone_elses" }));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
        Assert.Empty(_publisher.Sent);
    }

    [Theory]
    [InlineData("999999")]
    [InlineData("not-a-number")]
    [InlineData(null)]
    public async Task Ignores_an_event_that_does_not_point_at_one_of_our_appointments(string? reference)
    {
        var session = await PaidSessionAsync();

        await Webhook(Completed(session with { ClientReferenceId = reference }));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
    }

    [Fact]
    public async Task Ignores_a_completed_session_that_is_not_paid_yet()
    {
        var id = await StartPaymentAsync();

        await Webhook(Completed(_stripe.Session(id)));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
    }

    [Fact]
    public async Task Refuses_a_webhook_that_is_not_signed_by_Stripe()
    {
        var session = await PaidSessionAsync();

        await Assert.ThrowsAsync<ClientException>(() => Webhook(Completed(session), signature: "forged"));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
        Assert.Empty(_publisher.Sent);
    }

    [Fact]
    public async Task Does_not_record_a_webhook_whose_amount_is_not_the_appointments()
    {
        var session = await PaidSessionAsync();

        await Assert.ThrowsAsync<ClientException>(() => Webhook(Completed(session with { AmountTotal = 100 })));

        Assert.Equal(PaymentStatusNames.Unpaid, Appointment.PaymentStatus);
        Assert.Empty(_publisher.Sent);
    }

    private async Task PayAsync()
    {
        var session = await StartPaymentAsync();
        _stripe.Pay(session);
        await CustomerService().ConfirmStripePaymentAsync(AppointmentId);
        _publisher.Sent.Clear();
    }

    private PaymentService AdminService() => ServiceFor(userId: 0, RoleNames.Admin);

    [Fact]
    public async Task Refunds_a_card_payment_through_Stripe_and_tells_the_customer()
    {
        await PayAsync();

        var response = await AdminService().RefundAsync(AppointmentId);

        var refund = Assert.Single(_stripe.Refunds);
        Assert.Equal(("pi_test_1", TotalInCents), refund);
        Assert.Equal(PaymentStatusNames.Refunded, response.Status);
        Assert.Equal(PaymentStatusNames.Refunded, Appointment.PaymentStatus);
        var message = Assert.Single(_publisher.Sent);
        Assert.Equal(NotificationMessageTypes.PaymentRefunded, message.Type);
        Assert.Equal(Total, message.Amount);
        Assert.Single(_context.Notifications.Where(x => x.UserId == _customerId && x.Type == nameof(CutCal.Model.Enums.NotificationType.PaymentRefunded)));
    }

    [Fact]
    public async Task Refunds_a_card_payment_only_once()
    {
        await PayAsync();
        await AdminService().RefundAsync(AppointmentId);

        await Assert.ThrowsAsync<ClientException>(() => AdminService().RefundAsync(AppointmentId));

        Assert.Single(_stripe.Refunds);
        Assert.Single(_publisher.Sent);
    }

    [Fact]
    public async Task Keeps_the_payment_paid_when_Stripe_cannot_refund_it()
    {
        await PayAsync();
        _stripe.RefundFails = true;

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => AdminService().RefundAsync(AppointmentId));

        Assert.Equal(PaymentStatusNames.Paid, Appointment.PaymentStatus);
        Assert.Equal(PaymentStatusNames.Paid, Appointment.Payment!.Status);
        Assert.Empty(_publisher.Sent);
    }

    [Fact]
    public async Task Does_not_refund_a_card_payment_that_was_never_paid()
    {
        await StartPaymentAsync();

        await Assert.ThrowsAsync<ClientException>(() => AdminService().RefundAsync(AppointmentId));

        Assert.Empty(_stripe.Refunds);
    }

    [Fact]
    public async Task A_manager_cannot_refund_a_payment_of_another_salon()
    {
        await PayAsync();
        var salon = _context.Salons.Single(x => x.Id == Appointment.SalonId);
        var otherManagerId = _context.Salons.First(x => x.OwnerId != salon.OwnerId).OwnerId;

        await Assert.ThrowsAsync<ClientException>(() => ServiceFor(otherManagerId, RoleNames.SalonManager).RefundAsync(AppointmentId));

        Assert.Empty(_stripe.Refunds);
        Assert.Equal(PaymentStatusNames.Paid, Appointment.Payment!.Status);
    }

    [Fact]
    public async Task A_manager_refunds_a_payment_of_their_own_salon()
    {
        await PayAsync();
        var salon = _context.Salons.Single(x => x.Id == Appointment.SalonId);

        await ServiceFor(salon.OwnerId, RoleNames.SalonManager).RefundAsync(AppointmentId);

        Assert.Single(_stripe.Refunds);
    }

    [Fact]
    public async Task The_refund_after_a_cancellation_does_not_need_a_manager()
    {
        await PayAsync();

        await CustomerService().RefundCancelledAppointmentAsync(AppointmentId);

        Assert.Single(_stripe.Refunds);
    }

    [Fact]
    public async Task A_paypal_order_is_refused_for_an_appointment_booked_with_another_method()
    {
        var error = await Assert.ThrowsAsync<ClientException>(() => CustomerService().CreateOrderAsync(Quote()));

        Assert.Equal("This appointment is not set to be paid with PayPal.", error.Message);
        Assert.Null(Appointment.Payment);
    }

    [Fact]
    public async Task A_paypal_order_is_refused_for_a_cancelled_appointment()
    {
        var appointment = _context.Appointments.Single(x => x.Id == AppointmentId);
        appointment.PaymentMethod = PaymentMethodNames.PayPal;
        appointment.StateName = AppointmentStateNames.Cancelled;
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() => CustomerService().CreateOrderAsync(Quote()));

        Assert.Equal("A cancelled appointment cannot be paid.", error.Message);
    }

    [Fact]
    public async Task A_paypal_capture_is_refused_for_a_cancelled_appointment_or_another_method()
    {
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().CaptureOrderAsync("order", AppointmentId));

        var appointment = _context.Appointments.Single(x => x.Id == AppointmentId);
        appointment.PaymentMethod = PaymentMethodNames.PayPal;
        appointment.StateName = AppointmentStateNames.Cancelled;
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() => CustomerService().CaptureOrderAsync("order", AppointmentId));

        Assert.Equal("A cancelled appointment cannot be paid.", error.Message);
    }

    [Fact]
    public async Task The_server_adds_tax_and_the_chosen_tip_to_the_price()
    {
        var byPercent = await CustomerService().GetQuoteAsync(Quote(tipPercent: 10));
        var byAmount = await CustomerService().GetQuoteAsync(Quote(tipAmount: 5m));
        var noTip = await CustomerService().GetQuoteAsync(Quote());

        Assert.Equal((Price, Tax, 4.55m, Price + Tax + 4.55m, "USD"), (byPercent.Subtotal, byPercent.Tax, byPercent.Tip, byPercent.Total, byPercent.Currency));
        Assert.Equal(Price + Tax + 5m, byAmount.Total);
        Assert.Equal((0m, Total), (noTip.Tip, noTip.Total));
    }

    [Fact]
    public async Task A_tip_that_is_negative_or_more_than_the_price_is_refused()
    {
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().GetQuoteAsync(Quote(tipAmount: -1m)));
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().GetQuoteAsync(Quote(tipPercent: -5)));
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().GetQuoteAsync(Quote(tipPercent: 101)));
        await Assert.ThrowsAsync<ClientException>(() => CustomerService().CreateStripeSessionAsync(Quote(tipAmount: Price + 1)));
        Assert.Empty(_stripe.Created);
    }

    [Fact]
    public async Task The_card_is_charged_the_total_and_the_tax_and_tip_are_stored()
    {
        var response = await CustomerService().CreateStripeSessionAsync(Quote(tipPercent: 10));

        var request = Assert.Single(_stripe.Created);
        Assert.Equal(5779, request.AmountInMinorUnits);
        var payment = Appointment.Payment!;
        Assert.Equal((Price + Tax + 4.55m, Tax, 4.55m), (payment.Amount, payment.TaxAmount, payment.TipAmount));
        Assert.Equal(response.SessionId, payment.StripeSessionId);
    }

    [Fact]
    public async Task Asking_again_with_the_same_tip_reuses_the_open_page_and_another_tip_replaces_it()
    {
        var first = (await CustomerService().CreateStripeSessionAsync(Quote(tipPercent: 10))).SessionId;
        var same = (await CustomerService().CreateStripeSessionAsync(Quote(tipPercent: 10))).SessionId;
        var other = (await CustomerService().CreateStripeSessionAsync(Quote(tipPercent: 20))).SessionId;

        Assert.Equal(first, same);
        Assert.NotEqual(first, other);
        Assert.Equal(first, Assert.Single(_stripe.Expired));
        Assert.Equal(other, Appointment.Payment!.StripeSessionId);
        Assert.Equal(Price + Tax + 9.10m, Appointment.Payment.Amount);
    }

    [Fact]
    public async Task The_payment_is_taken_in_the_currency_of_the_salon()
    {
        _context.Salons.Single(x => x.Id == Appointment.SalonId).Currency = "EUR";
        await _context.SaveChangesAsync();

        await CustomerService().CreateStripeSessionAsync(Quote());

        Assert.Equal("eur", Assert.Single(_stripe.Created).Currency);
        Assert.Equal("EUR", Appointment.Payment!.Currency);

        _stripe.Pay("cs_test_1");
        await CustomerService().ConfirmStripePaymentAsync(AppointmentId);

        Assert.Equal(PaymentStatusNames.Paid, Appointment.PaymentStatus);
    }

    [Fact]
    public async Task An_appointment_the_salon_has_not_confirmed_yet_cannot_be_paid()
    {
        _context.Appointments.Single(x => x.Id == AppointmentId).StateName = AppointmentStateNames.Pending;
        await _context.SaveChangesAsync();

        var stripe = await Assert.ThrowsAsync<ClientException>(() => CustomerService().CreateStripeSessionAsync(Quote()));
        _context.Appointments.Single(x => x.Id == AppointmentId).PaymentMethod = PaymentMethodNames.PayPal;
        await _context.SaveChangesAsync();
        var paypal = await Assert.ThrowsAsync<ClientException>(() => CustomerService().CreateOrderAsync(Quote()));

        Assert.Equal("The appointment can be paid only after the salon has confirmed it.", stripe.Message);
        Assert.Equal(stripe.Message, paypal.Message);
        Assert.Empty(_stripe.Created);
    }

    [Fact]
    public async Task A_completed_appointment_that_is_still_unpaid_can_be_paid()
    {
        _context.Appointments.Single(x => x.Id == AppointmentId).StateName = AppointmentStateNames.Completed;
        await _context.SaveChangesAsync();

        await CustomerService().CreateStripeSessionAsync(Quote());

        Assert.Single(_stripe.Created);
    }
}
