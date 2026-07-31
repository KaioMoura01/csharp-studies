using BankLedgerApi.Application.DTOs.Accounts;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface IAccountService
{
    Task<AccountCreatedResponse?> CreateAsync(CreateAccountRequest request);
    Task<AccountDetailsResponse?> GetByIdAsync(Guid id);
    Task<AccountDetailsResponse?> DepositAsync(Guid accountId, decimal amount);
    Task<IReadOnlyList<AccountSummaryResponse>> GetByCustomerAsync(Guid customerId);
}
