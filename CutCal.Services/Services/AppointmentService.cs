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

namespace CutCal.Services.Services;

public interface IAppointmentService : IBaseReadService<AppointmentResponse, AppointmentSearchObject>
{
    Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request, int customerId);
    Task<AppointmentResponse> ConfirmAsync(int id, int managerId);
    Task<AppointmentResponse> CancelAsync(int id, string reason, int userId);
    Task<AppointmentResponse> CompleteAsync(int id);
}

public class AppointmentService : BaseReadService<Appointment, AppointmentResponse, AppointmentSearchObject>, IAppointmentService
{
    private readonly IRabbitMqPublisher _publisher;
    private readonly INotificationService _notificationService;
    private readonly ISalonService _salonService;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public AppointmentService(
        CutCalDbContext context,
        IRabbitMqPublisher publisher,
        INotificationService notificationService,
        ISalonService salonService,
        IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _publisher = publisher;
        _notificationService = notificationService;
        _salonService = salonService;
        _userAccessor = userAccessor;
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
        if (_userAccessor.IsInRole("Customer"))
        {
            query = query.Where(x => x.CustomerId == _userAccessor.UserId);
        }
        else if (_userAccessor.IsInRole("Staff"))
        {
            query = query.Where(x => x.Staff.UserId == _userAccessor.UserId);
        }
        else if (_userAccessor.IsInRole("SalonManager"))
        {
            query = query.Where(x => x.Salon.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<Appointment> AddFilter(IQueryable<Appointment> query, AppointmentSearchObject search)
    {
        // CustomerId is already forced to the caller's own id for the Customer role via
        // AddSecurityFilter, so an explicit filter value here is only meaningful for Admin/staff/manager.
        if (search.CustomerId.HasValue && !_userAccessor.IsInRole("Customer"))
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
        "Pending" => new PendingAppointmentState(),
        "Confirmed" => new ConfirmedAppointmentState(),
        "Completed" => new CompletedAppointmentState(),
        "Cancelled" => new CancelledAppointmentState(),
        _ => throw new ClientException($"Unknown appointment state '{stateName}'.")
    };

    public async Task<AppointmentResponse> InsertAsync(AppointmentInsertRequest request, int customerId)
    {
        var salon = await Context.Salons.Include(x => x.WorkingHours).FirstOrDefaultAsync(x => x.Id == request.SalonId)
            ?? throw new ClientException("Salon not found.");
        var service = await Context.SalonServices.FirstOrDefaultAsync(x => x.Id == request.ServiceId && x.SalonId == request.SalonId)
            ?? throw new ClientException("Service not found for this salon.");
        var staff = await Context.Staff.FirstOrDefaultAsync(x => x.Id == request.StaffId && x.SalonId == request.SalonId)
            ?? throw new ClientException("Staff member not found for this salon.");

        var dayOfWeek = (int)request.ScheduledAt.DayOfWeek;
        var workingHours = salon.WorkingHours.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
        if (workingHours is null || workingHours.IsClosed || workingHours.OpenTime is null || workingHours.CloseTime is null)
        {
            throw new ClientException("Salon is closed on the selected day.");
        }

        var requestedTime = TimeOnly.FromDateTime(request.ScheduledAt);
        var endTime = TimeOnly.FromDateTime(request.ScheduledAt.AddMinutes(service.DurationMinutes));
        if (requestedTime < workingHours.OpenTime.Value || endTime > workingHours.CloseTime.Value)
        {
            throw new ClientException("Selected time is outside of salon working hours.");
        }

        var endsAt = request.ScheduledAt.AddMinutes(service.DurationMinutes);
        var overlapping = await Context.Appointments.AnyAsync(a =>
            a.StaffId == request.StaffId &&
            a.StateName != "Cancelled" &&
            a.ScheduledAt < endsAt &&
            request.ScheduledAt < a.ScheduledAt.AddMinutes(a.DurationMinutes));
        if (overlapping)
        {
            throw new ClientException("Selected staff member already has an appointment at that time.");
        }

        var appointment = new Appointment
        {
            CustomerId = customerId,
            SalonId = request.SalonId,
            StaffId = request.StaffId,
            ServiceId = request.ServiceId,
            ScheduledAt = request.ScheduledAt,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = "Unpaid",
            StateName = salon.AutoConfirm ? "Confirmed" : "Pending",
            CreatedAt = DateTime.UtcNow
        };
        if (salon.AutoConfirm)
        {
            appointment.ApprovedById = salon.OwnerId;
            appointment.ApprovedAt = DateTime.UtcNow;
        }

        Context.Appointments.Add(appointment);
        await Context.SaveChangesAsync();

        await _notificationService.CreateAsync(customerId,
            appointment.StateName == "Confirmed" ? "Appointment confirmed" : "Appointment requested",
            $"Your appointment at {salon.Name} on {appointment.ScheduledAt:g} is {appointment.StateName}.",
            appointment.StateName == "Confirmed" ? "AppointmentConfirmed" : "AppointmentConfirmed");

        await _publisher.PublishAsync("appointment.created", new
        {
            Type = "AppointmentConfirmed",
            appointment.Id,
            CustomerEmail = (await Context.Users.FindAsync(customerId))?.Email,
            SalonName = salon.Name,
            appointment.ScheduledAt
        });

        return await GetByIdAsync(appointment.Id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> ConfirmAsync(int id, int managerId)
    {
        var appointment = await Context.Appointments.Include(x => x.Salon).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ClientException("Appointment not found.");
        var state = GetState(appointment.StateName);
        state.Confirm(appointment, managerId);
        await Context.SaveChangesAsync();

        await _notificationService.CreateAsync(appointment.CustomerId, "Appointment confirmed",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} has been confirmed.", "AppointmentConfirmed");
        await _publisher.PublishAsync("appointment.confirmed", new { Type = "AppointmentConfirmed", appointment.Id });

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> CancelAsync(int id, string reason, int userId)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ClientException("Cancellation reason is required.");
        }

        var appointment = await Context.Appointments.Include(x => x.Salon).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ClientException("Appointment not found.");
        var state = GetState(appointment.StateName);
        state.Cancel(appointment, reason);
        await Context.SaveChangesAsync();

        await _notificationService.CreateAsync(appointment.CustomerId, "Appointment cancelled",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} was cancelled. Reason: {reason}", "AppointmentCancelled");
        await _publisher.PublishAsync("appointment.cancelled", new { Type = "AppointmentCancelled", appointment.Id, Reason = reason });

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }

    public async Task<AppointmentResponse> CompleteAsync(int id)
    {
        var appointment = await Context.Appointments.Include(x => x.Salon).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ClientException("Appointment not found.");
        var state = GetState(appointment.StateName);
        state.Complete(appointment);
        await Context.SaveChangesAsync();

        await _notificationService.CreateAsync(appointment.CustomerId, "Appointment completed",
            $"Your appointment at {appointment.Salon.Name} on {appointment.ScheduledAt:g} is now complete. Feel free to leave a review!",
            nameof(CutCal.Model.Enums.NotificationType.AppointmentCompleted));

        return await GetByIdAsync(id) ?? appointment.Adapt<AppointmentResponse>();
    }
}
