using CutCal.Model.Constants;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class RecommenderDemoDataSeederTests
{
    private const int AddedCustomers = 45;
    private const int BookingsPerCustomer = 4;
    private const int FavoritesPerCustomer = 2;
    private const int ViewsPerCustomer = 7;

    private static async Task<CutCalDbContext> NewSeededContextAsync()
    {
        var options = new DbContextOptionsBuilder<CutCalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new CutCalDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    private static async Task<List<int>> SeedAndGetNewCustomerIdsAsync(CutCalDbContext context)
    {
        var existing = await context.Users.Select(x => x.Id).ToListAsync();
        await RecommenderDemoDataSeeder.EnsureAsync(context, NullLogger.Instance);
        return await context.Users.Where(x => !existing.Contains(x.Id)).Select(x => x.Id).ToListAsync();
    }

    [Fact]
    public async Task Adds_customers_with_the_customer_role()
    {
        await using var context = await NewSeededContextAsync();
        var customerRoleId = await context.Roles.Where(x => x.Name == RoleNames.Customer).Select(x => x.Id).SingleAsync();

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        Assert.Equal(AddedCustomers, newIds.Count);
        var roleIds = await context.UserRoles.Where(x => newIds.Contains(x.UserId)).Select(x => x.RoleId).ToListAsync();
        Assert.Equal(AddedCustomers, roleIds.Count);
        Assert.All(roleIds, roleId => Assert.Equal(customerRoleId, roleId));
    }

    [Fact]
    public async Task Gives_every_customer_a_profile_picture_from_the_shipped_avatars()
    {
        await using var context = await NewSeededContextAsync();

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        var pictures = await context.Users.Where(x => newIds.Contains(x.Id)).Select(x => x.ProfileImageUrl).ToListAsync();
        Assert.All(pictures, picture => Assert.Matches(@"^/images/avatars/([1-9]|[1-3][0-9]|4[0-7])\.jpg$", picture));
        Assert.True(pictures.Distinct().Count() > 10, "the customers should not all share one picture");
    }

    [Fact]
    public async Task Gives_every_customer_bookings_favourites_and_views()
    {
        await using var context = await NewSeededContextAsync();

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        var bookings = await context.Appointments.Where(x => newIds.Contains(x.CustomerId)).GroupBy(x => x.CustomerId).Select(g => g.Count()).ToListAsync();
        var favorites = await context.Favorites.Where(x => newIds.Contains(x.UserId)).GroupBy(x => x.UserId).Select(g => g.Count()).ToListAsync();
        var views = await context.SalonViews.Where(x => newIds.Contains(x.UserId)).GroupBy(x => x.UserId).Select(g => g.Count()).ToListAsync();

        Assert.Equal(AddedCustomers, bookings.Count);
        Assert.All(bookings, count => Assert.Equal(BookingsPerCustomer, count));
        Assert.Equal(AddedCustomers, favorites.Count);
        Assert.All(favorites, count => Assert.Equal(FavoritesPerCustomer, count));
        Assert.Equal(AddedCustomers, views.Count);
        Assert.All(views, count => Assert.Equal(ViewsPerCustomer, count));
    }

    [Fact]
    public async Task Bookings_are_completed_and_paid()
    {
        await using var context = await NewSeededContextAsync();

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        var bookings = await context.Appointments.Where(x => newIds.Contains(x.CustomerId)).ToListAsync();
        Assert.All(bookings, booking =>
        {
            Assert.Equal(AppointmentStateNames.Completed, booking.StateName);
            Assert.Equal(PaymentStatusNames.Paid, booking.PaymentStatus);
        });
    }

    [Fact]
    public async Task Customers_book_mostly_within_two_salon_categories()
    {
        await using var context = await NewSeededContextAsync();
        var categoryBySalon = await context.Salons.ToDictionaryAsync(x => x.Id, x => x.SalonCategoryId);

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        var bookingsByCustomer = (await context.Appointments.Where(x => newIds.Contains(x.CustomerId)).ToListAsync())
            .GroupBy(x => x.CustomerId);
        Assert.All(bookingsByCustomer, customerBookings =>
        {
            var inTopTwoCategories = customerBookings
                .GroupBy(x => categoryBySalon[x.SalonId])
                .Select(g => g.Count())
                .OrderDescending()
                .Take(2)
                .Sum();
            Assert.True(inTopTwoCategories >= BookingsPerCustomer - 1);
        });
    }

    [Fact]
    public async Task Never_double_books_a_staff_member()
    {
        await using var context = await NewSeededContextAsync();

        var newIds = await SeedAndGetNewCustomerIdsAsync(context);

        var all = await context.Appointments.Select(x => new { x.Id, x.CustomerId, x.StaffId, x.ScheduledAt, x.DurationMinutes }).ToListAsync();
        foreach (var added in all.Where(x => newIds.Contains(x.CustomerId)))
        {
            var addedEnd = added.ScheduledAt.AddMinutes(added.DurationMinutes);
            Assert.DoesNotContain(all, other =>
                other.Id != added.Id
                && other.StaffId == added.StaffId
                && other.ScheduledAt < addedEnd
                && added.ScheduledAt < other.ScheduledAt.AddMinutes(other.DurationMinutes));
        }
    }

    [Fact]
    public async Task Running_twice_adds_nothing_the_second_time()
    {
        await using var context = await NewSeededContextAsync();
        await SeedAndGetNewCustomerIdsAsync(context);
        var users = await context.Users.CountAsync();
        var appointments = await context.Appointments.CountAsync();
        var favorites = await context.Favorites.CountAsync();
        var views = await context.SalonViews.CountAsync();

        await RecommenderDemoDataSeeder.EnsureAsync(context, NullLogger.Instance);

        Assert.Equal(users, await context.Users.CountAsync());
        Assert.Equal(appointments, await context.Appointments.CountAsync());
        Assert.Equal(favorites, await context.Favorites.CountAsync());
        Assert.Equal(views, await context.SalonViews.CountAsync());
    }
}
