using CutCal.Model.Constants;
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
        if (user.IsInRole(RoleNames.Admin))
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

    /// <summary>
    /// Throws unless the current user may act on the appointment: an Admin on any, a salon manager on the appointments of their
    /// own salon, a staff member on those assigned to them and, when <paramref name="allowCustomer"/> is set, the customer on their own.
    /// </summary>
    public static async Task EnsureCanActOnAppointmentAsync(CutCalDbContext context, Appointment appointment, IAuthenticatedUserAccessor user, bool allowCustomer)
    {
        if (user.IsInRole(RoleNames.Admin))
        {
            return;
        }

        var allowed = user.IsInRole(RoleNames.SalonManager) ? await context.Salons.AnyAsync(x => x.Id == appointment.SalonId && x.OwnerId == user.UserId)
            : user.IsInRole(RoleNames.Staff) ? await context.Staff.AnyAsync(x => x.Id == appointment.StaffId && x.UserId == user.UserId)
            : allowCustomer && appointment.CustomerId == user.UserId;
        if (!allowed)
        {
            throw new ClientException("You are not allowed to change this appointment.");
        }
    }

    /// <summary>
    /// Throws unless the current user is an Admin or the account identified by <paramref name="targetUserId"/> itself.
    /// Used so a customer/manager/staff account can view and edit only their own profile.
    /// </summary>
    public static void EnsureIsSelfOrAdmin(int targetUserId, IAuthenticatedUserAccessor user)
    {
        if (user.IsInRole(RoleNames.Admin) || user.UserId == targetUserId)
        {
            return;
        }
        throw new ClientException("You can only manage your own account.");
    }
}
