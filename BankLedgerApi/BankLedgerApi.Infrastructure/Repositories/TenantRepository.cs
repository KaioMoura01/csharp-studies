using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;
using BankLedgerApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Infrastructure.Repositories;

public class TenantRepository(AppDbContext dbContext) : ITenantRepository
{
    public void Add(Tenant tenant) => dbContext.Tenants.Add(tenant);

    public Task<Tenant?> GetByIdAsync(Guid id) =>
        dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id);

    public Task<Tenant?> GetBySlugAsync(string slug) =>
        dbContext.Tenants.FirstOrDefaultAsync(t => t.Slug == slug);

    public Task<bool> SlugExistsAsync(string slug) =>
        dbContext.Tenants.AnyAsync(t => t.Slug == slug);
}
