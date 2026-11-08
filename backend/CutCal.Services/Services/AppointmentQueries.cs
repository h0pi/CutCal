using CutCal.Services.Database;

namespace CutCal.Services.Services;

internal static class AppointmentQueries
{
    /// <summary>
    /// Appointments from <paramref name="dateFrom"/> up to, but not including, <paramref name="dateTo"/>. A client that wants a whole last
    /// day sends the start of the day after it, so nothing scheduled during that day is cut off.
    /// </summary>
    public static IQueryable<Appointment> InPeriod(this IQueryable<Appointment> appointments, DateTime? dateFrom, DateTime? dateTo)
    {
        if (dateFrom.HasValue)
        {
            appointments = appointments.Where(x => x.ScheduledAt >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            appointments = appointments.Where(x => x.ScheduledAt < dateTo.Value);
        }
        return appointments;
    }
}
