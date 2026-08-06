namespace BankLedgerApi.Application.DTOs.Tenants;

public record CreateTenantRequest(
    string Name,
    string Slug);
