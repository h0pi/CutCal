using CutCal.Services.Database;

namespace CutCal.Services.Services;

/// <summary>What a customer may book; the availability calendar and the booking itself must apply exactly the same rules.</summary>
internal static class BookingRules
{
    /// <summary>The salon's own services that have not been deactivated.</summary>
    public static IQueryable<SalonService> Bookable(this IQueryable<SalonService> services, int salonId) =>
        services.Where(x => x.SalonId == salonId && x.IsActive);

    /// <summary>The salon's own active staff who perform the given service.</summary>
    public static IQueryable<Staff> Bookable(this IQueryable<Staff> staff, int salonId, int serviceId) =>
        staff.Where(x => x.SalonId == salonId && x.IsActive && x.StaffServices.Any(ss => ss.ServiceId == serviceId));
}
