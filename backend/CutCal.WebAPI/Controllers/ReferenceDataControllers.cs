using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

public class SalonCategoriesController : BaseCRUDController<SalonCategoryResponse, SalonCategorySearchObject, SalonCategoryInsertRequest, SalonCategoryUpdateRequest, ISalonCategoryService>
{
    public SalonCategoriesController(ISalonCategoryService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public override Task<ActionResult<SalonCategoryResponse>> Insert([FromBody] SalonCategoryInsertRequest request) => base.Insert(request);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public override Task<ActionResult<SalonCategoryResponse>> Update(int id, [FromBody] SalonCategoryUpdateRequest request) => base.Update(id, request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}

public class CitiesController : BaseCRUDController<CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest, ICityService>
{
    public CitiesController(ICityService service) : base(service)
    {
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public override Task<ActionResult<CityResponse>> Insert([FromBody] CityInsertRequest request) => base.Insert(request);

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public override Task<ActionResult<CityResponse>> Update(int id, [FromBody] CityUpdateRequest request) => base.Update(id, request);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);
}
