using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Base;
using CutCal.Services.Database;
using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface ISalonService : IBaseCRUDService<SalonResponse, SalonSearchObject, SalonInsertRequest, SalonUpdateRequest>
{
    Task<SalonResponse> ApproveAsync(int id);
    Task<List<SalonGalleryResponse>> GetGalleryAsync(int salonId);
    Task<SalonGalleryResponse> AddGalleryImageAsync(int salonId, SalonGalleryInsertRequest request);
    Task LogCategorySearchAsync(int userId, int categoryId);
    Task LogViewAsync(int userId, int salonId);
}

public class SalonManagementService : BaseCRUDService<Salon, SalonResponse, SalonSearchObject, SalonInsertRequest, SalonUpdateRequest>, ISalonService
{
    private const int ViewDedupeMinutes = 10;
    private const double EarthRadiusKm = 6371.0;

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
        }

        return result;
    }

    protected override IQueryable<Salon> AddInclude(IQueryable<Salon> query)
    {
        return query
            .Include(x => x.SalonCategory)
            .Include(x => x.City)
            .Include(x => x.WorkingHours)
            .Include(x => x.Services);
    }

    protected override IQueryable<Salon> AddSecurityFilter(IQueryable<Salon> query)
    {
        // A SalonManager only ever sees their own salon(s), whether approved yet or not
        // (they need to see a freshly-created, still-pending-approval salon of their own).
        if (_userAccessor.IsInRole(RoleNames.SalonManager))
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
        else if (!_userAccessor.IsInRole(RoleNames.SalonManager))
        {
            // Public/customer browsing only ever sees approved salons by default.
            query = query.Where(x => x.IsApproved);
        }
        if (search.MinRating.HasValue)
        {
            query = query.Where(x => x.AvgRating >= search.MinRating.Value);
        }
        if (search.OpenNow == true)
        {
            var now = search.NowLocal ?? DateTime.UtcNow;
            var day = (int)now.DayOfWeek;
            var time = TimeOnly.FromDateTime(now);
            query = query.Where(x => x.WorkingHours.Any(w => w.DayOfWeek == day && !w.IsClosed && w.OpenTime <= time && w.CloseTime > time));
        }

        var hasLocation = search.Lat.HasValue && search.Lng.HasValue;
        if (hasLocation && search.RadiusKm.HasValue)
        {
            var distance = DistanceExpression(search.Lat!.Value, search.Lng!.Value);
            var withinRadius = Expression.Lambda<Func<Salon, bool>>(
                Expression.LessThanOrEqual(distance.Body, Expression.Constant(search.RadiusKm.Value)), distance.Parameters);
            query = query.Where(withinRadius);
        }

        return ApplySorting(query, search, hasLocation);
    }

    private static IQueryable<Salon> ApplySorting(IQueryable<Salon> query, SalonSearchObject search, bool hasLocation)
    {
        // Without a location "nearest" is meaningless, so it falls back to rating; Id keeps paging stable.
        var sortBy = search.SortBy ?? (hasLocation ? SalonSortBy.Nearest : SalonSortBy.TopRated);
        if (sortBy == SalonSortBy.Nearest && hasLocation)
        {
            return query.OrderBy(DistanceExpression(search.Lat!.Value, search.Lng!.Value)).ThenBy(x => x.Id);
        }
        if (sortBy == SalonSortBy.PriceLow)
        {
            return query.OrderBy(x => x.Services.Where(s => s.IsActive).Min(s => (decimal?)s.Price)).ThenBy(x => x.Id);
        }
        return query.OrderByDescending(x => x.AvgRating).ThenBy(x => x.Id);
    }

    /// <summary>Haversine distance in km, written as an expression so the database can filter and sort by it.</summary>
    private static Expression<Func<Salon, double>> DistanceExpression(double lat, double lng)
    {
        var lat1 = lat * Math.PI / 180;
        var lng1 = lng * Math.PI / 180;
        return x => 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(
            Math.Sin((x.Latitude * Math.PI / 180 - lat1) / 2) * Math.Sin((x.Latitude * Math.PI / 180 - lat1) / 2)
            + Math.Cos(lat1) * Math.Cos(x.Latitude * Math.PI / 180)
              * Math.Sin((x.Longitude * Math.PI / 180 - lng1) / 2) * Math.Sin((x.Longitude * Math.PI / 180 - lng1) / 2)));
    }

    public async Task<SalonResponse> ApproveAsync(int id)
    {
        var salon = await GetEntityByIdAsync(id) ?? throw new ClientException("Salon not found.");
        salon.IsApproved = true;
        await Context.SaveChangesAsync();
        return salon.Adapt<SalonResponse>();
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

    public async Task LogViewAsync(int userId, int salonId)
    {
        if (!await Context.Salons.AnyAsync(x => x.Id == salonId && x.IsApproved))
        {
            throw new ClientException("Salon not found.");
        }

        // Re-opening the same salon within minutes is one interest signal, not several.
        var recentCutoff = DateTime.UtcNow.AddMinutes(-ViewDedupeMinutes);
        if (await Context.SalonViews.AnyAsync(x => x.UserId == userId && x.SalonId == salonId && x.ViewedAt >= recentCutoff))
        {
            return;
        }

        Context.SalonViews.Add(new SalonView { UserId = userId, SalonId = salonId, ViewedAt = DateTime.UtcNow });
        await Context.SaveChangesAsync();
    }

    public static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180.0;
}
