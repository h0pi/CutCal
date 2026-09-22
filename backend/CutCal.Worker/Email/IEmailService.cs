using CutCal.Messaging;

namespace CutCal.Worker.Email;

/// <summary>Every method throws when the e-mail could not be sent, so the consumer can retry it.</summary>
public interface IEmailService
{
    Task SendAppointmentRequested(NotificationMessage message);
    Task SendAppointmentConfirmed(NotificationMessage message);
    Task SendAppointmentCancelled(NotificationMessage message);
    Task SendAppointmentRescheduled(NotificationMessage message);
    Task SendPaymentReceived(NotificationMessage message);
    Task SendPaymentRefunded(NotificationMessage message);
}
