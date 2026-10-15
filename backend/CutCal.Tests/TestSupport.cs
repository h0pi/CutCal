using CutCal.Messaging;
using CutCal.Recommender;
using CutCal.Services.Auth;
using CutCal.Services.Messaging;
using CutCal.Services.Recommendation;
using Microsoft.Extensions.Logging;

namespace CutCal.Tests;

/// <summary>Hands the service a fixed model (or none) instead of reading a model file.</summary>
internal sealed class FixedModelProvider : IRecommenderModelProvider
{
    public FixedModelProvider(SalonRecommenderModel? model) => Current = model;

    public SalonRecommenderModel? Current { get; }
}

/// <summary>Keeps what was logged, including the values behind the message template.</summary>
internal sealed class ListLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, string Message, Dictionary<string, object?> Values)> Entries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var values = state is IEnumerable<KeyValuePair<string, object?>> pairs ? pairs.ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, object?>();
        Entries.Add((logLevel, formatter(state, exception), values));
    }
}

/// <summary>Remembers the e-mail notifications the code asked to send.</summary>
internal sealed class RecordingPublisher : IRabbitMqPublisher
{
    public List<NotificationMessage> Sent { get; } = new();

    public Task PublishAsync(NotificationMessage message)
    {
        Sent.Add(message);
        return Task.CompletedTask;
    }

    public Task RequestRecommenderRetrainAsync() => Task.CompletedTask;
}

internal sealed class SignedInUser : IAuthenticatedUserAccessor
{
    public SignedInUser(int userId, string role)
    {
        UserId = userId;
        Role = role;
    }

    public int UserId { get; }
    public string? Role { get; }
    public bool IsInRole(string role) => role == Role;
}
