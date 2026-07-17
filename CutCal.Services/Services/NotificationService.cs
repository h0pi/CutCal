using CutCal.Model.Common;
using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface INotificationService : IBaseReadService<NotificationResponse, NotificationSearchObject>
{
    Task<NotificationResponse> CreateAsync(int userId, string title, string body, string type);
    Task MarkReadAsync(int id, int userId);
    Task MarkAllReadAsync(int userId);
    Task<PageResult<NotificationResponse>> GetForUserAsync(int userId, NotificationSearchObject search);
}

public class NotificationService : BaseReadService<Notification, NotificationResponse, NotificationSearchObject>, INotificationService
{
    public NotificationService(CutCalDbContext context) : base(context)
    {
    }

    protected override IQueryable<Notification> AddFilter(IQueryable<Notification> query, NotificationSearchObject search)
    {
        if (search.IsRead.HasValue)
        {
            query = query.Where(x => x.IsRead == search.IsRead.Value);
        }
        return query.OrderByDescending(x => x.SentAt);
    }

    public async Task<PageResult<NotificationResponse>> GetForUserAsync(int userId, NotificationSearchObject search)
    {
        var query = AddFilter(Context.Notifications.Where(x => x.UserId == userId), search);

        var totalCount = search.IncludeTotalCount ? await query.CountAsync() : 0;
        var pageSize = Math.Min(search.PageSize ?? CutCal.Model.Common.BaseSearchObject.DefaultPageSize, CutCal.Model.Common.BaseSearchObject.MaxPageSize);
        var page = Math.Max(search.Page ?? 0, 0);
        var items = await query.Skip(page * pageSize).Take(pageSize).ToListAsync();

        return new PageResult<NotificationResponse> { Items = items.Adapt<List<NotificationResponse>>(), TotalCount = totalCount };
    }

    public async Task<NotificationResponse> CreateAsync(int userId, string title, string body, string type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            Type = type,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };
        Context.Notifications.Add(notification);
        await Context.SaveChangesAsync();
        return notification.Adapt<NotificationResponse>();
    }

    public async Task MarkReadAsync(int id, int userId)
    {
        var notification = await Context.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId)
            ?? throw new ClientException("Notification not found.");
        notification.IsRead = true;
        await Context.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(int userId)
    {
        var notifications = await Context.Notifications.Where(x => x.UserId == userId && !x.IsRead).ToListAsync();
        foreach (var n in notifications)
        {
            n.IsRead = true;
        }
        await Context.SaveChangesAsync();
    }
}
