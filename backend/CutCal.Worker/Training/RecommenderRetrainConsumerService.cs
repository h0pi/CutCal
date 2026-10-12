using CutCal.Messaging;
using CutCal.Worker.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CutCal.Worker.Training;

/// <summary>Retrains the recommender whenever the API publishes a retrain request, which is what an admin triggers to get a fresh model right away.</summary>
public class RecommenderRetrainConsumerService : BackgroundService
{
    private readonly RecommenderTrainer _trainer;
    private readonly ILogger<RecommenderRetrainConsumerService> _logger;

    public RecommenderRetrainConsumerService(RecommenderTrainer trainer, ILogger<RecommenderRetrainConsumerService> logger)
    {
        _trainer = trainer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await RabbitMqConnector.ConnectAsync(_logger, stoppingToken);
        if (connection is null)
        {
            return;
        }

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await MessagingTopology.DeclareAsync(channel, stoppingToken);
        await channel.BasicQosAsync(0, 1, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, delivery) => RetrainAsync(channel, delivery, stoppingToken);
        await channel.BasicConsumeAsync(MessagingTopology.RetrainQueueName, autoAck: false, consumer, stoppingToken);

        _logger.LogInformation("Worker is listening for recommender retrain requests on queue '{Queue}'.", MessagingTopology.RetrainQueueName);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Recommender retrain listener is shutting down.");
        }
    }

    private async Task RetrainAsync(IChannel channel, BasicDeliverEventArgs delivery, CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Recommender retrain requested.");
            await _trainer.TrainAsync(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Shutting down: leave the request unacknowledged so RabbitMQ delivers it again on the next start.
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retraining the recommender on request failed; the scheduled training will try again.");
        }

        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
    }
}
