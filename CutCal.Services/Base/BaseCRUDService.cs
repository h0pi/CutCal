using CutCal.Model.Common;
using CutCal.Model.Exceptions;
using CutCal.Services.Database;
using Mapster;

namespace CutCal.Services.Base;

public abstract class BaseCRUDService<TEntity, TResponse, TSearch, TInsert, TUpdate>
    : BaseReadService<TEntity, TResponse, TSearch>, IBaseCRUDService<TResponse, TSearch, TInsert, TUpdate>
    where TEntity : class
    where TSearch : BaseSearchObject
{
    protected BaseCRUDService(CutCalDbContext context) : base(context)
    {
    }

    public virtual async Task<TResponse> InsertAsync(TInsert request)
    {
        var entity = MapInsertToEntity(request);
        await BeforeInsert(entity, request);
        Context.Set<TEntity>().Add(entity);
        await Context.SaveChangesAsync();
        await AfterInsert(entity, request);
        return entity.Adapt<TResponse>();
    }

    public virtual async Task<TResponse> UpdateAsync(int id, TUpdate request)
    {
        var entity = await GetEntityByIdAsync(id) ?? throw new ClientException("Entity not found.");
        MapUpdateToEntity(entity, request);
        await BeforeUpdate(entity, request);
        await Context.SaveChangesAsync();
        return entity.Adapt<TResponse>();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetEntityByIdAsync(id) ?? throw new ClientException("Entity not found.");
        await BeforeDelete(entity);
        Context.Set<TEntity>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    protected virtual TEntity MapInsertToEntity(TInsert request) => request.Adapt<TEntity>()!;
    protected virtual void MapUpdateToEntity(TEntity entity, TUpdate request) => request.Adapt(entity);
    protected virtual Task BeforeInsert(TEntity entity, TInsert request) => Task.CompletedTask;
    protected virtual Task AfterInsert(TEntity entity, TInsert request) => Task.CompletedTask;
    protected virtual Task BeforeUpdate(TEntity entity, TUpdate request) => Task.CompletedTask;
    protected virtual Task BeforeDelete(TEntity entity) => Task.CompletedTask;
}
