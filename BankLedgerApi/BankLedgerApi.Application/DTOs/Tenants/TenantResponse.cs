namespace BankLedgerApi.Application.DTOs.Tenants;

public record TenantResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive);
