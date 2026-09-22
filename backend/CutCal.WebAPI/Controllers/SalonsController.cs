using CutCal.Model.Constants;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Auth;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

public class SalonsController : BaseCRUDController<SalonResponse, SalonSearchObject, SalonInsertRequest, SalonUpdateRequest, ISalonService>
{
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public SalonsController(ISalonService service, IAuthenticatedUserAccessor userAccessor) : base(service)
    {
        _userAccessor = userAccessor;
    }

    public override async Task<ActionResult<CutCal.Model.Common.PageResult<SalonResponse>>> Get([FromQuery] SalonSearchObject search)
    {
        if (search.CategoryId.HasValue && _userAccessor.UserId > 0)
        {
            await Service.LogCategorySearchAsync(_userAccessor.UserId, search.CategoryId.Value);
        }
        return await base.Get(search);
    }

    [HttpPost("{id:int}/Approve")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<SalonResponse>> Approve(int id)
    {
        return Ok(await Service.ApproveAsync(id));
    }

    [HttpPost("{id:int}/View")]
    [Authorize(Roles = RoleNames.Customer)]
    public async Task<IActionResult> LogView(int id)
    {
        await Service.LogViewAsync(_userAccessor.UserId, id);
        return NoContent();
    }

    [HttpGet("{id:int}/Gallery")]
    public async Task<ActionResult<List<SalonGalleryResponse>>> GetGallery(int id)
    {
        return Ok(await Service.GetGalleryAsync(id));
    }

    [HttpPost("{id:int}/Gallery")]
    [Authorize(Roles = RoleNames.AdminOrManager)]
    public async Task<ActionResult<SalonGalleryResponse>> AddGalleryImage(int id, [FromBody] SalonGalleryInsertRequest request)
    {
        return Ok(await Service.AddGalleryImageAsync(id, request));
    }

    [HttpDelete("{id:int}/Gallery/{imageId:int}")]
    [Authorize(Roles = RoleNames.AdminOrManager)]
    public async Task<IActionResult> RemoveGalleryImage(int id, int imageId)
    {
        await Service.RemoveGalleryImageAsync(id, imageId);
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminOrManager)]
    public override Task<ActionResult<SalonResponse>> Insert([FromBody] SalonInsertRequest request) => base.Insert(request);

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrManager)]
    public override Task<ActionResult<SalonResponse>> Update(int id, [FromBody] SalonUpdateRequest request) => base.Update(id, request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin)]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}
