using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CatalogApi.Services;

public interface ITokenService
{
    JwtSecurityToken GenerateToken(
        IEnumerable<Claim> claims, IConfiguration configuration);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration);
}