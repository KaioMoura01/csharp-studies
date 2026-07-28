using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankLedgerApi.Configurations;
using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Auth;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankLedgerApi.Services;

public class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<Account> passwordHasher,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Number == request.AccountNumber);

        if (account is null || !account.IsActive)
            return null;

        var verification = passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            return null;

        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim("accountNumber", account.Number),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse(serializedToken, new DateTimeOffset(expiresAt, TimeSpan.Zero));
    }
}
