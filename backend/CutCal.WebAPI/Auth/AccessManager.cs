using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CutCal.Common.Services;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Services.Database;
using CutCal.Services.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CutCal.WebAPI.Auth;

public interface IAccessManager
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> LoginWithRefreshTokenAsync(LoginWithRefreshTokenRequest request);
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync(int userId, string? refreshToken);
}

public class AccessManager : IAccessManager
{
    private readonly CutCalDbContext _context;
    private readonly ICryptoService _crypto;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _durationInMinutes;

    public AccessManager(CutCalDbContext context, ICryptoService crypto)
    {
        _context = context;
        _crypto = crypto;
        _secretKey = Environment.GetEnvironmentVariable("JwtToken__SecretKey") ?? throw new InvalidOperationException("JwtToken__SecretKey is not configured.");
        _issuer = Environment.GetEnvironmentVariable("JwtToken__Issuer") ?? "CutCal";
        _audience = Environment.GetEnvironmentVariable("JwtToken__Audience") ?? "CutCalUsers";
        _durationInMinutes = int.TryParse(Environment.GetEnvironmentVariable("JwtToken__DurationInMinutes"), out var d) ? d : 60;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user is null || !_crypto.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new ClientException("Invalid username or password.");
        }
        if (!user.IsActive)
        {
            throw new ClientException("This account is deactivated.");
        }

        return await BuildLoginResponseAsync(user);
    }

    public async Task<LoginResponse> LoginWithRefreshTokenAsync(LoginWithRefreshTokenRequest request)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (token is null || token.ExpiresAt < DateTime.UtcNow)
        {
            throw new ClientException("Invalid or expired refresh token.");
        }

        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();

        return await BuildLoginResponseAsync(token.User);
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new ClientException("Password and confirmation do not match.");
        }
        if (await _context.Users.AnyAsync(x => x.Username == request.Username))
        {
            throw new ClientException("Username is already taken.");
        }
        if (await _context.Users.AnyAsync(x => x.Email == request.Email))
        {
            throw new ClientException("Email is already registered.");
        }

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = _crypto.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var customerRole = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "Customer")
            ?? throw new ClientException("Default Customer role is not configured.");
        _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = customerRole.Id });
        await _context.SaveChangesAsync();

        user.UserRoles = new List<UserRole> { new() { UserId = user.Id, RoleId = customerRole.Id, Role = customerRole } };
        return user.Adapt<UserResponse>();
    }

    public async Task LogoutAsync(int userId, string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return;
        }

        var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken);
        if (token is not null)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<LoginResponse> BuildLoginResponseAsync(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var primaryRole = roles.FirstOrDefault() ?? "Customer";

        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName),
            new("Email", user.Email),
            new(ClaimTypes.Role, primaryRole),
            new("IsActive", user.IsActive.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_durationInMinutes);

        var token = new JwtSecurityToken(_issuer, _audience, claims, expires: expires, signingCredentials: credentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = expires,
            User = user.Adapt<UserResponse>()
        };
    }
}
