using CutCal.Model.Common;
using CutCal.Model.Responses;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IFavoriteService
{
    Task<List<FavoriteResponse>> GetForUserAsync(int userId);
    Task<FavoriteResponse> AddAsync(int userId, int salonId);
    Task RemoveAsync(int userId, int salonId);
}

public class FavoriteService : IFavoriteService
{
    private readonly CutCalDbContext _context;

    public FavoriteService(CutCalDbContext context)
    {
        _context = context;
    }

    public async Task<List<FavoriteResponse>> GetForUserAsync(int userId)
    {
        // Not paginated (the client just renders a flat "your favorites" list), but still capped
        // server-side so a single user can never make this endpoint return an unbounded payload.
        var favorites = await _context.Favorites
            .Include(x => x.Salon)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SavedAt)
            .Take(BaseSearchObject.MaxPageSize)
            .ToListAsync();
        return favorites.Adapt<List<FavoriteResponse>>();
    }

    public async Task<FavoriteResponse> AddAsync(int userId, int salonId)
    {
        var existing = await _context.Favorites.FirstOrDefaultAsync(x => x.UserId == userId && x.SalonId == salonId);
        if (existing is not null)
        {
            return existing.Adapt<FavoriteResponse>();
        }

        var favorite = new Favorite { UserId = userId, SalonId = salonId, SavedAt = DateTime.UtcNow };
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return favorite.Adapt<FavoriteResponse>();
    }

    public async Task RemoveAsync(int userId, int salonId)
    {
        var favorite = await _context.Favorites.FirstOrDefaultAsync(x => x.UserId == userId && x.SalonId == salonId);
        if (favorite is not null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }
}
