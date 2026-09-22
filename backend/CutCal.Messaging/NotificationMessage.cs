namespace CutCal.Messaging;

/// <summary>What happened; also used as the RabbitMQ routing key of the message.</summary>
public static class NotificationMessageTypes
{
    public const string AppointmentRequested = "AppointmentRequested";
    public const string AppointmentConfirmed = "AppointmentConfirmed";
    public const string AppointmentCancelled = "AppointmentCancelled";
    public const string AppointmentRescheduled = "AppointmentRescheduled";
    public const string PaymentReceived = "PaymentReceived";
    public const string PaymentRefunded = "PaymentRefunded";

    public static readonly string[] All =
    {
        AppointmentRequested, AppointmentConfirmed, AppointmentCancelled, AppointmentRescheduled, PaymentReceived, PaymentRefunded
    };
}

/// <summary>
/// Message the API publishes and the worker turns into an e-mail. It carries everything the worker
/// needs, so the worker never has to read the API's database.
/// </summary>
public class NotificationMessage
{
    public string Type { get; set; } = null!;
    public int AppointmentId { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string SalonName { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string? Reason { get; set; }
    public decimal? Amount { get; set; }
}
