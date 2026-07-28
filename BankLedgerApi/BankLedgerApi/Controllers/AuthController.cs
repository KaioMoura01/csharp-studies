using BankLedgerApi.DTOs.Auth;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EndpointSummary("Authenticate an account")]
    [EndpointDescription("Validates the account number and password and returns a JWT bearer token used to authorize account operations.")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}
