using CutCal.Model.Exceptions;
using CutCal.Services.Database;

namespace CutCal.Services.StateMachine;

public abstract class BaseAppointmentState
{
    public abstract string Name { get; }

    public virtual void Confirm(Appointment appointment, int managerId)
    {
        throw new ClientException($"Appointment in state '{Name}' cannot be confirmed.");
    }

    public virtual void Cancel(Appointment appointment, string reason)
    {
        throw new ClientException($"Appointment in state '{Name}' cannot be cancelled.");
    }

    public virtual void Complete(Appointment appointment)
    {
        throw new ClientException($"Appointment in state '{Name}' cannot be completed.");
    }
}

public class PendingAppointmentState : BaseAppointmentState
{
    public override string Name => "Pending";

    public override void Confirm(Appointment appointment, int managerId)
    {
        appointment.StateName = "Confirmed";
        appointment.ApprovedById = managerId;
        appointment.ApprovedAt = DateTime.UtcNow;
    }

    public override void Cancel(Appointment appointment, string reason)
    {
        appointment.StateName = "Cancelled";
        appointment.CancellationReason = reason;
    }
}

public class ConfirmedAppointmentState : BaseAppointmentState
{
    public override string Name => "Confirmed";

    public override void Complete(Appointment appointment)
    {
        appointment.StateName = "Completed";
    }

    public override void Cancel(Appointment appointment, string reason)
    {
        appointment.StateName = "Cancelled";
        appointment.CancellationReason = reason;
    }
}

public class CompletedAppointmentState : BaseAppointmentState
{
    public override string Name => "Completed";
}

public class CancelledAppointmentState : BaseAppointmentState
{
    public override string Name => "Cancelled";
}
