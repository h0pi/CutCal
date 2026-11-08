using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Services.Database;
using CutCal.Services.Services;
using CutCal.Services.Validators;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Tests;

public class SalonSettingsTests : IDisposable
{
    private readonly CutCalDbContext _context;
    private readonly Salon _salon;

    public SalonSettingsTests()
    {
        _context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Database.EnsureCreated();
        _salon = _context.Salons.First();
    }

    public void Dispose() => _context.Dispose();

    private SalonManagementService ServiceFor(int userId, string role) => new(_context, new SignedInUser(userId, role));

    private SalonManagementService Owner() => ServiceFor(_salon.OwnerId, RoleNames.SalonManager);

    private Salon Stored => _context.Salons.Include(x => x.WorkingHours).AsNoTracking().Single(x => x.Id == _salon.Id);

    private SalonUpdateRequest Profile(string name) => new()
    {
        Name = name,
        SalonCategoryId = _salon.SalonCategoryId,
        Description = _salon.Description,
        Address = _salon.Address,
        CityId = _salon.CityId,
        Latitude = _salon.Latitude,
        Longitude = _salon.Longitude
    };

    private static readonly SalonSettingsRequest EuroSettings = new()
    {
        AutoConfirm = true,
        Currency = "EUR",
        Is24HourFormat = false,
        TimeZone = "Europe/Sarajevo",
        WorkingHours = new() { new SalonWorkingHoursUpsertRequest { DayOfWeek = 1, OpenTime = new TimeOnly(8, 0), CloseTime = new TimeOnly(16, 0) } }
    };

    [Fact]
    public async Task Saving_the_profile_does_not_undo_the_settings()
    {
        await Owner().UpdateSettingsAsync(_salon.Id, EuroSettings);

        await Owner().UpdateAsync(_salon.Id, Profile("Renamed"));

        var salon = Stored;
        Assert.Equal("Renamed", salon.Name);
        Assert.True(salon.AutoConfirm);
        Assert.Equal("EUR", salon.Currency);
        Assert.False(salon.Is24HourFormat);
        Assert.Equal("Europe/Sarajevo", salon.TimeZone);
        var hours = Assert.Single(salon.WorkingHours);
        Assert.Equal(new TimeOnly(8, 0), hours.OpenTime);
    }

    [Fact]
    public async Task Saving_the_settings_does_not_touch_the_profile()
    {
        await Owner().UpdateAsync(_salon.Id, Profile("Renamed"));

        await Owner().UpdateSettingsAsync(_salon.Id, EuroSettings);

        Assert.Equal("Renamed", Stored.Name);
        Assert.Equal("EUR", Stored.Currency);
    }

    [Fact]
    public async Task Settings_that_are_not_sent_stay_as_they_are()
    {
        await Owner().UpdateSettingsAsync(_salon.Id, EuroSettings);

        await Owner().UpdateSettingsAsync(_salon.Id, new SalonSettingsRequest
        {
            WorkingHours = new() { new SalonWorkingHoursUpsertRequest { DayOfWeek = 2, OpenTime = new TimeOnly(10, 0), CloseTime = new TimeOnly(18, 0) } }
        });

        var salon = Stored;
        Assert.Equal("EUR", salon.Currency);
        Assert.True(salon.AutoConfirm);
        Assert.Equal(2, Assert.Single(salon.WorkingHours).DayOfWeek);
    }

    [Fact]
    public async Task Only_the_salons_own_manager_or_an_administrator_changes_the_settings()
    {
        var otherManagerId = _context.Salons.First(x => x.OwnerId != _salon.OwnerId).OwnerId;

        await Assert.ThrowsAsync<ClientException>(() => ServiceFor(otherManagerId, RoleNames.SalonManager).UpdateSettingsAsync(_salon.Id, EuroSettings));
        await ServiceFor(0, RoleNames.Admin).UpdateSettingsAsync(_salon.Id, EuroSettings);

        Assert.Equal("EUR", Stored.Currency);
    }

    [Fact]
    public void The_time_at_a_salon_follows_its_own_time_zone()
    {
        var far = new Salon { TimeZone = "Pacific/Kiritimati" }; // UTC+14, all year
        var utc = new Salon { TimeZone = "UTC" };

        Assert.InRange((SalonClock.Now(far) - DateTime.UtcNow).TotalHours, 13.9, 14.1);
        Assert.InRange((SalonClock.Now(utc) - DateTime.UtcNow).TotalHours, -0.1, 0.1);
    }

    [Fact]
    public void An_unknown_currency_or_an_empty_time_zone_is_rejected_but_unsent_settings_are_fine()
    {
        var validator = new SalonSettingsRequestValidator();

        Assert.False(validator.Validate(new SalonSettingsRequest { Currency = "XYZ" }).IsValid);
        Assert.False(validator.Validate(new SalonSettingsRequest { Currency = "BAM" }).IsValid); // neither PayPal nor Stripe can charge it
        Assert.False(validator.Validate(new SalonSettingsRequest { TimeZone = "" }).IsValid);
        Assert.False(validator.Validate(new SalonSettingsRequest { TimeZone = "Not/AZone" }).IsValid);
        Assert.True(validator.Validate(new SalonSettingsRequest()).IsValid);
        Assert.True(validator.Validate(EuroSettings).IsValid);
    }
}
