using CutCal.Model.Requests;
using CutCal.Model.Responses;
using CutCal.WebAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CutCal.WebAPI.Controllers;

[ApiController]
[Route("Access")]
[AllowAnonymous]
public class AccessController : ControllerBase
{
    private readonly IAccessManager _accessManager;

    public AccessController(IAccessManager accessManager)
    {
        _accessManager = accessManager;
    }

    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        return Ok(await _accessManager.LoginAsync(request));
    }

    [HttpPost("LoginWithRefreshToken")]
    public async Task<ActionResult<LoginResponse>> LoginWithRefreshToken([FromBody] LoginWithRefreshTokenRequest request)
    {
        return Ok(await _accessManager.LoginWithRefreshTokenAsync(request));
    }

    [HttpPost("Register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
    {
        return Ok(await _accessManager.RegisterAsync(request));
    }
}
