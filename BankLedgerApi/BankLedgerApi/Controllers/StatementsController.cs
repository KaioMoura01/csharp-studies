using System.Security.Claims;
using BankLedgerApi.Application.DTOs.Statements;
using BankLedgerApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BankLedgerApi.Controllers;

[ApiController]
[Authorize]
[Route("statements")]
[Produces("application/json")]
[Tags("Statements")]
public class StatementsController(IStatementService statementService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the account statement")]
    [EndpointDescription("Returns the ledger entries of the authenticated account within a date range, with opening balance, closing balance and running balance per entry. Requires a valid JWT.")]
    [ProducesResponseType<StatementResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] StatementQuery query)
    {
        var accountId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        try
        {
            var statement = await statementService.GetAsync(accountId, query);
            return statement is null ? NotFound() : Ok(statement);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
