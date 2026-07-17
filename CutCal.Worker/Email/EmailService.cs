using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CutCal.Worker.Email;

public class EmailService : IEmailService
{
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
        _port = int.TryParse(Environment.GetEnvironmentVariable("Smtp__Port"), out var port) ? port : 587;
        _username = Environment.GetEnvironmentVariable("Smtp__Username") ?? string.Empty;
        _password = Environment.GetEnvironmentVariable("Smtp__Password") ?? string.Empty;
        _useSsl = bool.TryParse(Environment.GetEnvironmentVariable("Smtp__UseSsl"), out var ssl) && ssl;
    }

    public Task SendAppointmentConfirmed(string email, string customerName, string salonName, DateTime scheduledAt)
    {
        return SendAsync(email, "Appointment confirmed",
            $"Hi {customerName}, your appointment at {salonName} on {scheduledAt:g} has been confirmed.");
    }

    public Task SendAppointmentCancelled(string email, string customerName, string salonName, string? reason)
    {
        return SendAsync(email, "Appointment cancelled",
            $"Hi {customerName}, your appointment at {salonName} was cancelled. Reason: {reason}");
    }

    public Task SendAppointmentReminder(string email, string customerName, string salonName, DateTime scheduledAt)
    {
        return SendAsync(email, "Appointment reminder",
            $"Hi {customerName}, this is a reminder for your appointment at {salonName} on {scheduledAt:g}.");
    }

    private async Task SendAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            _logger.LogWarning("Skipping email '{Subject}' because recipient address is empty.", subject);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(string.IsNullOrWhiteSpace(_username) ? "no-reply@cutcal.com" : _username));
        message.To.Add(MailboxAddress.Parse(toEmail));
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
            _logger.LogInformation("Sent email '{Subject}' to {Recipient}", subject, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email '{Subject}' to {Recipient}", subject, toEmail);
        }
    }
}
