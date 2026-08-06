using BankLedgerApi.Application.DTOs.Tenants;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface ITenantService
{
    Task<TenantResponse> CreateAsync(CreateTenantRequest request);
    Task<TenantResponse?> GetByIdAsync(Guid id);
}
