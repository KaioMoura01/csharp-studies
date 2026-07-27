using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LibraryApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LibraryApi.Services;

public class TokenService:IToken
{
    public JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, IConfiguration configuration)
    {
        var sectionJwt = configuration.GetSection("JWT");
        var secretKey = sectionJwt
            .GetValue<string>("SecretKey") ?? throw new Exception("SecretKey not found");
        
        var privateKey = Encoding.UTF8.GetBytes(secretKey);
        
        var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(sectionJwt.GetValue<int>("TokenValidityInMinutes")),
            Audience = sectionJwt.GetValue<string>("Audience"),
            Issuer = sectionJwt.GetValue<string>("Issuer"),
            SigningCredentials = signinCredentials
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }

    public string GenerateRefreshToken()
    {
        var secureRandomBytes = new byte[128];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        
        randomNumberGenerator.GetBytes(secureRandomBytes);
        
        var refreshToken = Convert.ToBase64String(secureRandomBytes);
        
        return refreshToken;
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration)
    {
        var secretKey = configuration.GetSection("JWT")
            .GetValue<string>("SecretKey") ?? throw new Exception("SecretKey not found");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception("Invalid token");
        }
        
        return principal;
    }
}