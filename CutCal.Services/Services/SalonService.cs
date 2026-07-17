using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface ISalonService : IBaseCRUDService<SalonResponse, SalonSearchObject, SalonInsertRequest, SalonUpdateRequest>
{
    Task<SalonResponse> ApproveAsync(int id);
    Task UpdateAvgRatingAsync(int salonId);
    Task<List<SalonGalleryResponse>> GetGalleryAsync(int salonId);
    Task<SalonGalleryResponse> AddGalleryImageAsync(int salonId, SalonGalleryInsertRequest request);
    Task LogCategorySearchAsync(int userId, int categoryId);
}

public class SalonManagementService : BaseCRUDService<Salon, SalonResponse, SalonSearchObject, SalonInsertRequest, SalonUpdateRequest>, ISalonService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public SalonManagementService(CutCalDbContext context, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _userAccessor = userAccessor;
    }

    public override async Task<CutCal.Model.Common.PageResult<SalonResponse>> GetPagedAsync(SalonSearchObject search)
    {
        var result = await base.GetPagedAsync(search);

        if (search.Lat.HasValue && search.Lng.HasValue)
        {
            foreach (var salon in result.Items)
            {
                salon.DistanceKm = Math.Round(DistanceKm(search.Lat.Value, search.Lng.Value, salon.Latitude, salon.Longitude), 2);
            }

            if (search.RadiusKm.HasValue)
            {
                result.Items = result.Items.Where(x => x.DistanceKm <= search.RadiusKm.Value).ToList();
            }
        }

        return result;
    }

    protected override IQueryable<Salon> AddInclude(IQueryable<Salon> query)
    {
        return query
            .Include(x => x.SalonCategory)
            .Include(x => x.City)
            .Include(x => x.WorkingHours);
    }

    protected override IQueryable<Salon> AddSecurityFilter(IQueryable<Salon> query)
    {
        // A SalonManager only ever sees their own salon(s), whether approved yet or not
        // (they need to see a freshly-created, still-pending-approval salon of their own).
        if (_userAccessor.IsInRole("SalonManager"))
        {
            query = query.Where(x => x.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<Salon> AddFilter(IQueryable<Salon> query, SalonSearchObject search)
    {
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => x.Name.Contains(search.Name));
        }
        if (search.CategoryId.HasValue)
        {
            query = query.Where(x => x.SalonCategoryId == search.CategoryId.Value);
        }
        if (search.CityId.HasValue)
        {
            query = query.Where(x => x.CityId == search.CityId.Value);
        }
        if (search.IsApproved.HasValue)
        {
            query = query.Where(x => x.IsApproved == search.IsApproved.Value);
        }
        else if (!_userAccessor.IsInRole("SalonManager"))
        {
            // Public/customer browsing only ever sees approved salons by default.
            query = query.Where(x => x.IsApproved);
        }
        return query;
    }

    public async Task<SalonResponse> ApproveAsync(int id)
    {
        var salon = await GetEntityByIdAsync(id) ?? throw new ClientException("Salon not found.");
        salon.IsApproved = true;
        await Context.SaveChangesAsync();
        return salon.Adapt<SalonResponse>();
    }

    public async Task UpdateAvgRatingAsync(int salonId)
    {
        var salon = await Context.Salons.FindAsync(salonId);
        if (salon is null)
        {
            return;
        }

        var ratings = await Context.Reviews
            .Where(r => r.SalonId == salonId && !r.IsRemoved)
            .Select(r => r.Rating)
            .ToListAsync();

        salon.AvgRating = ratings.Count > 0 ? Math.Round(ratings.Average(), 2) : 0;
        await Context.SaveChangesAsync();
    }

    public async Task<List<SalonGalleryResponse>> GetGalleryAsync(int salonId)
    {
        var images = await Context.SalonGalleries.Where(x => x.SalonId == salonId).ToListAsync();
        return images.Adapt<List<SalonGalleryResponse>>();
    }

    public async Task<SalonGalleryResponse> AddGalleryImageAsync(int salonId, SalonGalleryInsertRequest request)
    {
        await OwnershipGuard.EnsureManagesSalonAsync(Context, salonId, _userAccessor);
        var salon = await Context.Salons.FindAsync(salonId) ?? throw new ClientException("Salon not found.");
        var image = new SalonGallery
        {
            SalonId = salon.Id,
            ImageUrl = request.ImageUrl,
            Caption = request.Caption,
            UploadedAt = DateTime.UtcNow
        };
        Context.SalonGalleries.Add(image);
        await Context.SaveChangesAsync();
        return image.Adapt<SalonGalleryResponse>();
    }

    protected override Salon MapInsertToEntity(SalonInsertRequest request)
    {
        var salon = request.Adapt<Salon>();
        salon.OwnerId = _userAccessor.UserId;
        salon.CreatedAt = DateTime.UtcNow;
        salon.IsApproved = false;
        salon.WorkingHours = request.WorkingHours.Select(wh => new SalonWorkingHours
        {
            DayOfWeek = wh.DayOfWeek,
            OpenTime = wh.OpenTime,
            CloseTime = wh.CloseTime,
            IsClosed = wh.IsClosed
        }).ToList();
        return salon;
    }

    protected override Task BeforeUpdate(Salon entity, SalonUpdateRequest request)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.Id, _userAccessor);

    protected override Task BeforeDelete(Salon entity)
        => OwnershipGuard.EnsureManagesSalonAsync(Context, entity.Id, _userAccessor);

    protected override void MapUpdateToEntity(Salon entity, SalonUpdateRequest request)
    {
        entity.Name = request.Name;
        entity.SalonCategoryId = request.SalonCategoryId;
        entity.Description = request.Description;
        entity.Address = request.Address;
        entity.CityId = request.CityId;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.ProfileImageUrl = request.ProfileImageUrl;
        entity.AutoConfirm = request.AutoConfirm;

        if (request.WorkingHours.Count > 0)
        {
            Context.SalonWorkingHours.RemoveRange(entity.WorkingHours);
            entity.WorkingHours = request.WorkingHours.Select(wh => new SalonWorkingHours
            {
                SalonId = entity.Id,
                DayOfWeek = wh.DayOfWeek,
                OpenTime = wh.OpenTime,
                CloseTime = wh.CloseTime,
                IsClosed = wh.IsClosed
            }).ToList();
        }
    }

    public async Task LogCategorySearchAsync(int userId, int categoryId)
    {
        Context.UserSearchHistories.Add(new UserSearchHistory
        {
            UserId = userId,
            SalonCategoryId = categoryId,
            SearchedAt = DateTime.UtcNow
        });
        await Context.SaveChangesAsync();
    }

    public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var earthRadiusKm = 6371.0;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
}
