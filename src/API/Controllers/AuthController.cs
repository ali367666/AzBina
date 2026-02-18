using Application.Abstracts.Services;
using Application.DTOs.Auth;
using Application.Shared.Helpers.Responses;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, error) = await _authService.RegisterAsync(request, ct);

        if (!success)
            return BadRequest(BaseResponse.Fail(error ?? "Registration failed."));

        return Ok(BaseResponse.Ok());
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var tokenResponse = await _authService.LoginAsync(request, ct);

            if (tokenResponse is null)
                return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid login or password."));

            return Ok(BaseResponse<TokenResponse>.Ok(tokenResponse));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<TokenResponse>.Fail(ex.Message));
        }
    }


    [HttpPost("admin/register-user")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> AdminRegisterUser([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var (success, error) = await _authService.RegisterAsync(request, ct);

        if (!success)
            return BadRequest(BaseResponse.Fail(error ?? "User creation failed."));

        return Ok(BaseResponse.Ok("User yaradıldı."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        // refresh token boşdursa BadRequest
        if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(BaseResponse.Fail("RefreshToken is required."));

        var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken, ct);

        // refresh token tapılmadı / vaxtı keçib / consume olunubsa -> Unauthorized
        if (tokenResponse is null)
            return Unauthorized(BaseResponse<TokenResponse>.Fail("Invalid or expired refresh token."));

        return Ok(BaseResponse<TokenResponse>.Ok(tokenResponse));
    }
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string token, CancellationToken ct)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
            return BadRequest("userId və token boş ola bilməz.");

        var ok = await _authService.ConfirmEmailAsync(userId, token, ct);

        if (!ok)
            return BadRequest("Token etibarsızdır və ya vaxtı keçib.");

        return Ok("Email təsdiqləndi.");
    }
}
