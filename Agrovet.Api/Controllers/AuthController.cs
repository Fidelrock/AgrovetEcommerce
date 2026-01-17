using Agrovet.Application.DTOs.Auth;
using Agrovet.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agrovet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var (success, token, message) = await _authService.RegisterAsync(
            request.Email,
            request.FullName,
            request.PhoneNumber,
            request.Password
        );

        if (!success)
        {
            return BadRequest(new { message });
        }

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = request.Email,
            FullName = request.FullName,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var (success, token, message) = await _authService.LoginAsync(request.Email, request.Password);

        if (!success)
        {
            return Unauthorized(new { message });
        }

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = request.Email,
            FullName = string.Empty,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
    }
}
