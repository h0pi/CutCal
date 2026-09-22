using System.Text.Json;
using CutCal.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CutCal.Services.Messaging;

public interface IRabbitMqPublisher
{
    /// <summary>Publishes an e-mail notification. Never throws: a broken broker must not fail the user's booking.</summary>
    Task PublishAsync(NotificationMessage message);
}

/// <summary>
/// Publishes through one shared connection (never a new connection per message). If the connection is
/// lost, or the broker was down at start-up, the next publish opens a new one.
/// </summary>
public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly ConnectionFactory _factory;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
        var settings = RabbitMqSettings.FromEnvironment();
        _factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password
        };
    }

    public async Task PublishAsync(NotificationMessage message)
    {
        try
        {
            var connection = await GetConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Type = message.Type
            };
            await channel.BasicPublishAsync(MessagingTopology.ExchangeName, message.Type, mandatory: false, properties, JsonSerializer.SerializeToUtf8Bytes(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not publish {MessageType} for appointment {AppointmentId}; the customer will not get this e-mail.", message.Type, message.AppointmentId);
        }
    }

    private async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync();
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }

            var connection = await _factory.CreateConnectionAsync();

            // Declare exchange and queues once per connection, so messages are kept even if the worker is not running yet.
            await using (var channel = await connection.CreateChannelAsync())
            {
                await MessagingTopology.DeclareAsync(channel);
            }

            _connection = connection;
            return connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
        _connectionLock.Dispose();
    }
}
