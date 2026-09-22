using CutCal.Model.Constants;
using CutCal.Model.Responses;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IRecommendationService
{
    Task<List<RecommendationResponse>> GetRecommendationsAsync(int customerId, double? lat, double? lng);
}

/// <summary>
/// Hybrid, explainable salon recommender (see recommender-dokumentacija.md).
/// Final score = weighted mean of the components that are available for this user:
///   history similarity (content-based) + proximity + quality (avg rating) + popularity.
/// </summary>
public class RecommendationService : IRecommendationService
{
    private const int MaxResults = 10;
    private const double MaxDistanceKm = 20;

    private const double WeightHistory = 0.40;
    private const double WeightProximity = 0.20;
    private const double WeightQuality = 0.25;
    private const double WeightPopularity = 0.15;

    private const double BookingStrength = 3;
    private const double MaxBookingStrength = 6;
    private const double FavoriteStrength = 2;
    private const double ViewStrength = 0.5;
    private const double MaxViewStrength = 2;
    private const double PositiveReviewBonus = 1;
    private const double NegativeReviewPenalty = -2;
    private const int PositiveRatingMin = 4;
    private const int NegativeRatingMax = 2;

    private const double SimilarityCategory = 0.6;
    private const double SimilarityCity = 0.2;
    private const double SimilarityPrice = 0.2;
    private const double SimilarityNeutralPrice = 0.5;
    private const double SimilarityMentionMin = 0.5;

    private const double SearchShareInHistory = 0.25;
    private const int SearchCountCap = 3;

    private const double ProximityMention = 0.5;
    private const double ExcellentRating = 4.5;
    private const double GoodRating = 4.0;
    private const int PopularMinBookings = 3;
    private const double PopularMinShare = 0.5;

    private readonly CutCalDbContext _context;

    public RecommendationService(CutCalDbContext context)
    {
        _context = context;
    }

    private sealed record Candidate(Salon Salon, decimal? AvgPrice, double? DistanceKm);

    private sealed record Seed(Candidate Candidate, double Strength, int Bookings, bool IsFavorite);

