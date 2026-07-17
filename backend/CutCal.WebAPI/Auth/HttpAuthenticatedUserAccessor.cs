using CutCal.Services.Auth;

namespace CutCal.WebAPI.Auth;

public class HttpAuthenticatedUserAccessor : IAuthenticatedUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthenticatedUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst("Id")?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    public string? Role => _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}
