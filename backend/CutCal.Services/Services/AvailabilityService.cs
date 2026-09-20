using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IAvailabilityService
{
    Task<List<AvailabilityDayResponse>> GetDaysAsync(int salonId, int serviceId, int? staffId, DateTime from, int days, DateTime? nowLocal);
    Task<List<AvailabilitySlotResponse>> GetSlotsAsync(int salonId, int serviceId, int? staffId, DateTime date, int? excludeAppointmentId, DateTime? nowLocal);
}

public class AvailabilityService : IAvailabilityService
{
    private const int SlotStepMinutes = 30;
    private const int MaxDaysRange = 62;
    private const double FreeThreshold = 0.5;

    private readonly CutCalDbContext _context;

    public AvailabilityService(CutCalDbContext context)
    {
        _context = context;
    }

    private sealed record Booking(int StaffId, DateTime Start, DateTime End);

    private sealed record AvailabilityContext(
        List<SalonWorkingHours> WorkingHours,
        int DurationMinutes,
        List<int> StaffIds,
        List<Booking> Bookings,
        DateTime Now);

    public async Task<List<AvailabilityDayResponse>> GetDaysAsync(int salonId, int serviceId, int? staffId, DateTime from, int days, DateTime? nowLocal)
    {
        if (days < 1 || days > MaxDaysRange)
        {
            throw new ClientException($"Days must be between 1 and {MaxDaysRange}.");
        }

        var firstDay = from.Date;
        var ctx = await LoadContextAsync(salonId, serviceId, staffId, firstDay, firstDay.AddDays(days), null, nowLocal);

        var result = new List<AvailabilityDayResponse>(days);
        for (var i = 0; i < days; i++)
        {
            var date = firstDay.AddDays(i);
            var slots = BuildSlots(ctx, date);
            var free = slots.Count(s => s.IsAvailable);
            result.Add(new AvailabilityDayResponse
            {
                Date = date,
                FreeSlots = free,
                TotalSlots = slots.Count,
                Status = slots.Count == 0 ? AvailabilityStatus.Closed
                    : free == 0 ? AvailabilityStatus.Full
                    : (double)free / slots.Count >= FreeThreshold ? AvailabilityStatus.Free
                    : AvailabilityStatus.Limited
            });
        }
        return result;
    }

    public async Task<List<AvailabilitySlotResponse>> GetSlotsAsync(int salonId, int serviceId, int? staffId, DateTime date, int? excludeAppointmentId, DateTime? nowLocal)
    {
        var day = date.Date;
        var ctx = await LoadContextAsync(salonId, serviceId, staffId, day, day.AddDays(1), excludeAppointmentId, nowLocal);
        return BuildSlots(ctx, day);
    }

    private async Task<AvailabilityContext> LoadContextAsync(int salonId, int serviceId, int? staffId, DateTime rangeStart, DateTime rangeEnd, int? excludeAppointmentId, DateTime? nowLocal)
    {
        var salon = await _context.Salons.AsNoTracking()
            .Include(x => x.WorkingHours)
            .FirstOrDefaultAsync(x => x.Id == salonId)
            ?? throw new ClientException("Salon not found.");

        var service = await _context.SalonServices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId && x.SalonId == salonId && x.IsActive)
            ?? throw new ClientException("Service not found for this salon.");

        var staffIds = await _context.Staff.AsNoTracking()
            .Where(x => x.SalonId == salonId && x.IsActive
                && (staffId == null || x.Id == staffId)
                && x.StaffServices.Any(ss => ss.ServiceId == serviceId))
            .Select(x => x.Id)
            .ToListAsync();

        if (staffId.HasValue && staffIds.Count == 0)
        {
            throw new ClientException("Selected staff member does not perform this service.");
        }

        // Look one day back so a late-evening appointment that runs past midnight still blocks the early slots.
        var lookBehind = rangeStart.AddDays(-1);
        var excludedId = excludeAppointmentId ?? 0;
        var bookings = await _context.Appointments.AsNoTracking()
            .Where(a => staffIds.Contains(a.StaffId)
                && a.StateName != "Cancelled"
                && a.Id != excludedId
                && a.ScheduledAt >= lookBehind
                && a.ScheduledAt < rangeEnd)
            .Select(a => new { a.StaffId, a.ScheduledAt, a.DurationMinutes })
            .ToListAsync();

        return new AvailabilityContext(
            salon.WorkingHours,
            service.DurationMinutes,
            staffIds,
            bookings.Select(b => new Booking(b.StaffId, b.ScheduledAt, b.ScheduledAt.AddMinutes(b.DurationMinutes))).ToList(),
            nowLocal ?? DateTime.UtcNow);
    }

    private static List<AvailabilitySlotResponse> BuildSlots(AvailabilityContext ctx, DateTime date)
    {
        var hours = ctx.WorkingHours.FirstOrDefault(x => x.DayOfWeek == (int)date.DayOfWeek);
        if (hours is null || hours.IsClosed || hours.OpenTime is null || hours.CloseTime is null)
        {
            return new List<AvailabilitySlotResponse>();
        }

        var open = date + hours.OpenTime.Value.ToTimeSpan();
        var close = date + hours.CloseTime.Value.ToTimeSpan();
        var duration = TimeSpan.FromMinutes(ctx.DurationMinutes);
        var slots = new List<AvailabilitySlotResponse>();

        for (var start = open; start + duration <= close; start = start.AddMinutes(SlotStepMinutes))
        {
            if (start <= ctx.Now)
            {
                continue;
            }

            var end = start + duration;
            var anyStaffFree = ctx.StaffIds.Any(staffId =>
                !ctx.Bookings.Any(b => b.StaffId == staffId && b.Start < end && start < b.End));

            slots.Add(new AvailabilitySlotResponse { StartsAt = start, IsAvailable = anyStaffFree });
        }
        return slots;
    }
}