    public async Task<List<RecommendationResponse>> GetRecommendationsAsync(int customerId, double? lat, double? lng)
    {
        var salons = await _context.Salons.AsNoTracking()
            .Include(x => x.SalonCategory)
            .Include(x => x.City)
            .Include(x => x.WorkingHours)
            .Include(x => x.Services)
            .Where(x => x.IsApproved)
            .ToListAsync();

        var myBookings = await CountBySalonAsync(_context.Appointments.Where(x => x.CustomerId == customerId && x.StateName != AppointmentStateNames.Cancelled).Select(x => x.SalonId));
        var allBookings = await CountBySalonAsync(_context.Appointments.Where(x => x.StateName != AppointmentStateNames.Cancelled).Select(x => x.SalonId));
        var myViews = await CountBySalonAsync(_context.SalonViews.Where(x => x.UserId == customerId).Select(x => x.SalonId));
        var favorites = (await _context.Favorites.Where(x => x.UserId == customerId).Select(x => x.SalonId).ToListAsync()).ToHashSet();
        var myRatings = await _context.Reviews.Where(x => x.CustomerId == customerId && !x.IsRemoved)
            .GroupBy(x => x.SalonId)
            .Select(g => new { SalonId = g.Key, Rating = g.Average(r => r.Rating) })
            .ToDictionaryAsync(x => x.SalonId, x => x.Rating);
        var categorySearches = await _context.UserSearchHistories.Where(x => x.UserId == customerId)
            .GroupBy(x => x.SalonCategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

        var candidates = salons.Select(s => new Candidate(
            s,
            s.Services.Where(x => x.IsActive).Select(x => (decimal?)x.Price).Average(),
            lat.HasValue && lng.HasValue ? SalonManagementService.DistanceKm(lat.Value, lng.Value, s.Latitude, s.Longitude) : null)).ToList();

        var seeds = candidates
            .Select(c => new Seed(
                c,
                InteractionStrength(c.Salon.Id, myBookings, myViews, favorites, myRatings),
                myBookings.GetValueOrDefault(c.Salon.Id),
                favorites.Contains(c.Salon.Id)))
            .Where(x => x.Strength > 0)
            .ToList();

        var maxBookings = Math.Max(allBookings.Values.DefaultIfEmpty(0).Max(), 1);
        var maxSearches = categorySearches.Values.DefaultIfEmpty(0).Select(x => Math.Min(x, SearchCountCap)).Max();

        var results = candidates.Select(c =>
        {
            var bestSeed = seeds
                .Where(s => s.Candidate.Salon.Id != c.Salon.Id)
                .OrderByDescending(s => s.Strength * Similarity(c, s.Candidate))
                .FirstOrDefault();

            double? history = seeds.Count > 0
                ? seeds.Sum(s => s.Strength * Similarity(c, s.Candidate)) / seeds.Sum(s => s.Strength)
                : null;

            var searchCount = categorySearches.GetValueOrDefault(c.Salon.SalonCategoryId);
            double? searchInterest = maxSearches > 0 ? (double)Math.Min(searchCount, SearchCountCap) / SearchCountCap : null;
            if (searchInterest.HasValue)
            {
                history = history.HasValue
                    ? (1 - SearchShareInHistory) * history.Value + SearchShareInHistory * searchInterest.Value
                    : searchInterest;
            }

            double? proximity = c.DistanceKm.HasValue ? 1 - Math.Min(c.DistanceKm.Value, MaxDistanceKm) / MaxDistanceKm : null;
            var quality = c.Salon.AvgRating / 5.0;
            var bookingCount = allBookings.GetValueOrDefault(c.Salon.Id);
            var popularity = (double)bookingCount / maxBookings;

            var components = new List<(double Weight, double Value)>
            {
                (WeightQuality, quality),
                (WeightPopularity, popularity)
            };
            if (history.HasValue) components.Add((WeightHistory, history.Value));
            if (proximity.HasValue) components.Add((WeightProximity, proximity.Value));
            var score = components.Sum(x => x.Weight * x.Value) / components.Sum(x => x.Weight);

            var response = c.Salon.Adapt<SalonResponse>();
            response.DistanceKm = c.DistanceKm.HasValue ? Math.Round(c.DistanceKm.Value, 2) : null;

            return new RecommendationResponse
            {
                Salon = response,
                Score = Math.Round(score, 4),
                Reason = BuildReason(c, bestSeed, searchCount, proximity, myBookings.GetValueOrDefault(c.Salon.Id), myViews.GetValueOrDefault(c.Salon.Id), favorites.Contains(c.Salon.Id), bookingCount, popularity)
            };
        });

        return results.OrderByDescending(x => x.Score).Take(MaxResults).ToList();
    }

    private static async Task<Dictionary<int, int>> CountBySalonAsync(IQueryable<int> salonIds)
    {
        return await salonIds.GroupBy(x => x)
            .Select(g => new { SalonId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.SalonId, x => x.Count);
    }

    private static double InteractionStrength(int salonId, Dictionary<int, int> bookings, Dictionary<int, int> views, HashSet<int> favorites, Dictionary<int, double> ratings)
    {
        var strength = Math.Min(bookings.GetValueOrDefault(salonId) * BookingStrength, MaxBookingStrength)
            + (favorites.Contains(salonId) ? FavoriteStrength : 0)
            + Math.Min(views.GetValueOrDefault(salonId) * ViewStrength, MaxViewStrength);

        if (ratings.TryGetValue(salonId, out var rating))
        {
            if (rating >= PositiveRatingMin) strength += PositiveReviewBonus;
            else if (rating <= NegativeRatingMax) strength += NegativeReviewPenalty;
        }
        return strength;
    }

    private static double Similarity(Candidate a, Candidate b)
    {
        var category = a.Salon.SalonCategoryId == b.Salon.SalonCategoryId ? 1.0 : 0.0;
        var city = a.Salon.CityId == b.Salon.CityId ? 1.0 : 0.0;
        return SimilarityCategory * category + SimilarityCity * city + SimilarityPrice * PriceCloseness(a.AvgPrice, b.AvgPrice);
    }

    private static double PriceCloseness(decimal? a, decimal? b)
    {
        if (a is null || b is null || Math.Max(a.Value, b.Value) == 0) return SimilarityNeutralPrice;
        return 1 - Math.Min((double)(Math.Abs(a.Value - b.Value) / Math.Max(a.Value, b.Value)), 1);
    }

    private static string BuildReason(Candidate c, Seed? bestSeed, int searchCount, double? proximity, int myBookings, int myViews, bool isFavorite, int totalBookings, double popularity)
    {
        var parts = new List<string>();

        if (bestSeed is not null && Similarity(c, bestSeed.Candidate) >= SimilarityMentionMin)
        {
            var matches = new List<string>();
            if (c.Salon.SalonCategoryId == bestSeed.Candidate.Salon.SalonCategoryId) matches.Add("ista kategorija");
            if (c.Salon.CityId == bestSeed.Candidate.Salon.CityId) matches.Add("isti grad");
            if (c.AvgPrice.HasValue && PriceCloseness(c.AvgPrice, bestSeed.Candidate.AvgPrice) >= SimilarityMentionMin) matches.Add("sličan raspon cijena");

            var relation = bestSeed.Bookings > 0 ? "koji ste posjetili" : bestSeed.IsFavorite ? "iz vaših favorita" : "koji ste pregledali";
            parts.Add($"sličan salonu {bestSeed.Candidate.Salon.Name} {relation}" + (matches.Count > 0 ? $" ({string.Join(", ", matches)})" : string.Empty));
        }
        else if (searchCount > 0)
        {
            parts.Add($"pretraživali ste kategoriju {c.Salon.SalonCategory.Name} ({searchCount}x)");
        }

        if (myBookings > 0) parts.Add($"već ste ovdje rezervisali {myBookings}x");
        else if (myViews > 0) parts.Add($"pregledali ste ga {myViews}x");
        if (isFavorite) parts.Add("u vašim favoritima");
        if (proximity is >= ProximityMention && c.DistanceKm.HasValue) parts.Add($"{c.DistanceKm.Value:F1} km od vas");

        if (c.Salon.AvgRating >= ExcellentRating) parts.Add($"odlična ocjena {c.Salon.AvgRating:F1}/5");
        else if (c.Salon.AvgRating >= GoodRating) parts.Add($"dobra ocjena {c.Salon.AvgRating:F1}/5");

        if (totalBookings >= PopularMinBookings && popularity >= PopularMinShare) parts.Add($"popularan ({totalBookings} rezervacija)");

        return parts.Count == 0
            ? "Preporučeno na osnovu opće popularnosti salona."
            : $"Preporučeno: {string.Join(", ", parts)}.";
    }
}
