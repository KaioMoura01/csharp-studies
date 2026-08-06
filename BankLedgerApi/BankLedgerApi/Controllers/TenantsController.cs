using BankLedgerApi.Application.DTOs.Tenants;
using BankLedgerApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("tenants")]
[Produces("application/json")]
[Tags("Tenants")]
public class TenantsController(ITenantService tenantService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a tenant")]
    [EndpointDescription("Provisions a new tenant (e.g. a bank) identified by a unique slug. Every customer, account and transfer created afterwards must be scoped to a tenant via the X-Tenant-Id header.")]
    [ProducesResponseType<TenantResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateTenantRequest request)
    {
        try
        {
            var created = await tenantService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Get a tenant by id")]
    [ProducesResponseType<TenantResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenant = await tenantService.GetByIdAsync(id);
        return tenant is null ? NotFound() : Ok(tenant);
    }
}
