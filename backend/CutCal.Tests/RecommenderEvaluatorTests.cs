using CutCal.Recommender;

namespace CutCal.Tests;

public class RecommenderEvaluatorTests
{
    // Three tastes, each liking its own four salons; every customer used three of the four. After one is hidden two
    // remain, which is what lets the model see that salons of one taste are used together.
    private static readonly int[][] SalonGroups = { new[] { 1, 2, 3, 4 }, new[] { 5, 6, 7, 8 }, new[] { 9, 10, 11, 12 } };

    private static List<Interaction> ClearTastes(int usersPerGroup)
    {
        var interactions = new List<Interaction>();
        for (var group = 0; group < SalonGroups.Length; group++)
        {
            for (var u = 0; u < usersPerGroup; u++)
            {
                var skipped = SalonGroups[group][u % SalonGroups[group].Length];
                interactions.AddRange(SalonGroups[group].Where(id => id != skipped).Select(id => new Interaction(group * 100 + u + 1, id)));
            }
        }
        return interactions;
    }

    [Fact]
    public void Finds_the_hidden_salon_of_a_clear_taste_far_more_often_than_chance()
    {
        var result = RecommenderEvaluator.Evaluate(ClearTastes(usersPerGroup: 12))!;

        Assert.Equal(36, result.Users);
        Assert.Equal(RecommenderEvaluator.DefaultK, result.K);
        // Picking three of the ten candidates at random would hit 30% of the time.
        Assert.True(result.ModelHitRate >= 0.8, $"model HitRate@{result.K} was {result.ModelHitRate:P1}");
    }

    [Fact]
    public void Never_trains_on_the_salon_it_hides()
    {
        // Every customer has two salons nobody else uses. Without the hidden one the model knows nothing about it,
        // so any hit would mean the model had seen it.
        var exclusivePairs = Enumerable.Range(1, 15).SelectMany(user => new[] { new Interaction(user, user * 2), new Interaction(user, user * 2 + 1) });

        Assert.Equal(0, RecommenderEvaluator.Evaluate(exclusivePairs)!.ModelHitRate);
    }

    [Fact]
    public void Is_not_computed_when_too_few_customers_have_two_salons()
    {
        var fewCustomers = ClearTastes(usersPerGroup: 3);
        Assert.Null(RecommenderEvaluator.Evaluate(fewCustomers));
    }

    [Fact]
    public void Customers_with_a_single_salon_do_not_take_part()
    {
        var interactions = ClearTastes(usersPerGroup: 12).Concat(Enumerable.Range(1000, 20).Select(user => new Interaction(user, 1))).ToList();

        Assert.Equal(36, RecommenderEvaluator.Evaluate(interactions)!.Users);
    }

    [Fact]
    public void Gives_the_same_numbers_every_time_for_the_same_data()
    {
        var interactions = ClearTastes(usersPerGroup: 12);

        Assert.Equal(RecommenderEvaluator.Evaluate(interactions), RecommenderEvaluator.Evaluate(interactions));
    }

    [Fact]
    public void Does_not_depend_on_the_order_or_repetition_of_the_input()
    {
        var interactions = ClearTastes(usersPerGroup: 12);
        var shuffled = interactions.Concat(interactions).OrderByDescending(x => x.SalonId).ThenBy(x => x.UserId);

        Assert.Equal(RecommenderEvaluator.Evaluate(interactions), RecommenderEvaluator.Evaluate(shuffled));
    }

    [Fact]
    public void A_hit_rate_is_a_share_of_the_customers()
    {
        var result = RecommenderEvaluator.Evaluate(ClearTastes(usersPerGroup: 12), k: 5)!;

        Assert.InRange(result.ModelHitRate, 0, 1);
        Assert.InRange(result.PopularityHitRate, 0, 1);
    }
}
