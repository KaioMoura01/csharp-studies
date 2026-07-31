using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;
using BankLedgerApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public void Add(Customer customer) => dbContext.Customers.Add(customer);

    public Task<Customer?> GetByIdAsync(Guid id) =>
        dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Customer?> GetByIdWithAccountsAsync(Guid id) =>
        dbContext.Customers.Include(c => c.Accounts).FirstOrDefaultAsync(c => c.Id == id);

    public Task<Customer?> GetByDocumentNumberAsync(string documentNumber) =>
        dbContext.Customers.FirstOrDefaultAsync(c => c.TaxDocument.Number == documentNumber);
}
