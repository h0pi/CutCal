using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.WebAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Route("Access")]
public class AccessController : ControllerBase
{
    private readonly IAccessManager _accessManager;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    public AccessController(IAccessManager accessManager, IAuthenticatedUserAccessor userAccessor)
    {
        _accessManager = accessManager;
        _userAccessor = userAccessor;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        return Ok(await _accessManager.LoginAsync(request));
    }

    [AllowAnonymous]
    [HttpPost("LoginWithRefreshToken")]
    public async Task<ActionResult<LoginResponse>> LoginWithRefreshToken([FromBody] LoginWithRefreshTokenRequest request)
    {
        return Ok(await _accessManager.LoginWithRefreshTokenAsync(request));
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
    {
        return Ok(await _accessManager.RegisterAsync(request));
    }

    /// <summary>Invalidates the calling access token on the server and deletes the given refresh token.</summary>
    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var token = HttpContext.Items[JwtValidation.ValidatedTokenItem] as SecurityToken;
        await _accessManager.LogoutAsync(_userAccessor.UserId, request.RefreshToken, token?.Id, token?.ValidTo);
        return NoContent();
    }
}
