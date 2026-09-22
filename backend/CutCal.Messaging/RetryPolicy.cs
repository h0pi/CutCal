namespace CutCal.Messaging;

/// <summary>Exponential backoff 1 s, 2 s, 4 s, 8 s; after the last delay a message is moved to the dead-letter queue.</summary>
public static class RetryPolicy
{
    private static readonly TimeSpan[] BackoffDelays =
    {
        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8)
    };

    /// <summary>Delay before retry number <paramref name="failedAttempts"/> + 1, or null when the retries are used up.</summary>
    public static TimeSpan? DelayAfter(int failedAttempts) =>
        failedAttempts >= 0 && failedAttempts < BackoffDelays.Length ? BackoffDelays[failedAttempts] : null;

    /// <summary>Delay between connection attempts; grows like the retries but never beyond the longest one.</summary>
    public static TimeSpan ConnectionDelay(int attempt) => BackoffDelays[Math.Clamp(attempt, 0, BackoffDelays.Length - 1)];
}
