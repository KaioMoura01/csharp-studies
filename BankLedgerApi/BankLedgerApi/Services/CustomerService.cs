using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Customers;
using BankLedgerApi.Mappings;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Services;

public class CustomerService(AppDbContext dbContext) : ICustomerService
{
    public async Task<CustomerDetailsResponse> CreateAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            TaxDocument = new TaxDocument(request.DocumentNumber, request.DocumentType)
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync();

        return customer.ToDetails();
    }

    public async Task<CustomerDetailsResponse?> GetByIdAsync(Guid id)
    {
        var customer = await dbContext.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer?.ToDetails();
    }
}
