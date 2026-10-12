using CutCal.Messaging;
using CutCal.Model.Constants;
using CutCal.Recommender;
using CutCal.Services.Database;
using CutCal.Services.Messaging;
using CutCal.Services.Services;
using CutCal.Worker.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

/// <summary>The recommender on the demo data: the worker's trainer builds the model, the service ranks with it.</summary>
public class RecommendationServiceTests : IAsyncLifetime
{
    private const string CollaborativeReason = "često ga biraju korisnici sličnih navika";
    private const int FirstTasteCustomerNumber = 11;
    private const int CustomersWithTastes = 45;

    private readonly string _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    private ServiceProvider _services = null!;
    private IServiceScope _scope = null!;
    private CutCalDbContext _context = null!;
    private SalonRecommenderModel _model = null!;
    private List<int> _tasteCustomerIds = null!;

    private sealed class NoPublisher : IRabbitMqPublisher
    {
        public Task PublishAsync(NotificationMessage message) => Task.CompletedTask;
        public Task RequestRecommenderRetrainAsync() => Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        var databaseName = Guid.NewGuid().ToString();
        _services = new ServiceCollection()
            .AddDbContext<CutCalDbContext>(options => options.UseInMemoryDatabase(databaseName))
            .BuildServiceProvider();
        _scope = _services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<CutCalDbContext>();
        await _context.Database.EnsureCreatedAsync();
        await RecommenderDemoDataSeeder.EnsureAsync(_context, NullLogger.Instance);

        var path = Path.Combine(_directory, "recommender.zip");
        await new RecommenderTrainer(_services.GetRequiredService<IServiceScopeFactory>(), new RecommenderSettings(path), NullLogger<RecommenderTrainer>.Instance)
            .TrainAsync(CancellationToken.None);
        _model = SalonRecommenderModel.Load(path);

        var usernames = Enumerable.Range(FirstTasteCustomerNumber, CustomersWithTastes).Select(number => $"customer{number}").ToList();
        _tasteCustomerIds = await _context.Users.Where(x => usernames.Contains(x.Username)).Select(x => x.Id).ToListAsync();
    }

    public Task DisposeAsync()
    {
        _scope.Dispose();
        _services.Dispose();
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
        return Task.CompletedTask;
    }

    private RecommendationService ServiceWith(SalonRecommenderModel? model, IMemoryCache? cache = null) =>
        new(_context, new NoPublisher(), new FixedModelProvider(model), cache ?? new MemoryCache(new MemoryCacheOptions()));

    private static Appointment CompletedBooking(int salonId) => new()
    {
        CustomerId = 1,
        SalonId = salonId,
        StaffId = 1,
        ServiceId = 1,
        ScheduledAt = DateTime.UtcNow,
        DurationMinutes = 30,
        Price = 20,
        StateName = AppointmentStateNames.Completed,
        PaymentMethod = PaymentMethodNames.Cash,
        PaymentStatus = PaymentStatusNames.Paid
    };

    private async Task<HashSet<int>> UsedSalonsAsync(int customerId)
    {
        var booked = await _context.Appointments.Where(x => x.CustomerId == customerId).Select(x => x.SalonId).ToListAsync();
        var favourites = await _context.Favorites.Where(x => x.UserId == customerId).Select(x => x.SalonId).ToListAsync();
        var viewed = await _context.SalonViews.Where(x => x.UserId == customerId).Select(x => x.SalonId).ToListAsync();
        return booked.Concat(favourites).Concat(viewed).ToHashSet();
    }

    [Fact]
    public async Task Keeps_serving_the_salons_it_loaded_until_the_cache_expires()
    {
        var service = ServiceWith(_model);
        var customerId = _tasteCustomerIds[0];
        var salonId = (await service.GetRecommendationsAsync(customerId, null, null))[0].Salon.Id;

        var salon = await _context.Salons.SingleAsync(x => x.Id == salonId);
        salon.IsApproved = false;
        await _context.SaveChangesAsync();

        Assert.Contains(await service.GetRecommendationsAsync(customerId, null, null), x => x.Salon.Id == salonId);
        Assert.DoesNotContain(await ServiceWith(_model).GetRecommendationsAsync(customerId, null, null), x => x.Salon.Id == salonId);
    }

