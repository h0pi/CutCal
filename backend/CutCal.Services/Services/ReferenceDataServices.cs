using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Model.SearchObjects;
using CutCal.Services.Base;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Services;

public interface ISalonCategoryService : IBaseCRUDService<SalonCategoryResponse, SalonCategorySearchObject, SalonCategoryInsertRequest, SalonCategoryUpdateRequest>
{
}

public class SalonCategoryService : BaseCRUDService<SalonCategory, SalonCategoryResponse, SalonCategorySearchObject, SalonCategoryInsertRequest, SalonCategoryUpdateRequest>, ISalonCategoryService
{
    public SalonCategoryService(CutCalDbContext context) : base(context)
    {
    }

    protected override IQueryable<SalonCategory> AddFilter(IQueryable<SalonCategory> query, SalonCategorySearchObject search)
    {
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => x.Name.Contains(search.Name));
        }
        return query;
    }
}

public interface ICountryService : IBaseCRUDService<CountryResponse, CountrySearchObject, CountryInsertRequest, CountryUpdateRequest>
{
}

public class CountryService : BaseCRUDService<Country, CountryResponse, CountrySearchObject, CountryInsertRequest, CountryUpdateRequest>, ICountryService
{
    public CountryService(CutCalDbContext context) : base(context)
    {
    }

    protected override IQueryable<Country> AddFilter(IQueryable<Country> query, CountrySearchObject search)
    {
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => x.Name.Contains(search.Name));
        }
        return query;
    }
}

public interface ICityService : IBaseCRUDService<CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest>
{
}

public class CityService : BaseCRUDService<City, CityResponse, CitySearchObject, CityInsertRequest, CityUpdateRequest>, ICityService
{
    public CityService(CutCalDbContext context) : base(context)
    {
    }

    protected override IQueryable<City> AddInclude(IQueryable<City> query)
    {
        return query.Include(x => x.Country);
    }

    protected override IQueryable<City> AddFilter(IQueryable<City> query, CitySearchObject search)
    {
        if (!string.IsNullOrWhiteSpace(search.Name))
        {
            query = query.Where(x => x.Name.Contains(search.Name));
        }
        return query;
    }
}
