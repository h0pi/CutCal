using CutCal.Model.Common;

namespace CutCal.Services.Base;

public interface IBaseReadService<TResponse, in TSearch> where TSearch : BaseSearchObject
{
    Task<PageResult<TResponse>> GetPagedAsync(TSearch search);
    Task<TResponse?> GetByIdAsync(int id);
}

public interface IBaseCRUDService<TResponse, in TSearch, in TInsert, in TUpdate> : IBaseReadService<TResponse, TSearch>
    where TSearch : BaseSearchObject
{
    Task<TResponse> InsertAsync(TInsert request);
    Task<TResponse> UpdateAsync(int id, TUpdate request);
    Task DeleteAsync(int id);
}
