using CutCal.Recommender;
using CutCal.Services.Recommendation;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class RecommenderModelProviderTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    private string ModelPath => Path.Combine(_directory, "recommender.zip");

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private RecommenderModelProvider NewProvider() => new(new RecommenderSettings(ModelPath), NullLogger<RecommenderModelProvider>.Instance);

    private static SalonRecommenderModel ModelWithUsers(params int[] userIds) =>
        SalonRecommenderModel.Train(userIds.SelectMany(user => new[] { new Interaction(user, 1), new Interaction(user, 2) }));

    [Fact]
    public void There_is_no_model_until_the_worker_has_written_one()
    {
        Assert.Null(NewProvider().Current);
    }

    [Fact]
    public void Loads_the_model_as_soon_as_the_file_appears()
    {
        var provider = NewProvider();
        Assert.Null(provider.Current);

        ModelWithUsers(7).Save(ModelPath);

        Assert.NotEmpty(provider.Current!.Score(userId: 7, new[] { 1, 2 }));
    }

    [Fact]
    public void Keeps_the_same_model_while_the_file_is_unchanged()
    {
        ModelWithUsers(7).Save(ModelPath);
        var provider = NewProvider();

        Assert.Same(provider.Current, provider.Current);
    }

    [Fact]
    public void Picks_up_a_newly_trained_model_without_a_restart()
    {
        ModelWithUsers(7).Save(ModelPath);
        var provider = NewProvider();
        Assert.Empty(provider.Current!.Score(userId: 8, new[] { 1, 2 }));

        ModelWithUsers(7, 8).Save(ModelPath);
        File.SetLastWriteTimeUtc(ModelPath, DateTime.UtcNow.AddMinutes(1));

        Assert.NotEmpty(provider.Current!.Score(userId: 8, new[] { 1, 2 }));
    }

    [Fact]
    public void Keeps_the_previous_model_when_the_new_file_cannot_be_read()
    {
        ModelWithUsers(7).Save(ModelPath);
        var provider = NewProvider();
        var previous = provider.Current;

        File.WriteAllText(ModelPath, "this is not a model");
        File.SetLastWriteTimeUtc(ModelPath, DateTime.UtcNow.AddMinutes(1));

        Assert.Same(previous, provider.Current);
    }

    [Fact]
    public void Goes_back_to_no_model_when_the_file_is_removed()
    {
        ModelWithUsers(7).Save(ModelPath);
        var provider = NewProvider();
        Assert.NotNull(provider.Current);

        File.Delete(ModelPath);

        Assert.Null(provider.Current);
    }
}
