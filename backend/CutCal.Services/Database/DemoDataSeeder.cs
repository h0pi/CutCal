using CutCal.Model.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CutCal.Services.Database;

/// <summary>
/// Keeps the demo data usable no matter when the app is started. Seeded (HasData) rows have fixed dates
/// that are eventually in the past, so this tops every salon up to a few upcoming appointments
/// (respecting working hours and staff availability). It is idempotent and only ever adds rows.
/// </summary>
public static class DemoDataSeeder
{
    private const int UpcomingPerSalon = 3;
    private const int FirstDayOffset = 1;
    private const int SearchDays = 21;
    private const int DayStride = 2;

    public static async Task EnsureUpcomingAppointmentsAsync(CutCalDbContext context, ILogger logger)
    {
        var now = DateTime.UtcNow;

        var salons = await context.Salons
            .Include(x => x.WorkingHours)
            .Include(x => x.Services)
            .Include(x => x.Staff).ThenInclude(s => s.StaffServices)
            .Where(x => x.IsApproved)
            .ToListAsync();

        var customerIds = await context.UserRoles
            .Where(x => x.Role.Name == RoleNames.Customer && x.User.IsActive)
            .OrderBy(x => x.UserId)
            .Select(x => x.UserId)
            .ToListAsync();
        if (customerIds.Count == 0)
        {
            return;
        }

        var upcoming = await context.Appointments
            .Where(x => x.ScheduledAt > now && x.StateName != AppointmentStateNames.Cancelled)
            .Select(x => new { x.SalonId, x.StaffId, x.ScheduledAt, x.DurationMinutes })
            .ToListAsync();

        var added = 0;
        foreach (var salon in salons)
        {
            var activeStaff = salon.Staff.Where(x => x.IsActive && x.StaffServices.Count > 0).OrderBy(x => x.Id).ToList();
            if (activeStaff.Count == 0)
            {
                continue;
            }

            var missing = UpcomingPerSalon - upcoming.Count(x => x.SalonId == salon.Id);
            for (var k = 0; k < missing; k++)
            {
                var staff = activeStaff[(salon.Id + k) % activeStaff.Count];
                var serviceIds = staff.StaffServices.Select(x => x.ServiceId).ToHashSet();
                var service = salon.Services.Where(x => x.IsActive && serviceIds.Contains(x.Id)).OrderBy(x => x.Id).ElementAtOrDefault(k % serviceIds.Count);
                if (service is null)
                {
                    continue;
                }

                var start = FindFreeStart(salon, staff.Id, service.DurationMinutes, k, upcoming.Where(x => x.StaffId == staff.Id).Select(x => (x.ScheduledAt, x.DurationMinutes)).ToList(), now);
                if (start is null)
                {
                    continue;
                }

                // Salons that auto-confirm never hold pending requests; the others alternate so managers have something to approve.
                var confirmed = salon.AutoConfirm || k % 2 == 1;
                context.Appointments.Add(new Appointment
                {
                    CustomerId = customerIds[(salon.Id * 3 + k) % customerIds.Count],
                    SalonId = salon.Id,
                    StaffId = staff.Id,
                    ServiceId = service.Id,
                    ScheduledAt = start.Value,
                    DurationMinutes = service.DurationMinutes,
                    Price = service.Price,
                    StateName = confirmed ? AppointmentStateNames.Confirmed : AppointmentStateNames.Pending,
                    PaymentMethod = k % 2 == 0 ? PaymentMethodNames.Cash : PaymentMethodNames.PayPal,
                    PaymentStatus = PaymentStatusNames.Unpaid,
                    ApprovedById = confirmed ? salon.OwnerId : null,
                    ApprovedAt = confirmed ? now : null,
                    CreatedAt = now
                });
                upcoming.Add(new { SalonId = salon.Id, StaffId = staff.Id, ScheduledAt = start.Value, service.DurationMinutes });
                added++;
            }
        }

        if (added > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Demo data: added {Count} upcoming appointment(s).", added);
        }
    }

    private static DateTime? FindFreeStart(Salon salon, int staffId, int durationMinutes, int attempt, List<(DateTime Start, int Minutes)> staffBookings, DateTime now)
    {
        for (var offset = FirstDayOffset + attempt * DayStride; offset < FirstDayOffset + SearchDays; offset++)
        {
            var date = now.Date.AddDays(offset);
            var hours = salon.WorkingHours.FirstOrDefault(x => x.DayOfWeek == (int)date.DayOfWeek);
            if (hours is null || hours.IsClosed || hours.OpenTime is null || hours.CloseTime is null)
            {
                continue;
            }

            var open = date + hours.OpenTime.Value.ToTimeSpan();
            var close = date + hours.CloseTime.Value.ToTimeSpan();
            var duration = TimeSpan.FromMinutes(durationMinutes);

            // Start one or two hours after opening, then walk forward in half-hour steps until the slot is free.
            for (var start = open.AddHours(1 + (salon.Id + attempt) % 3); start + duration <= close; start = start.AddMinutes(30))
            {
                var end = start + duration;
                if (!staffBookings.Any(b => b.Start < end && start < b.Start.AddMinutes(b.Minutes)))
                {
                    return start;
                }
            }
        }
        return null;
    }
}
