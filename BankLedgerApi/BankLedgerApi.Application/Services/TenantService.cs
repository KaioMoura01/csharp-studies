using BankLedgerApi.Application.DTOs.Tenants;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class TenantService(
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork) : ITenantService
{
    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug is required.");

        if (await tenantRepository.SlugExistsAsync(slug))
            throw new ArgumentException($"Slug '{slug}' is already taken.");

        var tenant = new Tenant
        {
            Name = request.Name,
            Slug = slug
        };

        tenantRepository.Add(tenant);
        await unitOfWork.SaveChangesAsync();

        return tenant.ToResponse();
    }

    public async Task<TenantResponse?> GetByIdAsync(Guid id)
    {
        var tenant = await tenantRepository.GetByIdAsync(id);
        return tenant?.ToResponse();
    }
}
