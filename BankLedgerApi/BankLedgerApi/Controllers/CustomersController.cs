using System.Security.Claims;
using BankLedgerApi.Application.DTOs.Accounts;
using BankLedgerApi.Application.DTOs.Customers;
using BankLedgerApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("customers")]
[Produces("application/json")]
[Tags("Customers")]
public class CustomersController(
    ICustomerService customerService,
    IAccountService accountService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a customer")]
    [EndpointDescription("Registers a customer with a name, a tax document (CPF or CNPJ) and a password used for login. The document number is validated by its digit count.")]
    [ProducesResponseType<CustomerDetailsResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        try
        {
            var created = await customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a customer by id")]
    [EndpointDescription("Returns the customer together with the summary of all accounts they own.")]
    [ProducesResponseType<CustomerDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await customerService.GetByIdAsync(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [Authorize]
    [HttpGet("{id:guid}/accounts")]
    [EndpointSummary("List a customer's accounts")]
    [EndpointDescription("Returns the accounts owned by the customer without repeating the owner data. Requires a valid JWT issued for that same customer.")]
    [ProducesResponseType<IEnumerable<AccountSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAccounts(Guid id)
    {
        var callerCustomerId = Guid.Parse(User.FindFirstValue("customerId")!);

        if (callerCustomerId != id)
            return Forbid();

        var accounts = await accountService.GetByCustomerAsync(id);
        return Ok(accounts);
    }
}
