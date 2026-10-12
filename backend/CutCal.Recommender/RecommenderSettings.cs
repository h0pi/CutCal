namespace CutCal.Recommender;

/// <summary>Where the trained model file lives, read from the environment (.env) once at start-up. The API and the worker must point at the same file.</summary>
public sealed record RecommenderSettings(string ModelPath)
{
    public static RecommenderSettings FromEnvironment() => new(
        Environment.GetEnvironmentVariable("Recommender__ModelPath") ?? "recommender-model/recommender.zip");
}
