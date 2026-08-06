using KShop.UserApi.Application.DTOs.Auth;
using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KShop.UserApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserProfileResponse>> Register(RegisterRequest request)
    {
        var profile = await authService.RegisterAsync(request);
        return Created(string.Empty, profile);
    }
}
