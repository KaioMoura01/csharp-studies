using BankLedgerApi.Application.DTOs.Accounts;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class AccountService(
    IAccountRepository accountRepository,
    ITransferRepository transferRepository,
    IUnitOfWork unitOfWork) : IAccountService
{
    public async Task<AccountCreatedResponse?> CreateAsync(CreateAccountRequest request)
    {
        var customerExists = await accountRepository.CustomerExistsAsync(request.CustomerId);
        if (!customerExists)
            return null;

        var account = new Account
        {
            Name = request.Name,
            Number = await GenerateUniqueNumberAsync(),
            Type = request.Type,
            CurrentBalance = 0m,
            IsActive = true,
            CustomerId = request.CustomerId
        };

        accountRepository.Add(account);
        await unitOfWork.SaveChangesAsync();

        return account.ToCreated();
    }

    public async Task<AccountDetailsResponse?> GetByIdAsync(Guid id)
    {
        var account = await accountRepository.GetByIdWithCustomerAsync(id);
        return account?.ToDetails();
    }

    public async Task<AccountDetailsResponse?> DepositAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");

        var account = await accountRepository.GetByIdWithCustomerAsync(accountId);
        if (account is null)
            return null;

        if (!account.IsActive)
            throw new InvalidOperationException("Account is not active.");

        account.CurrentBalance += amount;

        transferRepository.Add(new Transfer
        {
            SourceAccountId = null,
            DestinationAccountId = account.Id,
            Amount = amount,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow
        });

        await unitOfWork.SaveChangesAsync();

        return account.ToDetails();
    }

    public async Task<IReadOnlyList<AccountSummaryResponse>> GetByCustomerAsync(Guid customerId)
    {
        var accounts = await accountRepository.GetByCustomerAsync(customerId);
        return accounts.Select(a => a.ToSummary()).ToList();
    }

    private async Task<string> GenerateUniqueNumberAsync()
    {
        while (true)
        {
            var number = Random.Shared.NextInt64(0, 10_000_000_000L).ToString("D10");
            var exists = await accountRepository.NumberExistsAsync(number);
            if (!exists)
                return number;
        }
    }
}
