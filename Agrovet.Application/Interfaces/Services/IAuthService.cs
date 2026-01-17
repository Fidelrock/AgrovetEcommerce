namespace Agrovet.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(bool Success, string Token, string Message)> RegisterAsync(string email, string fullName, string phoneNumber, string password);
    Task<(bool Success, string Token, string Message)> LoginAsync(string email, string password);
    string GenerateJwtToken(Guid customerId, string email);
}
