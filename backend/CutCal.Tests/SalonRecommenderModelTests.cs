using CutCal.Recommender;

namespace CutCal.Tests;

public class SalonRecommenderModelTests : IDisposable
{
    // Three tastes, each liking its own three salons. Every user has used two of the three and never the third.
    private static readonly int[][] SalonGroups = { new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, new[] { 7, 8, 9 } };
    private const int UsersPerGroup = 12;

    private readonly string _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private static (List<Interaction> Interactions, Dictionary<int, int> NeverUsed) TasteData()
    {
        var interactions = new List<Interaction>();
        var neverUsed = new Dictionary<int, int>();
        for (var group = 0; group < SalonGroups.Length; group++)
        {
            for (var u = 0; u < UsersPerGroup; u++)
            {
                var userId = group * 100 + u + 1;
                var skipped = SalonGroups[group][u % SalonGroups[group].Length];
                neverUsed[userId] = skipped;
                interactions.AddRange(SalonGroups[group].Where(id => id != skipped).Select(id => new Interaction(userId, id)));
            }
        }
        return (interactions, neverUsed);
    }

    private static readonly int[] AllSalonIds = SalonGroups.SelectMany(x => x).ToArray();

    [Fact]
    public void Ranks_an_unused_salon_of_the_users_taste_above_salons_of_other_tastes()
    {
        var (interactions, neverUsed) = TasteData();
        var model = SalonRecommenderModel.Train(interactions);

        foreach (var (userId, salonId) in neverUsed)
        {
            var scores = model.Score(userId, AllSalonIds);
            var ownGroup = SalonGroups.Single(group => group.Contains(salonId));
            var bestOfOtherTastes = AllSalonIds.Except(ownGroup).Max(id => scores[id]);
            Assert.True(scores[salonId] > bestOfOtherTastes, $"user {userId}: salon {salonId} scored {scores[salonId]}, other tastes up to {bestOfOtherTastes}");
        }
    }

    [Fact]
    public void Scores_only_users_and_salons_it_was_trained_on()
    {
        var model = SalonRecommenderModel.Train(TasteData().Interactions);

        Assert.Empty(model.Score(userId: 9999, AllSalonIds));
        Assert.Equal(new[] { 1, 2 }, model.Score(userId: 1, new[] { 1, 2, 2, 777 }).Keys.Order());
    }

    [Fact]
    public void Training_twice_on_the_same_data_gives_the_same_scores()
    {
        var interactions = TasteData().Interactions;

        var first = SalonRecommenderModel.Train(interactions).Score(userId: 1, AllSalonIds);
        var second = SalonRecommenderModel.Train(interactions).Score(userId: 1, AllSalonIds);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Counts_repeated_interactions_once()
    {
        var interactions = TasteData().Interactions;

        var once = SalonRecommenderModel.Train(interactions).Score(userId: 1, AllSalonIds);
        var repeated = SalonRecommenderModel.Train(interactions.Concat(interactions)).Score(userId: 1, AllSalonIds);

        Assert.Equal(once, repeated);
    }

    [Fact]
    public void A_saved_model_scores_exactly_like_the_one_that_was_saved()
    {
        var model = SalonRecommenderModel.Train(TasteData().Interactions);
        var path = Path.Combine(_directory, "recommender.zip");

        model.Save(path);
        var loaded = SalonRecommenderModel.Load(path);

        Assert.Equal(model.Score(userId: 1, AllSalonIds), loaded.Score(userId: 1, AllSalonIds));
        Assert.Equal(model.Score(userId: 205, AllSalonIds), loaded.Score(userId: 205, AllSalonIds));
        Assert.Empty(loaded.Score(userId: 9999, AllSalonIds));
    }

    [Fact]
    public void Saving_again_replaces_the_file_and_leaves_no_temporary_files()
    {
        var model = SalonRecommenderModel.Train(TasteData().Interactions);
        var path = Path.Combine(_directory, "recommender.zip");

        model.Save(path);
        model.Save(path);

        Assert.Equal(new[] { path }, Directory.GetFiles(_directory));
    }

    [Fact]
    public void Cannot_be_trained_without_interactions()
    {
        Assert.Throws<ArgumentException>(() => SalonRecommenderModel.Train(Array.Empty<Interaction>()));
    }
}
