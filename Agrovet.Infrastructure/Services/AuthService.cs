using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Application.Interfaces.Services;
using Agrovet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Agrovet.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IConfiguration _configuration;

    public AuthService(ICustomerRepository customerRepository, IConfiguration configuration)
    {
        _customerRepository = customerRepository;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Token, string Message)> RegisterAsync(
        string email,
        string fullName,
        string phoneNumber,
        string password)
    {
        if (await _customerRepository.EmailExistsAsync(email))
        {
            return (false, string.Empty, "Email already exists.");
        }

        var passwordHash = HashPassword(password);
        var customer = new Customer(email, fullName, phoneNumber, passwordHash);

        await _customerRepository.AddAsync(customer);

        var token = GenerateJwtToken(customer.Id, customer.Email);
        return (true, token, "Registration successful.");
    }

    public async Task<(bool Success, string Token, string Message)> LoginAsync(string email, string password)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);

        if (customer == null)
        {
            return (false, string.Empty, "Invalid email or password.");
        }

        if (!VerifyPassword(password, customer.PasswordHash))
        {
            return (false, string.Empty, "Invalid email or password.");
        }

        if (!customer.IsActive)
        {
            return (false, string.Empty, "Account is inactive.");
        }

        var token = GenerateJwtToken(customer.Id, customer.Email);
        return (true, token, "Login successful.");
    }

    public string GenerateJwtToken(Guid customerId, string email)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == passwordHash;
    }
}
