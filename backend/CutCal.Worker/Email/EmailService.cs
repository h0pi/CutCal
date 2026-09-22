using CutCal.Messaging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CutCal.Worker.Email;

public class EmailService : IEmailService
{
    private const int DefaultSmtpPort = 587;
    private const string FallbackSender = "no-reply@cutcal.com";

    private readonly ILogger<EmailService> _logger;
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _useSsl;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
        _host = Environment.GetEnvironmentVariable("Smtp__Host") ?? "localhost";
        _port = int.TryParse(Environment.GetEnvironmentVariable("Smtp__Port"), out var port) ? port : DefaultSmtpPort;
        _username = Environment.GetEnvironmentVariable("Smtp__Username") ?? string.Empty;
        _password = Environment.GetEnvironmentVariable("Smtp__Password") ?? string.Empty;
        _useSsl = bool.TryParse(Environment.GetEnvironmentVariable("Smtp__UseSsl"), out var ssl) && ssl;
    }

    public Task SendAppointmentRequested(NotificationMessage message) => SendAsync(message, "Appointment request received",
        $"Hi {message.CustomerName}, we received your request for {message.SalonName} on {message.ScheduledAt:f}. The salon will confirm it shortly.");

    public Task SendAppointmentConfirmed(NotificationMessage message) => SendAsync(message, "Appointment confirmed",
        $"Hi {message.CustomerName}, your appointment at {message.SalonName} on {message.ScheduledAt:f} is confirmed. See you soon!");

    public Task SendAppointmentCancelled(NotificationMessage message) => SendAsync(message, "Appointment cancelled",
        $"Hi {message.CustomerName}, your appointment at {message.SalonName} on {message.ScheduledAt:f} was cancelled. Reason: {message.Reason}");

    public Task SendAppointmentRescheduled(NotificationMessage message) => SendAsync(message, "Appointment rescheduled",
        $"Hi {message.CustomerName}, your appointment at {message.SalonName} has been moved to {message.ScheduledAt:f}.");

    public Task SendPaymentReceived(NotificationMessage message) => SendAsync(message, "Payment received",
        $"Hi {message.CustomerName}, we received your payment of {message.Amount:0.00} for your appointment at {message.SalonName} on {message.ScheduledAt:f}. Thank you!");

    public Task SendPaymentRefunded(NotificationMessage message) => SendAsync(message, "Payment refunded",
        $"Hi {message.CustomerName}, your payment of {message.Amount:0.00} for the appointment at {message.SalonName} has been refunded.");

    private async Task SendAsync(NotificationMessage notification, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(notification.CustomerEmail))
        {
            // Retrying cannot help without an address, so this is logged and skipped rather than thrown.
            _logger.LogWarning("Skipping '{Subject}' for appointment {AppointmentId}: the customer has no e-mail address.", subject, notification.AppointmentId);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(string.IsNullOrWhiteSpace(_username) ? FallbackSender : _username));
        message.To.Add(new MailboxAddress(notification.CustomerName, notification.CustomerEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            if (!string.IsNullOrWhiteSpace(_username))
            {
                await client.AuthenticateAsync(_username, _password);
            }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation("Sent '{Subject}' to {Recipient} for appointment {AppointmentId}", subject, notification.CustomerEmail, notification.AppointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send '{Subject}' to {Recipient}", subject, notification.CustomerEmail);
            throw;
        }
    }
}
