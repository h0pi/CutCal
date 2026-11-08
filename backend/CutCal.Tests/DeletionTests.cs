using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Services.Database;
using CutCal.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Tests;

public class DeletionTests : IDisposable
{
    private readonly CutCalDbContext _context;
    private readonly Salon _salon;

    public DeletionTests()
    {
        _context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Database.EnsureCreated();
        _salon = _context.Salons.First();
    }

    public void Dispose() => _context.Dispose();

    private SalonServiceService Services() => new(_context, new SignedInUser(0, RoleNames.Admin));

    [Fact]
    public async Task A_city_that_has_salons_is_not_deleted_and_the_reason_is_given()
    {
        var error = await Assert.ThrowsAsync<ClientException>(() => new CityService(_context).DeleteAsync(_salon.CityId));

        Assert.StartsWith("The city cannot be deleted because", error.Message);
        Assert.NotNull(_context.Cities.Find(_salon.CityId));
    }

    [Fact]
    public async Task A_city_without_salons_is_deleted()
    {
        var city = _context.Cities.Add(new City { Name = "Unused", CountryId = _context.Countries.First().Id }).Entity;
        await _context.SaveChangesAsync();

        await new CityService(_context).DeleteAsync(city.Id);

        Assert.Null(_context.Cities.Find(city.Id));
    }

    [Fact]
    public async Task A_category_that_has_salons_is_not_deleted_and_the_reason_is_given()
    {
        var error = await Assert.ThrowsAsync<ClientException>(() => new SalonCategoryService(_context).DeleteAsync(_salon.SalonCategoryId));

        Assert.StartsWith("The category cannot be deleted because", error.Message);
        Assert.NotNull(_context.SalonCategories.Find(_salon.SalonCategoryId));
    }

    [Fact]
    public async Task A_service_that_has_appointments_is_not_deleted_and_the_reason_is_given()
    {
        var serviceId = _context.Appointments.First().ServiceId;

        var error = await Assert.ThrowsAsync<ClientException>(() => Services().DeleteAsync(serviceId));

        Assert.StartsWith("The service cannot be deleted because it has", error.Message);
        Assert.NotNull(_context.SalonServices.Find(serviceId));
    }

    [Fact]
    public async Task A_service_without_appointments_is_deleted_together_with_its_staff_links()
    {
        var staff = _context.Staff.First(x => x.SalonId == _salon.Id);
        var service = _context.SalonServices.Add(new SalonService { SalonId = _salon.Id, Name = "Unbooked", DurationMinutes = 30, Price = 10 }).Entity;
        await _context.SaveChangesAsync();
        _context.StaffServices.Add(new StaffService { StaffId = staff.Id, ServiceId = service.Id });
        await _context.SaveChangesAsync();

        await Services().DeleteAsync(service.Id);

        Assert.Null(_context.SalonServices.Find(service.Id));
        Assert.Empty(_context.StaffServices.Where(x => x.ServiceId == service.Id));
    }
}
