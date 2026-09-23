using CutCal.Messaging;
using CutCal.Model.Constants;
using CutCal.Model.Enums;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using CutCal.Services.Messaging;
using CutCal.Services.StateMachine;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CutCal.Services.Services;

public interface IAppointmentService : IBaseReadService<AppointmentResponse, AppointmentSearchObject>
{
    Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request, int customerId);
    Task<AppointmentResponse> ConfirmAsync(int id, int managerId);
    Task<AppointmentResponse> CancelAsync(int id, string reason, int userId);
    Task<AppointmentResponse> CompleteAsync(int id);
    Task<AppointmentResponse> RescheduleAsync(int id, DateTime newScheduledAt, int customerId);
    Task<AppointmentResponse> ReassignStaffAsync(int id, int staffId);
}

public class AppointmentService : BaseReadService<Appointment, AppointmentResponse, AppointmentSearchObject>, IAppointmentService
{
    private readonly IRabbitMqPublisher _publisher;
    private readonly INotificationService _notificationService;
    private readonly IAuthenticatedUserAccessor _userAccessor;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        CutCalDbContext context,
        IRabbitMqPublisher publisher,
        INotificationService notificationService,
        IAuthenticatedUserAccessor userAccessor,
        IPaymentService paymentService,
        ILogger<AppointmentService> logger) : base(context)
    {
        _publisher = publisher;
        _notificationService = notificationService;
        _userAccessor = userAccessor;
        _paymentService = paymentService;
        _logger = logger;
    }

    protected override IQueryable<Appointment> AddInclude(IQueryable<Appointment> query)
    {
        return query
            .Include(x => x.Customer)
            .Include(x => x.Salon)
            .Include(x => x.Staff).ThenInclude(s => s.User)
            .Include(x => x.Service)
            .Include(x => x.Review);
    }

    protected override IQueryable<Appointment> AddSecurityFilter(IQueryable<Appointment> query)
    {
        // Appointments are private: a Customer only ever sees their own bookings, Staff only
        // their own assigned appointments, and a SalonManager only appointments at salons they own.
        // Admin is unrestricted.
        if (_userAccessor.IsInRole(RoleNames.Customer))
        {
            query = query.Where(x => x.CustomerId == _userAccessor.UserId);
        }
        else if (_userAccessor.IsInRole(RoleNames.Staff))
        {
            query = query.Where(x => x.Staff.UserId == _userAccessor.UserId);
        }
        else if (_userAccessor.IsInRole(RoleNames.SalonManager))
        {
            query = query.Where(x => x.Salon.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<Appointment> AddFilter(IQueryable<Appointment> query, AppointmentSearchObject search)
    {
        // CustomerId is already forced to the caller's own id for the Customer role via
        // AddSecurityFilter, so an explicit filter value here is only meaningful for Admin/staff/manager.
        if (search.CustomerId.HasValue && !_userAccessor.IsInRole(RoleNames.Customer))
        {
            query = query.Where(x => x.CustomerId == search.CustomerId.Value);
        }
        if (search.SalonId.HasValue)
        {
            query = query.Where(x => x.SalonId == search.SalonId.Value);
        }
        if (search.StaffId.HasValue)
        {
            query = query.Where(x => x.StaffId == search.StaffId.Value);
        }
        if (!string.IsNullOrWhiteSpace(search.Status))
        {
            query = query.Where(x => x.StateName == search.Status);
        }
        if (!string.IsNullOrWhiteSpace(search.PaymentStatus))
        {
            query = query.Where(x => x.PaymentStatus == search.PaymentStatus);
        }
        if (search.DateFrom.HasValue)
        {
            query = query.Where(x => x.ScheduledAt >= search.DateFrom.Value);
        }
        if (search.DateTo.HasValue)
        {
            query = query.Where(x => x.ScheduledAt <= search.DateTo.Value);
        }
        return query.OrderByDescending(x => x.ScheduledAt);
    }

    public static BaseAppointmentState GetState(string stateName) => stateName switch
    {
        AppointmentStateNames.Pending => new PendingAppointmentState(),
        AppointmentStateNames.Confirmed => new ConfirmedAppointmentState(),
        AppointmentStateNames.Completed => new CompletedAppointmentState(),
        AppointmentStateNames.Cancelled => new CancelledAppointmentState(),
        _ => throw new ClientException($"Unknown appointment state '{stateName}'.")
    };

    /// <summary>
    /// Appointment times are the salon's wall-clock time and are compared with UTC, so a slot that is
    /// less than the salon's UTC offset in the past is not caught here; everything earlier is.
    /// </summary>
    private static void EnsureNotInPast(DateTime scheduledAt)
    {
        if (scheduledAt < DateTime.UtcNow)
        {
            throw new ClientException("An appointment cannot be scheduled in the past.");
        }
    }

    /// <summary>
    /// Shared by InsertAsync and RescheduleAsync: the salon must be open at that time, and the
    /// staff member must not already have another (non-cancelled) appointment overlapping it.
    /// </summary>
    private async Task ValidateSlotAvailableAsync(Salon salon, int durationMinutes, int staffId, DateTime scheduledAt, int? excludeAppointmentId)
    {
        var dayOfWeek = (int)scheduledAt.DayOfWeek;
        var workingHours = salon.WorkingHours.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
        if (workingHours is null || workingHours.IsClosed || workingHours.OpenTime is null || workingHours.CloseTime is null)
        {
            throw new ClientException("Salon is closed on the selected day.");
        }

        var requestedTime = TimeOnly.FromDateTime(scheduledAt);
        var endTime = TimeOnly.FromDateTime(scheduledAt.AddMinutes(durationMinutes));
        if (requestedTime < workingHours.OpenTime.Value || endTime > workingHours.CloseTime.Value)
        {
            throw new ClientException("Selected time is outside of salon working hours.");
        }

        var endsAt = scheduledAt.AddMinutes(durationMinutes);
        var excludedId = excludeAppointmentId ?? 0;
        var overlapping = await Context.Appointments.AnyAsync(a =>
            a.StaffId == staffId &&
            a.Id != excludedId &&
            a.StateName != AppointmentStateNames.Cancelled &&
            a.ScheduledAt < endsAt &&
            scheduledAt < a.ScheduledAt.AddMinutes(a.DurationMinutes));
        if (overlapping)
        {
            throw new ClientException("Selected staff member already has an appointment at that time.");
        }
    }

    private static NotificationMessage BuildMessage(string type, Appointment appointment, string? reason = null) => new()
    {
        Type = type,
        AppointmentId = appointment.Id,
        CustomerEmail = appointment.Customer.Email,
        CustomerName = appointment.Customer.FirstName,
        SalonName = appointment.Salon.Name,
        ScheduledAt = appointment.ScheduledAt,
        Reason = reason
    };

    private async Task<Appointment> LoadForChangeAsync(int id)
    {
        return await Context.Appointments
            .Include(x => x.Customer)
            .Include(x => x.Salon).ThenInclude(s => s.WorkingHours)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ClientException("Appointment not found.");
    }

    public async Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request, int customerId)
    {
        EnsureNotInPast(request.ScheduledAt);

        var salon = await Context.Salons.Include(x => x.WorkingHours).FirstOrDefaultAsync(x => x.Id == request.SalonId)
            ?? throw new ClientException("Salon not found.");
        var service = await Context.SalonServices.FirstOrDefaultAsync(x => x.Id == request.ServiceId && x.SalonId == request.SalonId)
            ?? throw new ClientException("Service not found for this salon.");
        var staff = await Context.Staff.FirstOrDefaultAsync(x => x.Id == request.StaffId && x.SalonId == request.SalonId)
            ?? throw new ClientException("Staff member not found for this salon.");
        var customer = await Context.Users.FirstOrDefaultAsync(x => x.Id == customerId)
            ?? throw new ClientException("Customer not found.");

        await ValidateSlotAvailableAsync(salon, service.DurationMinutes, request.StaffId, request.ScheduledAt, excludeAppointmentId: null);

        var confirmed = salon.AutoConfirm;
        var appointment = new Appointment
        {
            Customer = customer,
            Salon = salon,
            StaffId = staff.Id,
            ServiceId = service.Id,
            ScheduledAt = request.ScheduledAt,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatusNames.Unpaid,
            StateName = confirmed ? AppointmentStateNames.Confirmed : AppointmentStateNames.Pending,
            CreatedAt = DateTime.UtcNow
        };
        if (confirmed)
        {
            appointment.ApprovedById = salon.OwnerId;
            appointment.ApprovedAt = DateTime.UtcNow;
        }

        Context.Appointments.Add(appointment);
        _notificationService.Add(customerId,
            confirmed ? "Appointment confirmed" : "Appointment requested",
            $"Your appointment at {salon.Name} on {appointment.ScheduledAt:g} is {appointment.StateName}.",
            confirmed ? nameof(NotificationType.AppointmentConfirmed) : nameof(NotificationType.AppointmentRequested));

        // One SaveChanges: the appointment and its notification are stored together or not at all.
        await Context.SaveChangesAsync();

        await _publisher.PublishAsync(BuildMessage(confirmed ? NotificationMessageTypes.AppointmentConfirmed : NotificationMessageTypes.AppointmentRequested, appointment));

        return await GetByIdAsync(appointment.Id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> ConfirmAsync(int id, int managerId)
    {
        var appointment = await LoadForChangeAsync(id);
        GetState(appointment.StateName).Confirm(appointment, managerId);

        _notificationService.Add(appointment.CustomerId, "Appointment confirmed",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} has been confirmed.", nameof(NotificationType.AppointmentConfirmed));
        await Context.SaveChangesAsync();

        await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.AppointmentConfirmed, appointment));

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> CancelAsync(int id, string reason, int userId)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ClientException("Cancellation reason is required.");
        }

        var appointment = await LoadForChangeAsync(id);
        var wasPaid = appointment.PaymentStatus == PaymentStatusNames.Paid;
        GetState(appointment.StateName).Cancel(appointment, reason);

        _notificationService.Add(appointment.CustomerId, "Appointment cancelled",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} was cancelled. Reason: {reason}", nameof(NotificationType.AppointmentCancelled));
        await Context.SaveChangesAsync();

        if (wasPaid)
        {
            try
            {
                await _paymentService.RefundAsync(id);
            }
            catch (Exception ex)
            {
                // The cancellation itself already succeeded and must not be rolled back for a refund
                // failure; an Admin/Manager can still trigger Payments/Refund/{id} manually afterwards.
                _logger.LogError(ex, "Automatic refund failed for cancelled appointment {AppointmentId}; needs a manual refund.", id);
            }
        }

        await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.AppointmentCancelled, appointment, reason));

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> CompleteAsync(int id)
    {
        var appointment = await LoadForChangeAsync(id);
        GetState(appointment.StateName).Complete(appointment);

        _notificationService.Add(appointment.CustomerId, "Appointment completed",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} is now complete. Feel free to leave a review!",
            nameof(NotificationType.AppointmentCompleted));
        await Context.SaveChangesAsync();

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> RescheduleAsync(int id, DateTime newScheduledAt, int customerId)
    {
        EnsureNotInPast(newScheduledAt);

        var appointment = await LoadForChangeAsync(id);
        if (appointment.CustomerId != customerId)
        {
            throw new ClientException("You can only reschedule your own appointments.");
        }
        if (appointment.StateName is not (AppointmentStateNames.Pending or AppointmentStateNames.Confirmed))
        {
            throw new ClientException($"An appointment in state '{appointment.StateName}' cannot be rescheduled.");
        }

        await ValidateSlotAvailableAsync(appointment.Salon, appointment.DurationMinutes, appointment.StaffId, newScheduledAt, excludeAppointmentId: id);

        appointment.ScheduledAt = newScheduledAt;
        _notificationService.Add(appointment.CustomerId, "Appointment rescheduled",
            $"Your appointment at {appointment.Salon.Name} was moved to {newScheduledAt:g}.", nameof(NotificationType.AppointmentRescheduled));
        await Context.SaveChangesAsync();

        await _publisher.PublishAsync(BuildMessage(NotificationMessageTypes.AppointmentRescheduled, appointment));

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> ReassignStaffAsync(int id, int staffId)
    {
        var appointment = await LoadForChangeAsync(id);
        await OwnershipGuard.EnsureManagesSalonAsync(Context, appointment.SalonId, _userAccessor);

        if (appointment.StateName is not (AppointmentStateNames.Pending or AppointmentStateNames.Confirmed))
        {
            throw new ClientException($"An appointment in state '{appointment.StateName}' cannot be reassigned.");
        }

        _ = await Context.Staff.FirstOrDefaultAsync(x => x.Id == staffId && x.SalonId == appointment.SalonId)
            ?? throw new ClientException("Staff member not found for this salon.");

        await ValidateSlotAvailableAsync(appointment.Salon, appointment.DurationMinutes, staffId, appointment.ScheduledAt, excludeAppointmentId: id);

        appointment.StaffId = staffId;
        await Context.SaveChangesAsync();

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }
}
