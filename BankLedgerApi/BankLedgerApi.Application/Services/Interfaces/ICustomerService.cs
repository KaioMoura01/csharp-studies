using BankLedgerApi.Application.DTOs.Customers;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerDetailsResponse> CreateAsync(CreateCustomerRequest request);
    Task<CustomerDetailsResponse?> GetByIdAsync(Guid id);
}
