using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Domain.Repositories;

public interface ITenantRepository
{
    void Add(Tenant tenant);
    Task<Tenant?> GetByIdAsync(Guid id);
    Task<Tenant?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug);
}
