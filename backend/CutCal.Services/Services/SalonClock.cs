using CutCal.Services.Database;

namespace CutCal.Services.Services;

public static class SalonClock
{
    /// <summary>The time on the wall of the salon right now, in the salon's own time zone: appointment times are stored as that wall-clock time.</summary>
    public static DateTime Now(Salon salon) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(salon.TimeZone));
}
