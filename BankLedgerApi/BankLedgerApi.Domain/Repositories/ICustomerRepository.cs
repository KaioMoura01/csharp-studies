using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Domain.Repositories;

public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByIdWithAccountsAsync(Guid id);
    Task<Customer?> GetByDocumentNumberAsync(string documentNumber);
}
