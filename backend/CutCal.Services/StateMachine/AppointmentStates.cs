using CutCal.Model.Constants;
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

    public virtual void Cancel(Appointment appointment, string reason, int cancelledById)
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
    public override string Name => AppointmentStateNames.Pending;

    public override void Confirm(Appointment appointment, int managerId)
    {
        appointment.StateName = AppointmentStateNames.Confirmed;
        appointment.ApprovedById = managerId;
        appointment.ApprovedAt = DateTime.UtcNow;
    }

    public override void Cancel(Appointment appointment, string reason, int cancelledById)
    {
        appointment.StateName = AppointmentStateNames.Cancelled;
        appointment.CancellationReason = reason;
        appointment.CancelledById = cancelledById;
        appointment.CancelledAt = DateTime.UtcNow;
    }
}

public class ConfirmedAppointmentState : BaseAppointmentState
{
    public override string Name => AppointmentStateNames.Confirmed;

    public override void Complete(Appointment appointment)
    {
        appointment.StateName = AppointmentStateNames.Completed;
    }

    public override void Cancel(Appointment appointment, string reason, int cancelledById)
    {
        appointment.StateName = AppointmentStateNames.Cancelled;
        appointment.CancellationReason = reason;
        appointment.CancelledById = cancelledById;
        appointment.CancelledAt = DateTime.UtcNow;
    }
}

public class CompletedAppointmentState : BaseAppointmentState
{
    public override string Name => AppointmentStateNames.Completed;
}

public class CancelledAppointmentState : BaseAppointmentState
{
    public override string Name => AppointmentStateNames.Cancelled;
}
