using CutCal.Model.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CutCal.Services.Database;

/// <summary>
/// Gives the recommender something to learn from: customers with distinct tastes (each one favours two
/// neighbouring salon categories) and their bookings, favourites and salon views. Deterministic, and it
/// runs only once: if any of its customers already exists, nothing is added.
/// </summary>
public static class RecommenderDemoDataSeeder
{
    private const int FirstCustomerNumber = 11;
    private const int CustomerCount = 45;
    private const int RandomSeed = 20261011;

    // The avatars shipped with the API (wwwroot/images/avatars/1.jpg .. 47.jpg), reused so every demo account has a picture.
    private const int AvatarCount = 47;

    private const int LikedBookings = 3;
    private const int OtherBookings = 1;
    private const int FavoriteCount = 2;
    private const int LikedViews = 5;
    private const int OtherViews = 2;

    // Later than the last visit of the fixed seed data, so the visits added here never collide with it.
    private static readonly DateTime FirstVisitDay = new(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);

    private static readonly string[] FirstNames =
    {
        "Alma", "Benjamin", "Lejla", "Tarik", "Nerma", "Kenan", "Selma", "Armin", "Dina", "Emir", "Ajla", "Haris", "Mia", "Adin", "Zara"
    };

    private sealed record BookableSalon(Salon Salon, List<(Staff Staff, SalonService Service)> Options);

    public static async Task EnsureAsync(CutCalDbContext context, ILogger logger)
    {
        var usernames = Enumerable.Range(FirstCustomerNumber, CustomerCount).Select(number => $"customer{number}").ToList();
        if (await context.Users.AnyAsync(x => usernames.Contains(x.Username)))
        {
            return;
        }

        var customerRoleId = await context.Roles.Where(x => x.Name == RoleNames.Customer).Select(x => x.Id).SingleAsync();
        var salons = await context.Salons
            .Include(x => x.Services)
            .Include(x => x.Staff).ThenInclude(s => s.StaffServices)
            .Where(x => x.IsApproved)
            .OrderBy(x => x.Id)
            .ToListAsync();

        var bookable = salons
            .Select(salon => new BookableSalon(salon, BookingOptions(salon)))
            .Where(x => x.Options.Count > 0)
            .ToList();
        var categoryIds = bookable.Select(x => x.Salon.SalonCategoryId).Distinct().Order().ToList();

        var random = new Random(RandomSeed);
        var nextFreeDay = new Dictionary<int, DateTime>();
        for (var i = 0; i < CustomerCount; i++)
        {
            var number = FirstCustomerNumber + i;
            var customer = new User
            {
                Username = $"customer{number}",
                FirstName = FirstNames[i % FirstNames.Length],
                LastName = "Kupac",
                Email = $"customer{number}@cutcal.com",
                Phone = $"+3876120{number:0000}",
                PasswordHash = CutCalSeed.SeedPasswordHash,
                ProfileImageUrl = $"/images/avatars/{i % AvatarCount + 1}.jpg",
                UserRoles = { new UserRole { RoleId = customerRoleId } }
            };
            context.Users.Add(customer);

            var persona = i % categoryIds.Count;
            var likedCategories = new[] { categoryIds[persona], categoryIds[(persona + 1) % categoryIds.Count] };
            var liked = Shuffled(bookable.Where(x => likedCategories.Contains(x.Salon.SalonCategoryId)), random);
            var others = Shuffled(bookable.Where(x => !likedCategories.Contains(x.Salon.SalonCategoryId)), random);

            foreach (var salon in liked.Take(LikedBookings).Concat(others.Take(OtherBookings)))
            {
                context.Appointments.Add(NewVisit(customer, salon, random, nextFreeDay));
            }
            foreach (var salon in liked.Take(FavoriteCount))
            {
                context.Favorites.Add(new Favorite { User = customer, SalonId = salon.Salon.Id });
            }
            foreach (var salon in liked.Take(LikedViews).Concat(others.Take(OtherViews)))
            {
                context.SalonViews.Add(new SalonView { User = customer, SalonId = salon.Salon.Id });
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Recommender demo data: added {Customers} customers with their bookings, favourites and salon views.", CustomerCount);
    }

    private static List<(Staff Staff, SalonService Service)> BookingOptions(Salon salon) =>
        salon.Staff.Where(x => x.IsActive)
            .SelectMany(staff => salon.Services
                .Where(service => service.IsActive && staff.StaffServices.Any(link => link.ServiceId == service.Id))
                .Select(service => (staff, service)))
            .ToList();

    private static List<BookableSalon> Shuffled(IEnumerable<BookableSalon> salons, Random random) =>
        salons.OrderBy(_ => random.Next()).ToList();

    /// <summary>One completed visit per salon per weekday, so a salon's staff are never double-booked.</summary>
    private static Appointment NewVisit(User customer, BookableSalon bookable, Random random, Dictionary<int, DateTime> nextFreeDay)
    {
        var day = nextFreeDay.GetValueOrDefault(bookable.Salon.Id, FirstVisitDay);
        while (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            day = day.AddDays(1);
        }
        nextFreeDay[bookable.Salon.Id] = day.AddDays(1);

        var (staff, service) = bookable.Options[random.Next(bookable.Options.Count)];
        return new Appointment
        {
            Customer = customer,
            SalonId = bookable.Salon.Id,
            StaffId = staff.Id,
            ServiceId = service.Id,
            ScheduledAt = day,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            StateName = AppointmentStateNames.Completed,
            PaymentMethod = PaymentMethodNames.Cash,
            PaymentStatus = PaymentStatusNames.Paid,
            ApprovedById = bookable.Salon.OwnerId,
            ApprovedAt = day.AddDays(-1),
            CreatedAt = day.AddDays(-2)
        };
    }
}
