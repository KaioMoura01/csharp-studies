using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;
using BankLedgerApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Infrastructure.Repositories;

public class AccountRepository(AppDbContext dbContext) : IAccountRepository
{
    public void Add(Account account) => dbContext.Accounts.Add(account);

    public Task<Account?> GetByIdAsync(Guid id) =>
        dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public Task<Account?> GetByIdWithCustomerAsync(Guid id) =>
        dbContext.Accounts.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == id);

    public Task<Account?> GetByNumberAsync(string number) =>
        dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == number);

    public async Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId) =>
        await dbContext.Accounts.Where(a => a.CustomerId == customerId).ToListAsync();

    public async Task<IReadOnlyList<Account>> GetByIdsWithCustomerAsync(IEnumerable<Guid> ids) =>
        await dbContext.Accounts.Include(a => a.Customer).Where(a => ids.Contains(a.Id)).ToListAsync();

    public Task<bool> NumberExistsAsync(string number) =>
        dbContext.Accounts.AnyAsync(a => a.Number == number);

    public Task<bool> CustomerExistsAsync(Guid customerId) =>
        dbContext.Customers.AnyAsync(c => c.Id == customerId);
}
