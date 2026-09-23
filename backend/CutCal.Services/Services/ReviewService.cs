using CutCal.Model.Constants;
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

public interface IReviewService : IBaseReadService<ReviewResponse, ReviewSearchObject>
{
    Task<ReviewResponse> InsertAsync(ReviewInsertRequest request, int customerId);
    Task<ReviewResponse> ReplyAsync(int id, ReviewReplyRequest request, int salonManagerId);
    Task RemoveAsync(int id, int removedById);
}

public class ReviewService : BaseReadService<Review, ReviewResponse, ReviewSearchObject>, IReviewService
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public ReviewService(CutCalDbContext context, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _userAccessor = userAccessor;
    }

    protected override IQueryable<Review> AddInclude(IQueryable<Review> query)
    {
        return query.Include(x => x.Customer);
    }

    protected override IQueryable<Review> AddSecurityFilter(IQueryable<Review> query)
    {
        // Reviews are public browsing content (any customer views a salon's reviews), so only
        // a SalonManager gets scoped — to reviews of salons they actually manage.
        if (_userAccessor.IsInRole(RoleNames.SalonManager))
        {
            query = query.Where(x => x.Salon.OwnerId == _userAccessor.UserId);
        }
        return query;
    }

    protected override IQueryable<Review> AddFilter(IQueryable<Review> query, ReviewSearchObject search)
    {
        if (search.SalonId.HasValue)
        {
            query = query.Where(x => x.SalonId == search.SalonId.Value);
        }
        if (search.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == search.CustomerId.Value);
        }
        if (search.IsRemoved.HasValue)
        {
            query = query.Where(x => x.IsRemoved == search.IsRemoved.Value);
        }
        else
        {
            query = query.Where(x => !x.IsRemoved);
        }
        return query.OrderByDescending(x => x.CreatedAt);
    }

    public async Task<ReviewResponse> InsertAsync(ReviewInsertRequest request, int customerId)
    {
        if (request.Rating is < 1 or > 5)
        {
            throw new ClientException("Rating must be between 1 and 5.");
        }

        var appointment = await Context.Appointments.FirstOrDefaultAsync(x => x.Id == request.AppointmentId)
            ?? throw new ClientException("Appointment not found.");
        if (appointment.CustomerId != customerId)
        {
            throw new ClientException("You can only review your own appointments.");
        }
        if (appointment.StateName != AppointmentStateNames.Completed)
        {
            throw new ClientException("You can only review completed appointments.");
        }

        var exists = await Context.Reviews.AnyAsync(x => x.AppointmentId == request.AppointmentId);
        if (exists)
        {
            throw new ClientException("A review already exists for this appointment.");
        }

        var review = new Review
        {
            AppointmentId = request.AppointmentId,
            CustomerId = customerId,
            SalonId = appointment.SalonId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsRemoved = false,
            CreatedAt = DateTime.UtcNow
        };
        Context.Reviews.Add(review);
        await UpdateSalonRatingAsync(appointment.SalonId, newRating: request.Rating, excludedReviewId: null);
        await Context.SaveChangesAsync();

        return review.Adapt<ReviewResponse>();
    }

    public async Task<ReviewResponse> ReplyAsync(int id, ReviewReplyRequest request, int salonManagerId)
    {
        var review = await Context.Reviews.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ClientException("Review not found.");
        await OwnershipGuard.EnsureManagesSalonAsync(Context, review.SalonId, _userAccessor);
        review.SalonReply = request.Reply;
        await Context.SaveChangesAsync();
        return review.Adapt<ReviewResponse>();
    }

    public async Task RemoveAsync(int id, int removedById)
    {
        var review = await Context.Reviews.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ClientException("Review not found.");
        await OwnershipGuard.EnsureManagesSalonAsync(Context, review.SalonId, _userAccessor);
        review.IsRemoved = true;
        review.RemovedById = removedById;
        await UpdateSalonRatingAsync(review.SalonId, newRating: null, excludedReviewId: review.Id);
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Recalculates the salon's average rating on the shared DbContext, so the review change and the new
    /// average are stored by one SaveChanges. The review being added or removed is not in the database yet,
    /// so it is added or left out explicitly.
    /// </summary>
    private async Task UpdateSalonRatingAsync(int salonId, int? newRating, int? excludedReviewId)
    {
        var salon = await Context.Salons.FirstOrDefaultAsync(x => x.Id == salonId) ?? throw new ClientException("Salon not found.");
        var ratings = await Context.Reviews
            .Where(r => r.SalonId == salonId && !r.IsRemoved && r.Id != excludedReviewId)
            .Select(r => r.Rating)
            .ToListAsync();
        if (newRating.HasValue)
        {
            ratings.Add(newRating.Value);
        }

        salon.AvgRating = ratings.Count > 0 ? Math.Round(ratings.Average(), 2) : 0;
    }
}
