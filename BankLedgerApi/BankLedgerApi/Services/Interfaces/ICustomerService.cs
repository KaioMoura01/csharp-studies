using BankLedgerApi.DTOs.Customers;

namespace BankLedgerApi.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerDetailsResponse> CreateAsync(CreateCustomerRequest request);
    Task<CustomerDetailsResponse?> GetByIdAsync(Guid id);
}
