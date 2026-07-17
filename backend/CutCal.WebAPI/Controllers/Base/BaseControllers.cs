using CutCal.Model.Common;
using CutCal.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers.Base;

[ApiController]
[Authorize]
[Route("[controller]")]
public abstract class BaseReadController<TResponse, TSearch, TService> : ControllerBase
    where TSearch : BaseSearchObject, new()
    where TService : IBaseReadService<TResponse, TSearch>
{
    protected readonly TService Service;

    protected BaseReadController(TService service)
    {
        Service = service;
    }

    [HttpGet]
    public virtual async Task<ActionResult<PageResult<TResponse>>> Get([FromQuery] TSearch search)
    {
        return Ok(await Service.GetPagedAsync(search));
    }

    [HttpGet("{id:int}")]
    public virtual async Task<ActionResult<TResponse>> GetById(int id)
    {
        var result = await Service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}

public abstract class BaseCRUDController<TResponse, TSearch, TInsert, TUpdate, TService>
    : BaseReadController<TResponse, TSearch, TService>
    where TSearch : BaseSearchObject, new()
    where TService : IBaseCRUDService<TResponse, TSearch, TInsert, TUpdate>
{
    protected BaseCRUDController(TService service) : base(service)
    {
    }

    [HttpPost]
    public virtual async Task<ActionResult<TResponse>> Insert([FromBody] TInsert request)
    {
        var result = await Service.InsertAsync(request);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public virtual async Task<ActionResult<TResponse>> Update(int id, [FromBody] TUpdate request)
    {
        var result = await Service.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await Service.DeleteAsync(id);
        return NoContent();
    }
}
