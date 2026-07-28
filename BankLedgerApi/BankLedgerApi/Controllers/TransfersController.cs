using System.Security.Claims;
using BankLedgerApi.DTOs.Transfers;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BankLedgerApi.Controllers;

[ApiController]
[Authorize]
[Route("transfers")]
[Produces("application/json")]
[Tags("Transfers")]
public class TransfersController(ITransferService transferService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Transfer to another account")]
    [EndpointDescription("Moves an amount from the authenticated account to a destination account identified by its number, inside a database transaction. Requires a valid JWT.")]
    [ProducesResponseType<TransferResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CreateTransferRequest request)
    {
        var sourceAccountId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        try
        {
            var response = await transferService.ExecuteAsync(sourceAccountId, request);
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
