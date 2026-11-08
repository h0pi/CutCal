using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface ISalonServiceService : IBaseCRUDService<SalonServiceResponse, SalonServiceSearchObject, SalonServiceInsertRequest, SalonServiceUpdateRequest>
{
}

public class SalonServiceService : BaseCRUDService<SalonService, SalonServiceResponse, SalonServiceSearchObject, SalonServiceInsertRequest, SalonServiceUpdateRequest>, ISalonServiceService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public SalonServiceService(CutCalDbContext context, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _userAccessor = userAccessor;
    }

    protected override IQueryable<SalonService> AddSecurityFilter(IQueryable<SalonService> query)
    {
        // Services stay public for browsing (a Customer picks a service while booking), only
        // a SalonManager is scoped down to services at salons they manage.
        if (_userAccessor.IsInRole(RoleNames.SalonManager))
        {
            query = query.Where(x => x.Salon.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<SalonService> AddFilter(IQueryable<SalonService> query, SalonServiceSearchObject search)
    {
        if (search.SalonId.HasValue)
        {
            query = query.Where(x => x.SalonId == search.SalonId.Value);
        }
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => x.Name.Contains(search.Name));
        }
        if (search.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == search.IsActive.Value);
        }
        return query;
    }

    protected override Task BeforeInsert(SalonService entity, SalonServiceInsertRequest request)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.SalonId, _userAccessor);

    protected override Task BeforeUpdate(SalonService entity, SalonServiceUpdateRequest request)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.SalonId, _userAccessor);

    protected override async Task BeforeDelete(SalonService entity)
    {
        await OwnershipGuard.EnsureManagesSalonAsync(Context, entity.SalonId, _userAccessor);

        var appointments = await Context.Appointments.CountAsync(x => x.ServiceId == entity.Id);
        if (appointments > 0)
        {
            throw new ClientException($"The service cannot be deleted because it has {appointments} appointment(s). Deactivate it instead.");
        }

        // Which staff perform the service is only a link, so it goes with the service.
        Context.StaffServices.RemoveRange(Context.StaffServices.Where(x => x.ServiceId == entity.Id));
    }
}
