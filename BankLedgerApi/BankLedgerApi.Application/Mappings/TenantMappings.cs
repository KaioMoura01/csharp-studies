using BankLedgerApi.Application.DTOs.Tenants;
using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Application.Mappings;

public static class TenantMappings
{
    public static TenantResponse ToResponse(this Tenant tenant) =>
        new(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive);
}
