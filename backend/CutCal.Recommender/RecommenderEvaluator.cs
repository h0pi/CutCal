namespace CutCal.Recommender;

/// <param name="Users">Customers that took part: those with two or more salons.</param>
/// <param name="K">A salon counts as recommended when it is among the K best-ranked.</param>
/// <param name="ModelHitRate">Share of held-out salons the trained model ranked in its top K.</param>
/// <param name="PopularityHitRate">The same for the baseline that ranks salons by how many customers used them.</param>
public sealed record EvaluationResult(int Users, int K, double ModelHitRate, double PopularityHitRate);

/// <summary>
/// Offline evaluation by leave-one-out: for every customer with two or more salons one salon is hidden, a model is
/// trained on the rest, and the question is whether the hidden salon lands in the top K of the salons the customer
/// has not used. This is repeated in a few rounds (a different hidden salon each time) and averaged.
/// The model is compared with the simplest baseline, "recommend what is popular". It judges the learned model alone,
/// not the final ranking, which also weighs rating, distance and popularity.
/// </summary>
public static class RecommenderEvaluator
{
    public const int DefaultK = 3;
    public const int MinUsers = 10;

    private const int Rounds = 5;
    private const int Seed = 1;

    /// <summary>Returns null when too few customers have two or more salons for the numbers to mean anything.</summary>
    public static EvaluationResult? Evaluate(IEnumerable<Interaction> interactions, int k = DefaultK)
    {
        var distinct = interactions.Distinct().ToList();
        var salonsByUser = distinct.GroupBy(x => x.UserId).ToDictionary(g => g.Key, g => g.Select(x => x.SalonId).Order().ToList());
        var evaluatedUsers = salonsByUser.Where(x => x.Value.Count >= 2).Select(x => x.Key).Order().ToList();
        if (evaluatedUsers.Count < MinUsers)
        {
            return null;
        }

        var allSalons = distinct.Select(x => x.SalonId).Distinct().Order().ToList();
        var modelHits = 0;
        var popularityHits = 0;

        for (var round = 0; round < Rounds; round++)
        {
            var random = new Random(Seed + round);
            var hidden = evaluatedUsers.ToDictionary(user => user, user => salonsByUser[user][random.Next(salonsByUser[user].Count)]);

            var training = distinct.Where(x => !(hidden.TryGetValue(x.UserId, out var salonId) && salonId == x.SalonId)).ToList();
            var model = SalonRecommenderModel.Train(training);
            var popularity = training.GroupBy(x => x.SalonId).ToDictionary(g => g.Key, g => g.Count());

            foreach (var user in evaluatedUsers)
            {
                var used = salonsByUser[user].Where(salonId => salonId != hidden[user]).ToHashSet();
                var candidates = allSalons.Where(salonId => !used.Contains(salonId)).ToList();
                var modelScores = model.Score(user, candidates);

                if (RanksInTopK(candidates, hidden[user], salonId => modelScores.TryGetValue(salonId, out var score) ? score : double.NegativeInfinity, k)) modelHits++;
                if (RanksInTopK(candidates, hidden[user], salonId => popularity.GetValueOrDefault(salonId), k)) popularityHits++;
            }
        }

        var trials = evaluatedUsers.Count * Rounds;
        return new EvaluationResult(evaluatedUsers.Count, k, (double)modelHits / trials, (double)popularityHits / trials);
    }

    /// <summary>Ties are broken by salon id, so the result never depends on the order of the input.</summary>
    private static bool RanksInTopK(List<int> candidates, int hiddenSalonId, Func<int, double> score, int k) =>
        candidates.OrderByDescending(score).ThenBy(salonId => salonId).Take(k).Contains(hiddenSalonId);
}
