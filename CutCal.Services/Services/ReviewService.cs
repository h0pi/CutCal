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
    Task RemoveAsync(int id, int adminId);
}

public class ReviewService : BaseReadService<Review, ReviewResponse, ReviewSearchObject>, IReviewService
{
    private readonly ISalonService _salonService;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public ReviewService(CutCalDbContext context, ISalonService salonService, IAuthenticatedUserAccessor userAccessor) : base(context)
    {
        _salonService = salonService;
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
        if (_userAccessor.IsInRole("SalonManager"))
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
        if (appointment.StateName != "Completed")
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
        await Context.SaveChangesAsync();

        await _salonService.UpdateAvgRatingAsync(appointment.SalonId);

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

    public async Task RemoveAsync(int id, int adminId)
    {
        var review = await Context.Reviews.FirstOrDefaultAsync(x => x.Id == id) ?? throw new ClientException("Review not found.");
        review.IsRemoved = true;
        review.RemovedById = adminId;
        await Context.SaveChangesAsync();
        await _salonService.UpdateAvgRatingAsync(review.SalonId);
    }
}
