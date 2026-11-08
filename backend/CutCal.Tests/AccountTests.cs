using CutCal.Common.Services;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Services.Database;
using CutCal.Services.Services;
using CutCal.WebAPI.Auth;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Tests;

public class AccountTests : IDisposable
{
    private readonly CutCalDbContext _context;
    private readonly User _user;

    public AccountTests()
    {
        Environment.SetEnvironmentVariable("JwtToken__SecretKey", "test-secret-key-test-secret-key-test-secret-key");

        _context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Database.EnsureCreated();
        _user = _context.Users.First();
        _user.IsActive = false;
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    private static UserUpdateRequest Update(User user, bool isActive) => new()
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        IsActive = isActive
    };

    private UserService ServiceFor(string role) => new(_context, new CryptoService(), new SignedInUser(_user.Id, role));

    [Fact]
    public async Task A_deactivated_user_cannot_activate_their_own_account()
    {
        await ServiceFor(RoleNames.Customer).UpdateAsync(_user.Id, Update(_user, isActive: true));

        Assert.False(_context.Users.Single(x => x.Id == _user.Id).IsActive);
    }

    [Fact]
    public async Task An_administrator_can_activate_an_account()
    {
        await ServiceFor(RoleNames.Admin).UpdateAsync(_user.Id, Update(_user, isActive: true));

        Assert.True(_context.Users.Single(x => x.Id == _user.Id).IsActive);
    }

    [Fact]
    public async Task A_deactivated_user_cannot_renew_the_session_with_a_refresh_token()
    {
        _context.RefreshTokens.Add(new RefreshToken { UserId = _user.Id, Token = "refresh", ExpiresAt = DateTime.UtcNow.AddDays(1) });
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() =>
            new AccessManager(_context, new CryptoService()).LoginWithRefreshTokenAsync(new LoginWithRefreshTokenRequest { RefreshToken = "refresh" }));

        Assert.Equal("This account is deactivated.", error.Message);
    }
}
