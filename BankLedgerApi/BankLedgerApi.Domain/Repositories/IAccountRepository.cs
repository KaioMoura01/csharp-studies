using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Domain.Repositories;

public interface IAccountRepository
{
    void Add(Account account);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByIdWithCustomerAsync(Guid id);
    Task<Account?> GetByNumberAsync(string number);
    Task<IReadOnlyList<Account>> GetByCustomerAsync(Guid customerId);
    Task<IReadOnlyList<Account>> GetByIdsWithCustomerAsync(IEnumerable<Guid> ids);
    Task<bool> NumberExistsAsync(string number);
    Task<bool> CustomerExistsAsync(Guid customerId);
}
