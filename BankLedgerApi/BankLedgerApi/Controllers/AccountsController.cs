using System.Security.Claims;
using BankLedgerApi.DTOs.Accounts;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("accounts")]
[Produces("application/json")]
[Tags("Accounts")]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Open an account")]
    [EndpointDescription("Creates an account for an existing customer, generating a unique account number and storing the password as a hash.")]
    [ProducesResponseType<AccountCreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateAccountRequest request)
    {
        var created = await accountService.CreateAsync(request);
        return created is null
            ? NotFound("Customer not found.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpGet("me")]
    [EndpointSummary("Get the authenticated account")]
    [EndpointDescription("Returns the full data of the account identified by the bearer token, including the owner id. Requires a valid JWT.")]
    [ProducesResponseType<AccountDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrent()
    {
        var accountId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        var account = await accountService.GetByIdAsync(accountId);
        return account is null ? NotFound() : Ok(account);
    }

    [Authorize]
    [HttpPost("deposit")]
    [EndpointSummary("Deposit into the authenticated account")]
    [EndpointDescription("Credits an amount to the account identified by the bearer token and records it as a ledger entry. Requires a valid JWT.")]
    [ProducesResponseType<AccountDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Deposit(DepositRequest request)
    {
        var accountId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        try
        {
            var account = await accountService.DepositAsync(accountId, request.Amount);
            return account is null ? NotFound() : Ok(account);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get an account by id")]
    [EndpointDescription("Returns the full data of an account, including the owner reference.")]
    [ProducesResponseType<AccountDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await accountService.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }
}
