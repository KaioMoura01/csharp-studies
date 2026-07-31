using BankLedgerApi.Application.DTOs.Customers;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : ICustomerService
{
    public async Task<CustomerDetailsResponse> CreateAsync(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            TaxDocument = new TaxDocument(request.DocumentNumber, request.DocumentType),
            PasswordHash = passwordHasher.HashPassword(request.Password)
        };

        customerRepository.Add(customer);
        await unitOfWork.SaveChangesAsync();

        return customer.ToDetails();
    }

    public async Task<CustomerDetailsResponse?> GetByIdAsync(Guid id)
    {
        var customer = await customerRepository.GetByIdWithAccountsAsync(id);
        return customer?.ToDetails();
    }
}
