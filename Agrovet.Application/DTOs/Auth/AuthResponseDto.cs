namespace Agrovet.Application.DTOs.Auth;

public sealed class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
