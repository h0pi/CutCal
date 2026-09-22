using RabbitMQ.Client;

namespace CutCal.Messaging;

/// <summary>Names and declaration of the RabbitMQ exchange and queues, shared by the API and the worker.</summary>
public static class MessagingTopology
{
    public const string ExchangeName = "cutcal.notifications";
    public const string EmailQueueName = "cutcal.notifications.email";
    public const string DeadLetterQueueName = "cutcal.notifications.email.dead";

    public const string RetryCountHeader = "x-retry-count";
    public const string ErrorHeader = "x-error";

    /// <summary>Idempotent: safe to call from both services, in any start-up order.</summary>
    public static async Task DeclareAsync(IChannel channel, CancellationToken cancellationToken = default)
    {
        await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(EmailQueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

        foreach (var messageType in NotificationMessageTypes.All)
        {
            await channel.QueueBindAsync(EmailQueueName, ExchangeName, messageType, cancellationToken: cancellationToken);
        }
    }
}
