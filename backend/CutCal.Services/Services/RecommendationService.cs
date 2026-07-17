using CutCal.Model.Responses;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IRecommendationService
{
    Task<List<RecommendationResponse>> GetRecommendationsAsync(int customerId, double? lat, double? lng);
}

public class RecommendationService : IRecommendationService
{
    private readonly CutCalDbContext _context;

    public RecommendationService(CutCalDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecommendationResponse>> GetRecommendationsAsync(int customerId, double? lat, double? lng)
    {
        var salons = await _context.Salons
            .Include(x => x.SalonCategory)
            .Include(x => x.City)
            .Where(x => x.IsApproved)
            .ToListAsync();

        var customerAppointments = await _context.Appointments
            .Where(x => x.CustomerId == customerId)
            .Select(x => x.Salon.SalonCategoryId)
            .ToListAsync();

        var searchHistoryCategories = await _context.UserSearchHistories
            .Where(x => x.UserId == customerId)
            .Select(x => x.SalonCategoryId)
            .ToListAsync();

        var categoryCounts = customerAppointments.Concat(searchHistoryCategories)
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
        var totalSignals = Math.Max(categoryCounts.Values.Sum(), 1);

        var favorites = await _context.Favorites.Where(x => x.UserId == customerId).Select(x => x.SalonId).ToListAsync();

        var results = new List<RecommendationResponse>();
        foreach (var salon in salons)
        {
            var categoryMatchesCount = categoryCounts.GetValueOrDefault(salon.SalonCategoryId, 0);
            var categoryMatch = (double)categoryMatchesCount / totalSignals * 0.40;
            var ratingScore = salon.AvgRating / 5.0 * 0.30;

            double distanceKm = 0;
            double distanceScore = 0;
            if (lat.HasValue && lng.HasValue)
            {
                distanceKm = SalonManagementService.DistanceKm(lat.Value, lng.Value, salon.Latitude, salon.Longitude);
                distanceScore = (1 - Math.Min(distanceKm, 20) / 20) * 0.20;
            }

            var isFavorite = favorites.Contains(salon.Id);
            var favoriteBonus = isFavorite ? 0.10 : 0.0;

            var totalScore = categoryMatch + ratingScore + distanceScore + favoriteBonus;

            var reasonParts = new List<string>();
            if (categoryMatchesCount > 0)
            {
                reasonParts.Add($"ste {categoryMatchesCount}x koristili {salon.SalonCategory.Name} kategoriju");
            }
            if (lat.HasValue && lng.HasValue)
            {
                reasonParts.Add($"salon je udaljen {distanceKm:F1} km od vaše lokacije");
            }
            if (isFavorite)
            {
                reasonParts.Add("nalazi se u vašim omiljenim salonima");
            }
            if (salon.AvgRating >= 4.5)
            {
                reasonParts.Add($"ima visoku ocjenu ({salon.AvgRating:F1}/5)");
            }

            var reason = reasonParts.Count > 0
                ? $"Preporučujemo jer {string.Join(" i ", reasonParts)}."
                : "Preporučujemo na osnovu opšte popularnosti salona.";

            results.Add(new RecommendationResponse
            {
                Salon = salon.Adapt<SalonResponse>(),
                Score = Math.Round(totalScore, 4),
                Reason = reason
            });
        }

        return results.OrderByDescending(x => x.Score).ToList();
    }
}
