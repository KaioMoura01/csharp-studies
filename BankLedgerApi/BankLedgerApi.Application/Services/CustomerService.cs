using BankLedgerApi.Application.DTOs.Customers;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Multitenancy;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class CustomerService(
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITenantContext tenantContext) : ICustomerService
{
    public async Task<CustomerDetailsResponse> CreateAsync(CreateCustomerRequest request)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new ArgumentException("Tenant could not be resolved for this request.");

        var customer = new Customer
        {
            TenantId = tenantId,
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
