using System.Security.Claims;
using BankLedgerApi.Application.DTOs.Reversals;
using BankLedgerApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BankLedgerApi.Controllers;

[ApiController]
[Authorize]
[Route("reversals")]
[Produces("application/json")]
[Tags("Reversals")]
public class ReversalsController(IReversalService reversalService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Reverse a transfer")]
    [EndpointDescription("Reverses a completed transfer originated by the authenticated account, moving the amount back and recording a compensating ledger entry. Requires a valid JWT.")]
    [ProducesResponseType<ReversalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(ReversalRequest request)
    {
        var accountId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        try
        {
            var response = await reversalService.ReverseAsync(accountId, request.TransferId, request.Password);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
