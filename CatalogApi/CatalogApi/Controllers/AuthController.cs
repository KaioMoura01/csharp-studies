using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    ITokenService tokenService,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration):ControllerBase
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IConfiguration _configuration =  configuration;

    private async Task<ApplicationUser> GetUserAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        
        return user ?? throw new BadHttpRequestException("User not found");
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model.UserName is null || model.Password is null)
        {
            return Unauthorized();
        }
        
        var user = await GetUserAsync(model.UserName);

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return BadRequest("Username and password are required");
        }
        
        var userRoles = await _userManager.GetRolesAsync(user);
                                                                                                                                                                                                                            
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var token = _tokenService.GenerateToken(authClaims, _configuration);
        
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);
        
        user.RefreshToken = refreshToken;
        
        user.RefreshTokenExpiryToken = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);
        
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = token.ValidTo
        });
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName!);
        
        if (userExists is not null)
        {
            return StatusCode(
                StatusCodes.Status409Conflict,
                new Response
                {
                    Status = "Error",
                    Message = "User already exists"
                });
        }

        var user = new ApplicationUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new Response
                {
                    Status = "Error",
                    Message = "User creation failed"
                });
        }
        
        return Ok(new Response{Status = "Success", Message = "User created successfully"});
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel.AccessToken is null || tokenModel.RefreshToken is null)
        {
            throw new ArgumentNullException(nameof(tokenModel));
        }

        var token = tokenModel.AccessToken;
        var refreshToken = tokenModel.RefreshToken;
        
        var principal = _tokenService.GetPrincipalFromExpiredToken(token!, _configuration);

        if (principal?.Identity is null)
        {
            return BadRequest("Invalid token/refresh token");
        }

        var userName = principal.Identity.Name;
        var user = await GetUserAsync(userName!);

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryToken <= DateTime.UtcNow)
        {
            return BadRequest("Invalid token/refresh token");
        }

        var newAccessToken = _tokenService.GenerateToken(principal.Claims.ToList(), _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        
        await  _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            Status = "Success",
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
        });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{userName}")]
    public async Task<IActionResult> Revoke(string userName)
    {
        var user = await GetUserAsync(userName);
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        
        return NoContent();
    }
}