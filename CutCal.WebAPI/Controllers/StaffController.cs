using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

public class StaffController : BaseCRUDController<StaffResponse, StaffSearchObject, StaffInsertRequest, StaffUpdateRequest, IStaffService>
{
    public StaffController(IStaffService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<ActionResult<StaffResponse>> Insert([FromBody] StaffInsertRequest request) => base.Insert(request);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<ActionResult<StaffResponse>> Update(int id, [FromBody] StaffUpdateRequest request) => base.Update(id, request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SalonManager")]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}
