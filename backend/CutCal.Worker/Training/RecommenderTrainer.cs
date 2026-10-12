using System.Diagnostics;
using CutCal.Model.Constants;
using CutCal.Recommender;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Worker.Training;

/// <summary>
/// Trains the salon recommender from the bookings, favourites and salon views in the database and replaces the model
/// file the API reads. A booking the customer rated one or two stars is not a sign they like the salon, so it is left out.
/// </summary>
public sealed class RecommenderTrainer
{
    private const int DislikedRatingMax = 2;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RecommenderSettings _settings;
    private readonly ILogger<RecommenderTrainer> _logger;

    public RecommenderTrainer(IServiceScopeFactory scopeFactory, RecommenderSettings settings, ILogger<RecommenderTrainer> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings;
        _logger = logger;
    }

    public async Task TrainAsync(CancellationToken cancellationToken)
    {
        var interactions = await LoadInteractionsAsync(cancellationToken);
        if (interactions.Count == 0)
        {
            _logger.LogWarning("There are no bookings, favourites or salon views yet, so no recommender model was trained.");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        SalonRecommenderModel.Train(interactions).Save(_settings.ModelPath);
        _logger.LogInformation(
            "Recommender model trained on {Interactions} interactions from {Users} users and {Salons} salons in {Elapsed} ms and saved to {Path}.",
            interactions.Count, interactions.Select(x => x.UserId).Distinct().Count(), interactions.Select(x => x.SalonId).Distinct().Count(),
            stopwatch.ElapsedMilliseconds, _settings.ModelPath);

        LogOfflineEvaluation(interactions);
    }

    /// <summary>Judging the model must never undo a successful training, so a failure here is only logged.</summary>
    private void LogOfflineEvaluation(List<Interaction> interactions)
    {
        try
        {
            var result = RecommenderEvaluator.Evaluate(interactions);
            if (result is null)
            {
                _logger.LogInformation("Offline evaluation skipped: fewer than {MinUsers} customers have two or more salons.", RecommenderEvaluator.MinUsers);
                return;
            }

            _logger.LogInformation(
                "Offline evaluation (leave-one-out, {Users} customers): HitRate@{K} is {Model:P1} for the model and {Popularity:P1} for the popularity baseline.",
                result.Users, result.K, result.ModelHitRate, result.PopularityHitRate);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "The offline evaluation of the recommender failed; the trained model is not affected.");
        }
    }

    private async Task<List<Interaction>> LoadInteractionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CutCalDbContext>();

        var bookings = await context.Appointments
            .Where(x => x.StateName != AppointmentStateNames.Cancelled && (x.Review == null || x.Review.Rating > DislikedRatingMax))
            .Select(x => new { UserId = x.CustomerId, x.SalonId })
            .Distinct()
            .ToListAsync(cancellationToken);
        var favorites = await context.Favorites.Select(x => new { x.UserId, x.SalonId }).Distinct().ToListAsync(cancellationToken);
        var views = await context.SalonViews.Select(x => new { x.UserId, x.SalonId }).Distinct().ToListAsync(cancellationToken);

        return bookings.Concat(favorites).Concat(views)
            .Select(x => new Interaction(x.UserId, x.SalonId))
            .Distinct()
            .ToList();
    }
}
