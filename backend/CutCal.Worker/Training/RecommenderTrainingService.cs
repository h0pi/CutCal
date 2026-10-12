namespace CutCal.Worker.Training;

/// <summary>
/// Trains the recommender when the worker starts and then once an hour. A failed attempt (for example the
/// database is still being migrated by the API) is logged and retried after a short pause, so the worker never
/// goes quiet and the model appears as soon as the data can be read.
/// </summary>
public class RecommenderTrainingService : BackgroundService
{
    private static readonly TimeSpan RetrainInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(30);

    private readonly RecommenderTrainer _trainer;
    private readonly ILogger<RecommenderTrainingService> _logger;

    public RecommenderTrainingService(RecommenderTrainer trainer, ILogger<RecommenderTrainingService> logger)
    {
        _trainer = trainer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (true)
            {
                var pause = RetrainInterval;
                try
                {
                    await _trainer.TrainAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Training the recommender model failed; retrying in {Delay}.", RetryDelay);
                    pause = RetryDelay;
                }

                await Task.Delay(pause, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Recommender training is shutting down.");
        }
    }
}
