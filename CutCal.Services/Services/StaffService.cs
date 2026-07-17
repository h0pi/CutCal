using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface IStaffService : IBaseCRUDService<StaffResponse, StaffSearchObject, StaffInsertRequest, StaffUpdateRequest>
{
}

public class StaffManagementService : BaseCRUDService<Staff, StaffResponse, StaffSearchObject, StaffInsertRequest, StaffUpdateRequest>, IStaffService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public StaffManagementService(CutCalDbContext context, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _userAccessor = userAccessor;
    }

    protected override Task BeforeInsert(Staff entity, StaffInsertRequest request)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.SalonId, _userAccessor);

    protected override Task BeforeUpdate(Staff entity, StaffUpdateRequest request)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.SalonId, _userAccessor);

    protected override IQueryable<Staff> AddInclude(IQueryable<Staff> query)
    {
        return query.Include(x => x.User).Include(x => x.StaffServices);
    }

    protected override IQueryable<Staff> AddSecurityFilter(IQueryable<Staff> query)
    {
        // Staff listings stay public for browsing (a Customer picks staff while booking), only
        // a SalonManager is scoped down to staff at salons they manage.
        if (_userAccessor.IsInRole("SalonManager"))
        {
            query = query.Where(x => x.Salon.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<Staff> AddFilter(IQueryable<Staff> query, StaffSearchObject search)
    {
        if (search.SalonId.HasValue)
        {
            query = query.Where(x => x.SalonId == search.SalonId.Value);
        }
        if (search.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == search.IsActive.Value);
        }
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => (x.User.FirstName + " " + x.User.LastName).Contains(search.Name));
        }
        return query;
    }

    protected override Staff MapInsertToEntity(StaffInsertRequest request)
    {
        var staff = new Staff
        {
            SalonId = request.SalonId,
            UserId = request.UserId,
            Role = request.Role,
            Bio = request.Bio,
            ProfileImageUrl = request.ProfileImageUrl,
            IsActive = true,
            StaffServices = request.ServiceIds.Select(id => new StaffService { ServiceId = id }).ToList()
        };
        return staff;
    }

    protected override void MapUpdateToEntity(Staff entity, StaffUpdateRequest request)
    {
        entity.Role = request.Role;
        entity.Bio = request.Bio;
        entity.ProfileImageUrl = request.ProfileImageUrl;
        entity.IsActive = request.IsActive;

        Context.StaffServices.RemoveRange(entity.StaffServices);
        entity.StaffServices = request.ServiceIds.Select(id => new StaffService { StaffId = entity.Id, ServiceId = id }).ToList();
    }
}
