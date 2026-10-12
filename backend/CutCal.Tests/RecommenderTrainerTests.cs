using CutCal.Model.Constants;
using CutCal.Recommender;
using CutCal.Services.Database;
using CutCal.Worker.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class RecommenderTrainerTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    private readonly ServiceProvider _services;
    private readonly IServiceScope _scope;
    private readonly CutCalDbContext _context;
    private readonly ListLogger<RecommenderTrainer> _log = new();
    private readonly RecommenderTrainer _trainer;

    private string ModelPath => Path.Combine(_directory, "recommender.zip");

    public RecommenderTrainerTests()
    {
        var databaseName = Guid.NewGuid().ToString();
        _services = new ServiceCollection()
            .AddDbContext<CutCalDbContext>(options => options.UseInMemoryDatabase(databaseName))
            .BuildServiceProvider();
        _scope = _services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<CutCalDbContext>();
        _trainer = new RecommenderTrainer(
            _services.GetRequiredService<IServiceScopeFactory>(), new RecommenderSettings(ModelPath), _log);
    }

    public void Dispose()
    {
        _scope.Dispose();
        _services.Dispose();
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private static Appointment Booking(int userId, int salonId, string state) => new()
    {
        CustomerId = userId,
        SalonId = salonId,
        StaffId = 1,
        ServiceId = 1,
        ScheduledAt = DateTime.UtcNow,
        DurationMinutes = 30,
        Price = 20,
        StateName = state,
        PaymentMethod = PaymentMethodNames.Cash,
        PaymentStatus = PaymentStatusNames.Unpaid
    };

    [Fact]
    public async Task Learns_from_bookings_favourites_and_views_but_not_from_cancelled_bookings()
    {
        _context.Appointments.AddRange(
            Booking(userId: 1, salonId: 1, AppointmentStateNames.Completed),
            Booking(userId: 2, salonId: 1, AppointmentStateNames.Cancelled),
            Booking(userId: 3, salonId: 4, AppointmentStateNames.Pending));
        _context.Favorites.Add(new Favorite { UserId = 1, SalonId = 2 });
        _context.SalonViews.Add(new SalonView { UserId = 1, SalonId = 3 });
        await _context.SaveChangesAsync();

        await _trainer.TrainAsync(CancellationToken.None);

        var model = SalonRecommenderModel.Load(ModelPath);
        var salonIds = new[] { 1, 2, 3, 4 };
        Assert.Equal(salonIds, model.Score(userId: 1, salonIds).Keys.Order());
        Assert.NotEmpty(model.Score(userId: 3, salonIds));
        Assert.Empty(model.Score(userId: 2, salonIds));
    }

    [Fact]
    public async Task Does_not_count_a_booking_the_customer_rated_one_or_two_stars()
    {
        _context.Appointments.AddRange(
            Booking(userId: 1, salonId: 1, AppointmentStateNames.Completed),
            Booking(userId: 1, salonId: 2, AppointmentStateNames.Completed),
            Booking(userId: 1, salonId: 3, AppointmentStateNames.Completed));
        await _context.SaveChangesAsync();
        var appointments = await _context.Appointments.OrderBy(x => x.SalonId).ToListAsync();
        _context.Reviews.AddRange(
            new Review { AppointmentId = appointments[0].Id, CustomerId = 1, SalonId = 1, Rating = 1, CreatedAt = DateTime.UtcNow },
            new Review { AppointmentId = appointments[1].Id, CustomerId = 1, SalonId = 2, Rating = 3, CreatedAt = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        await _trainer.TrainAsync(CancellationToken.None);

        var scores = SalonRecommenderModel.Load(ModelPath).Score(userId: 1, new[] { 1, 2, 3 });
        Assert.Equal(new[] { 2, 3 }, scores.Keys.Order());
    }

    [Fact]
    public async Task Logs_an_offline_evaluation_in_which_the_model_beats_the_popularity_baseline()
    {
        await _context.Database.EnsureCreatedAsync();
        await RecommenderDemoDataSeeder.EnsureAsync(_context, NullLogger.Instance);

        await _trainer.TrainAsync(CancellationToken.None);

        var evaluation = Assert.Single(_log.Entries, x => x.Message.StartsWith("Offline evaluation (leave-one-out"));
        Assert.True((double)evaluation.Values["Model"]! > (double)evaluation.Values["Popularity"]!, evaluation.Message);
    }

    [Fact]
    public async Task Skips_the_offline_evaluation_when_there_are_too_few_customers()
    {
        _context.Appointments.Add(Booking(userId: 1, salonId: 1, AppointmentStateNames.Completed));
        await _context.SaveChangesAsync();

        await _trainer.TrainAsync(CancellationToken.None);

        Assert.Single(_log.Entries, x => x.Message.StartsWith("Offline evaluation skipped"));
        Assert.True(File.Exists(ModelPath));
    }

    [Fact]
    public async Task Does_not_write_a_model_when_there_is_nothing_to_learn_from()
    {
        await _trainer.TrainAsync(CancellationToken.None);

        Assert.False(File.Exists(ModelPath));
    }

    [Fact]
    public async Task Trains_a_model_that_scores_every_salon_for_the_demo_customers()
    {
        await _context.Database.EnsureCreatedAsync();
        await RecommenderDemoDataSeeder.EnsureAsync(_context, NullLogger.Instance);
        var salonIds = await _context.Salons.Select(x => x.Id).ToListAsync();
        var customerId = await _context.Users.Where(x => x.Username == "customer11").Select(x => x.Id).SingleAsync();

        await _trainer.TrainAsync(CancellationToken.None);

        var scores = SalonRecommenderModel.Load(ModelPath).Score(customerId, salonIds);
        Assert.Equal(salonIds.Count, scores.Count);
        Assert.DoesNotContain(scores.Values, float.IsNaN);
    }
}
