using System.Security.Claims;
using BankLedgerApi.Application.DTOs.Auth;
using BankLedgerApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EndpointSummary("Authenticate a customer")]
    [EndpointDescription("Validates the customer's tax document (CPF/CNPJ) and password and returns a JWT bearer token scoped to their first active account.")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response is null ? Unauthorized() : Ok(response);
    }

    [Authorize]
    [HttpPost("switch-account/{accountId:guid}")]
    [EndpointSummary("Switch the active account")]
    [EndpointDescription("Reissues a JWT scoped to another account owned by the same authenticated customer. Does not require the password again.")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SwitchAccount(Guid accountId)
    {
        var customerId = Guid.Parse(User.FindFirstValue("customerId")!);

        var response = await authService.SwitchAccountAsync(customerId, accountId);
        return response is null ? Forbid() : Ok(response);
    }
}
