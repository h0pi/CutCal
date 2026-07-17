namespace CutCal.Worker.Email;

public interface IEmailService
{
    Task SendAppointmentConfirmed(string email, string customerName, string salonName, DateTime scheduledAt);
    Task SendAppointmentCancelled(string email, string customerName, string salonName, string? reason);
    Task SendAppointmentReminder(string email, string customerName, string salonName, DateTime scheduledAt);
}
