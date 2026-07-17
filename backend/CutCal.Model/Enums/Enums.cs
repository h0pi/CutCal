namespace CutCal.Model.Enums;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    PayPal
}

public enum PaymentStatus
{
    Unpaid,
    Paid,
    Refunded
}

public enum NotificationType
{
    AppointmentConfirmed,
    AppointmentCancelled,
    AppointmentCompleted,
    AppointmentReminder,
    NewPromotion,
    PaymentReceived,
    PaymentRefunded
}

public enum UserRole
{
    Customer,
    Staff,
    SalonManager,
    Admin
}
