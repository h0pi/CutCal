using CutCal.Model.Exceptions;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Auth;

public static class OwnershipGuard
{
    /// <summary>
    /// Throws unless the current user is an Admin or the owner of the given salon.
    /// Used so a SalonManager can only manage salons/services/staff/gallery for salons they own.
    /// </summary>
    public static async Task EnsureManagesSalonAsync(CutCalDbContext context, int salonId, IAuthenticatedUserAccessor user)
    {
        if (user.IsInRole("Admin"))
        {
            return;
        }

        var ownerId = await context.Salons.Where(s => s.Id == salonId).Select(s => (int?)s.OwnerId).FirstOrDefaultAsync();
        if (ownerId is null)
        {
            throw new ClientException("Salon not found.");
        }
        if (ownerId != user.UserId)
        {
            throw new ClientException("You do not manage this salon.");
        }
    }
}
