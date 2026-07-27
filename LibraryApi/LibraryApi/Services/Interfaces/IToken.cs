using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibraryApi.Services.Interfaces;

public interface IToken
{
    JwtSecurityToken GenerateToken(
        IEnumerable<Claim> claims, IConfiguration configuration);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, IConfiguration configuration);
}