    [Fact]
    public async Task Keeps_using_the_booking_counts_it_loaded_until_the_cache_expires()
    {
        var service = ServiceWith(_model);
        var customerId = _tasteCustomerIds[0];
        var before = await service.GetRecommendationsAsync(customerId, null, null);
        var salonId = before[^1].Salon.Id;
        var scoreBefore = before.Single(x => x.Salon.Id == salonId).Score;

        _context.Appointments.AddRange(Enumerable.Range(0, 100).Select(_ => CompletedBooking(salonId)));
        await _context.SaveChangesAsync();

        var cached = (await service.GetRecommendationsAsync(customerId, null, null)).Single(x => x.Salon.Id == salonId).Score;
        var fresh = (await ServiceWith(_model).GetRecommendationsAsync(customerId, null, null)).Single(x => x.Salon.Id == salonId).Score;
        Assert.Equal(scoreBefore, cached);
        Assert.NotEqual(scoreBefore, fresh);
    }

    [Fact]
    public async Task Without_a_model_everyone_gets_the_same_popularity_and_quality_based_list()
    {
        var service = ServiceWith(null);

        var first = await service.GetRecommendationsAsync(_tasteCustomerIds[0], null, null);
        var other = await service.GetRecommendationsAsync(_tasteCustomerIds[2], null, null);

        Assert.Equal(first.Select(x => x.Salon.Id), other.Select(x => x.Salon.Id));
        Assert.DoesNotContain(first, x => x.Reason.Contains(CollaborativeReason));
    }

    [Fact]
    public async Task With_a_model_customers_with_different_tastes_get_different_lists()
    {
        var service = ServiceWith(_model);

        var lists = new List<string>();
        foreach (var customerId in _tasteCustomerIds)
        {
            lists.Add(string.Join(",", (await service.GetRecommendationsAsync(customerId, null, null)).Select(x => x.Salon.Id)));
        }

        Assert.True(lists.Distinct().Count() > 5, "the model should personalise the lists");
    }

    [Fact]
    public async Task With_a_model_the_salons_a_customer_already_likes_rank_higher_than_without_one()
    {
        var cold = ServiceWith(null);
        var learned = ServiceWith(_model);

        var totalWithoutModel = 0;
        var totalWithModel = 0;
        foreach (var customerId in _tasteCustomerIds)
        {
            var used = await UsedSalonsAsync(customerId);
            var withoutModel = (await cold.GetRecommendationsAsync(customerId, null, null)).Count(x => used.Contains(x.Salon.Id));
            var withModel = (await learned.GetRecommendationsAsync(customerId, null, null)).Count(x => used.Contains(x.Salon.Id));
            Assert.True(withModel >= withoutModel, $"customer {customerId}: {withModel} of their salons with the model, {withoutModel} without");
            totalWithoutModel += withoutModel;
            totalWithModel += withModel;
        }

        Assert.True(totalWithModel > totalWithoutModel, $"{totalWithModel} of the customers' salons with the model, {totalWithoutModel} without");
    }

    [Fact]
    public async Task A_customer_the_model_has_never_seen_gets_the_cold_start_list()
    {
        var newcomer = new User
        {
            Username = "newcomer",
            FirstName = "New",
            LastName = "Customer",
            Email = "newcomer@cutcal.com",
            PasswordHash = "not-a-real-hash"
        };
        _context.Users.Add(newcomer);
        await _context.SaveChangesAsync();

        var withModel = await ServiceWith(_model).GetRecommendationsAsync(newcomer.Id, null, null);
        var withoutModel = await ServiceWith(null).GetRecommendationsAsync(newcomer.Id, null, null);

        Assert.Equal(withoutModel.Select(x => (x.Salon.Id, x.Score)), withModel.Select(x => (x.Salon.Id, x.Score)));
    }

    [Fact]
    public async Task Says_customers_with_similar_habits_choose_a_salon_only_for_salons_the_customer_has_not_used()
    {
        var service = ServiceWith(_model);
        var mentioned = 0;

        foreach (var customerId in _tasteCustomerIds)
        {
            var used = await UsedSalonsAsync(customerId);
            foreach (var recommendation in await service.GetRecommendationsAsync(customerId, null, null))
            {
                if (!recommendation.Reason.Contains(CollaborativeReason))
                {
                    continue;
                }
                mentioned++;
                Assert.DoesNotContain(recommendation.Salon.Id, used);
            }
        }

        Assert.True(mentioned > 0, "the collaborative explanation should appear for some salons");
    }
}
