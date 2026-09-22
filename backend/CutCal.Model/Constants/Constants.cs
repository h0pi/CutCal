using CutCal.Model.Enums;

namespace CutCal.Model.Constants;

/// <summary>Role names as stored in the database and used in [Authorize(Roles = ...)] attributes.</summary>
public static class RoleNames
{
    public const string Customer = nameof(UserRole.Customer);
    public const string Staff = nameof(UserRole.Staff);
    public const string SalonManager = nameof(UserRole.SalonManager);
    public const string Admin = nameof(UserRole.Admin);

    public const string AdminOrManager = Admin + "," + SalonManager;
    public const string AdminManagerOrStaff = Admin + "," + SalonManager + "," + Staff;
}

/// <summary>Appointment state names as stored in Appointments.StateName.</summary>
public static class AppointmentStateNames
{
    public const string Pending = nameof(AppointmentStatus.Pending);
    public const string Confirmed = nameof(AppointmentStatus.Confirmed);
    public const string Completed = nameof(AppointmentStatus.Completed);
    public const string Cancelled = nameof(AppointmentStatus.Cancelled);
}

public static class PaymentMethodNames
{
    public const string Cash = nameof(PaymentMethod.Cash);
    public const string PayPal = nameof(PaymentMethod.PayPal);
}

public static class PaymentStatusNames
{
    public const string Unpaid = nameof(PaymentStatus.Unpaid);
    public const string Paid = nameof(PaymentStatus.Paid);
    public const string Refunded = nameof(PaymentStatus.Refunded);

    /// <summary>A PayPal order exists but has not been captured yet (Payments.Status only).</summary>
    public const string Created = "Created";
}
