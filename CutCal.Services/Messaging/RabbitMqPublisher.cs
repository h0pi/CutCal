using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CutCal.Services.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(string routingKey, T message);
}

public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private const string ExchangeName = "cutcal.notifications";
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly Task<IConnection> _connectionTask;
    private readonly ConnectionFactory _factory;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
        _factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RabbitMQ__Port"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest"
        };
        _connectionTask = _factory.CreateConnectionAsync();
    }

    public async Task PublishAsync<T>(string routingKey, T message)
    {
        try
        {
            var connection = await _connectionTask;
            await using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: true);
            await channel.QueueDeclareAsync(ExchangeName, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(ExchangeName, ExchangeName, routingKey);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var props = new BasicProperties { Persistent = true, Type = routingKey };
            await channel.BasicPublishAsync(ExchangeName, routingKey, mandatory: false, props, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish RabbitMQ message with routing key {RoutingKey}", routingKey);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connectionTask.IsCompletedSuccessfully)
        {
            var connection = await _connectionTask;
            await connection.DisposeAsync();
        }
    }
}
