namespace CutCal.Messaging;

/// <summary>RabbitMQ connection settings, read from the environment (.env) once at start-up.</summary>
public sealed record RabbitMqSettings(string Host, int Port, string Username, string Password)
{
    private const int DefaultPort = 5672;

    public static RabbitMqSettings FromEnvironment() => new(
        Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost",
        int.TryParse(Environment.GetEnvironmentVariable("RabbitMQ__Port"), out var port) ? port : DefaultPort,
        Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest",
        Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest");
}
