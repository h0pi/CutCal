using System.Text;
using System.Text.Json;
using CutCal.Worker.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CutCal.Worker.Messaging;

public class RabbitMqConsumerService : BackgroundService
{
    private const string QueueName = "cutcal.notifications";
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delays = new[] { 1000, 2000, 4000, 8000 };
        var attempt = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAndConsumeAsync(stoppingToken);
                attempt = 0;
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                var delay = delays[Math.Min(attempt, delays.Length - 1)];
                _logger.LogError(ex, "RabbitMQ connection failed, retrying in {Delay}ms", delay);
                attempt++;
                await Task.Delay(delay, stoppingToken);
            }
        }
    }

    private async Task ConnectAndConsumeAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("RabbitMQ__Port"), out var port) ? port : 5672,
            UserName = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(QueueName, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
        await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await _channel.QueueBindAsync(QueueName, QueueName, "#", cancellationToken: stoppingToken);
        await _channel.BasicQosAsync(0, 10, false, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                await HandleMessageAsync(ea.Body.ToArray());
                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message, requeueing.");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(QueueName, autoAck: false, consumer, stoppingToken);

        _logger.LogInformation("RabbitMQ consumer connected and listening on '{Queue}'", QueueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task HandleMessageAsync(byte[] body)
    {
        var json = Encoding.UTF8.GetString(body);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var type = root.TryGetProperty("Type", out var typeProp) ? typeProp.GetString() : null;
        if (string.IsNullOrEmpty(type))
        {
            _logger.LogWarning("Received message without a Type field: {Json}", json);
            return;
        }

        var email = root.TryGetProperty("CustomerEmail", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;
        var salonName = root.TryGetProperty("SalonName", out var salonProp) ? salonProp.GetString() ?? string.Empty : string.Empty;
        var reason = root.TryGetProperty("Reason", out var reasonProp) ? reasonProp.GetString() : null;
        var scheduledAt = root.TryGetProperty("ScheduledAt", out var scheduledProp) && scheduledProp.TryGetDateTime(out var dt)
            ? dt
            : DateTime.UtcNow;

        using var scope = _scopeFactory.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        switch (type)
        {
            case "AppointmentConfirmed":
                await emailService.SendAppointmentConfirmed(email, "Customer", salonName, scheduledAt);
                break;
            case "AppointmentCancelled":
                await emailService.SendAppointmentCancelled(email, "Customer", salonName, reason);
                break;
            case "AppointmentReminder":
                await emailService.SendAppointmentReminder(email, "Customer", salonName, scheduledAt);
                break;
            default:
                _logger.LogInformation("No handler configured for notification type '{Type}'", type);
                break;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.CloseAsync(cancellationToken);
        if (_connection is not null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
