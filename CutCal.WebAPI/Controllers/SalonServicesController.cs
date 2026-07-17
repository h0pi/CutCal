using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

public class SalonServicesController : BaseCRUDController<SalonServiceResponse, SalonServiceSearchObject, SalonServiceInsertRequest, SalonServiceUpdateRequest, ISalonServiceService>
{
    public SalonServicesController(ISalonServiceService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<ActionResult<SalonServiceResponse>> Insert([FromBody] SalonServiceInsertRequest request) => base.Insert(request);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<ActionResult<SalonServiceResponse>> Update(int id, [FromBody] SalonServiceUpdateRequest request) => base.Update(id, request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}
