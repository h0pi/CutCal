using System.Text.Json;
using CutCal.Messaging;
using CutCal.Worker.Email;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CutCal.Worker.Messaging;

/// <summary>
/// Turns notification messages into e-mails. A message that fails is retried with exponential backoff
/// (1 s, 2 s, 4 s, 8 s) and then moved to the dead-letter queue, so one bad message can never loop forever
/// and nothing is dropped silently.
/// </summary>
public class RabbitMqConsumerService : BackgroundService
{
    private const ushort PrefetchCount = 10;

    private readonly ILogger<RabbitMqConsumerService> _logger;
    private readonly IEmailService _emailService;
    private readonly ConnectionFactory _factory;

    public RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;

        var settings = RabbitMqSettings.FromEnvironment();
        _factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            AutomaticRecoveryEnabled = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await ConnectWithRetryAsync(stoppingToken);
        if (connection is null)
        {
            return;
        }

        connection.ConnectionShutdownAsync += (_, args) =>
        {
            _logger.LogWarning("RabbitMQ connection lost ({Reason}); automatic recovery is running.", args.ReplyText);
            return Task.CompletedTask;
        };
        connection.RecoverySucceededAsync += (_, _) =>
        {
            _logger.LogInformation("RabbitMQ connection recovered.");
            return Task.CompletedTask;
        };

        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await MessagingTopology.DeclareAsync(channel, stoppingToken);
        await channel.BasicQosAsync(0, PrefetchCount, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, delivery) => ProcessAsync(channel, delivery, stoppingToken);
        await channel.BasicConsumeAsync(MessagingTopology.EmailQueueName, autoAck: false, consumer, stoppingToken);

        _logger.LogInformation("Worker is listening on queue '{Queue}'.", MessagingTopology.EmailQueueName);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker is shutting down.");
        }
    }

    /// <summary>Keeps trying (1 s, 2 s, 4 s, then every 8 s) until RabbitMQ is reachable, logging every failure.</summary>
    private async Task<IConnection?> ConnectWithRetryAsync(CancellationToken stoppingToken)
    {
        for (var attempt = 0; !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                return await _factory.CreateConnectionAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var delay = RetryPolicy.ConnectionDelay(attempt);
                _logger.LogError(ex, "Cannot connect to RabbitMQ at {Host}:{Port} (attempt {Attempt}); retrying in {Delay}.", _factory.HostName, _factory.Port, attempt + 1, delay);
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

    private async Task ProcessAsync(IChannel channel, BasicDeliverEventArgs delivery, CancellationToken stoppingToken)
    {
        NotificationMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<NotificationMessage>(delivery.Body.Span);
        }
        catch (JsonException ex)
        {
            await MoveToDeadLetterAsync(channel, delivery, $"Unreadable message: {ex.Message}", stoppingToken);
            return;
        }

        if (message is null || string.IsNullOrWhiteSpace(message.Type))
        {
            await MoveToDeadLetterAsync(channel, delivery, "Message has no type.", stoppingToken);
            return;
        }

        try
        {
            await SendEmailAsync(message);
            await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Shutting down: leave the message unacknowledged so RabbitMQ delivers it again on the next start.
        }
        catch (Exception ex)
        {
            await RetryOrDeadLetterAsync(channel, delivery, message, ex, stoppingToken);
        }
    }

    private async Task RetryOrDeadLetterAsync(IChannel channel, BasicDeliverEventArgs delivery, NotificationMessage message, Exception error, CancellationToken stoppingToken)
    {
        var failedAttempts = ReadRetryCount(delivery.BasicProperties);
        var delay = RetryPolicy.DelayAfter(failedAttempts);

        if (delay is null)
        {
            _logger.LogError(error, "{MessageType} for appointment {AppointmentId} failed {Attempts} times; moving it to the dead-letter queue.", message.Type, message.AppointmentId, failedAttempts + 1);
            await MoveToDeadLetterAsync(channel, delivery, error.Message, stoppingToken);
            return;
        }

        _logger.LogWarning(error, "{MessageType} for appointment {AppointmentId} failed (attempt {Attempt}); retrying in {Delay}.", message.Type, message.AppointmentId, failedAttempts + 1, delay);

        try
        {
            // Consumption is sequential on this channel, so waiting here also holds back later messages; acceptable for e-mail volume.
            await Task.Delay(delay.Value, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = delivery.BasicProperties.ContentType,
            Type = delivery.BasicProperties.Type,
            Headers = new Dictionary<string, object?> { [MessagingTopology.RetryCountHeader] = failedAttempts + 1 }
        };
        await channel.BasicPublishAsync(MessagingTopology.ExchangeName, delivery.RoutingKey, mandatory: false, properties, delivery.Body, stoppingToken);
        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
    }

    private async Task MoveToDeadLetterAsync(IChannel channel, BasicDeliverEventArgs delivery, string reason, CancellationToken stoppingToken)
    {
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = delivery.BasicProperties.ContentType,
            Type = delivery.BasicProperties.Type,
            Headers = new Dictionary<string, object?>
            {
                [MessagingTopology.RetryCountHeader] = ReadRetryCount(delivery.BasicProperties),
                [MessagingTopology.ErrorHeader] = reason
            }
        };
        await channel.BasicPublishAsync(string.Empty, MessagingTopology.DeadLetterQueueName, mandatory: false, properties, delivery.Body, stoppingToken);
        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
    }

    private static int ReadRetryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is not null && properties.Headers.TryGetValue(MessagingTopology.RetryCountHeader, out var value))
        {
            return value switch { int i => i, long l => (int)l, _ => 0 };
        }
        return 0;
    }

    private Task SendEmailAsync(NotificationMessage message) => message.Type switch
    {
        NotificationMessageTypes.AppointmentRequested => _emailService.SendAppointmentRequested(message),
        NotificationMessageTypes.AppointmentConfirmed => _emailService.SendAppointmentConfirmed(message),
        NotificationMessageTypes.AppointmentCancelled => _emailService.SendAppointmentCancelled(message),
        NotificationMessageTypes.AppointmentRescheduled => _emailService.SendAppointmentRescheduled(message),
        NotificationMessageTypes.PaymentReceived => _emailService.SendPaymentReceived(message),
        NotificationMessageTypes.PaymentRefunded => _emailService.SendPaymentRefunded(message),
        _ => throw new InvalidOperationException($"No e-mail template for message type '{message.Type}'.")
    };
}
