using CutCal.Messaging;
using RabbitMQ.Client;

namespace CutCal.Worker.Messaging;

/// <summary>
/// Opens the RabbitMQ connection of a worker consumer. It keeps trying (1 s, 2 s, 4 s, then every 8 s) until the broker
/// is reachable, logging every failure, and logs when the connection is lost and when it recovers.
/// Returns null only when the worker is shutting down.
/// </summary>
public static class RabbitMqConnector
{
    public static async Task<IConnection?> ConnectAsync(ILogger logger, CancellationToken stoppingToken)
    {
        var settings = RabbitMqSettings.FromEnvironment();
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            AutomaticRecoveryEnabled = true
        };

        for (var attempt = 0; !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                var connection = await factory.CreateConnectionAsync(stoppingToken);
                connection.ConnectionShutdownAsync += (_, args) =>
                {
                    logger.LogWarning("RabbitMQ connection lost ({Reason}); automatic recovery is running.", args.ReplyText);
                    return Task.CompletedTask;
                };
                connection.RecoverySucceededAsync += (_, _) =>
                {
                    logger.LogInformation("RabbitMQ connection recovered.");
                    return Task.CompletedTask;
                };
                return connection;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var delay = RetryPolicy.ConnectionDelay(attempt);
                logger.LogError(ex, "Cannot connect to RabbitMQ at {Host}:{Port} (attempt {Attempt}); retrying in {Delay}.", factory.HostName, factory.Port, attempt + 1, delay);
                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
            }
        }
        return null;
    }
}
