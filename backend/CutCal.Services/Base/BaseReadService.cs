using CutCal.Model.Common;
using CutCal.Services.Database;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Base;

public abstract class BaseReadService<TEntity, TResponse, TSearch> : IBaseReadService<TResponse, TSearch>
    where TEntity : class
    where TSearch : BaseSearchObject
{
    protected readonly CutCalDbContext Context;

    protected BaseReadService(CutCalDbContext context)
    {
        Context = context;
    }

    public virtual async Task<PageResult<TResponse>> GetPagedAsync(TSearch search)
    {
        var query = Context.Set<TEntity>().AsQueryable();
        query = AddInclude(query);
        query = AddSecurityFilter(query);
        query = AddFilter(query, search);

        var totalCount = 0;
        if (search.IncludeTotalCount)
        {
            totalCount = await query.CountAsync();
        }

        var pageSize = Math.Min(search.PageSize ?? BaseSearchObject.DefaultPageSize, BaseSearchObject.MaxPageSize);
        var page = Math.Max(search.Page ?? 0, 0);
        query = query.Skip(page * pageSize).Take(pageSize);

        var list = await query.ToListAsync();
        return new PageResult<TResponse>
        {
            Items = list.Adapt<List<TResponse>>(),
            TotalCount = totalCount
        };
    }

    public virtual async Task<TResponse?> GetByIdAsync(int id)
    {
        var query = AddSecurityFilter(AddInclude(Context.Set<TEntity>().AsQueryable()));
        var entity = await ApplyIdFilter(query, id).FirstOrDefaultAsync();
        return entity is null ? default : entity.Adapt<TResponse>();
    }

    protected virtual async Task<TEntity?> GetEntityByIdAsync(int id)
    {
        var query = AddSecurityFilter(AddInclude(Context.Set<TEntity>().AsQueryable()));
        return await ApplyIdFilter(query, id).FirstOrDefaultAsync();
    }

    protected virtual IQueryable<TEntity> ApplyIdFilter(IQueryable<TEntity> query, int id)
    {
        return query.Where(x => EF.Property<int>(x, "Id") == id);
    }

    protected virtual IQueryable<TEntity> AddInclude(IQueryable<TEntity> query) => query;

    protected virtual IQueryable<TEntity> AddFilter(IQueryable<TEntity> query, TSearch search) => query;

    /// <summary>
    /// Role-based visibility scoping (e.g. a SalonManager only ever seeing their own salon's data).
    /// Runs before AddFilter on every read path, including GetById, so scoping can't be bypassed
    /// by fetching a single record directly by id.
    /// </summary>
    protected virtual IQueryable<TEntity> AddSecurityFilter(IQueryable<TEntity> query) => query;
}
