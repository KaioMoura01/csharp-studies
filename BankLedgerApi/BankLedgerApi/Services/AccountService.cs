using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Accounts;
using BankLedgerApi.Enums;
using BankLedgerApi.Mappings;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Services;

public class AccountService(
    AppDbContext dbContext,
    IPasswordHasher<Account> passwordHasher) : IAccountService
{
    public async Task<AccountCreatedResponse?> CreateAsync(CreateAccountRequest request)
    {
        var customerExists = await dbContext.Customers.AnyAsync(c => c.Id == request.CustomerId);
        if (!customerExists)
            return null;

        var account = new Account
        {
            Name = request.Name,
            Number = await GenerateUniqueNumberAsync(),
            Type = request.Type,
            PasswordHash = string.Empty,
            CurrentBalance = 0m,
            IsActive = true,
            CustomerId = request.CustomerId
        };

        account.PasswordHash = passwordHasher.HashPassword(account, request.Password);

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync();

        return account.ToCreated();
    }

    public async Task<AccountDetailsResponse?> GetByIdAsync(Guid id)
    {
        var account = await dbContext.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == id);

        return account?.ToDetails();
    }

    public async Task<AccountDetailsResponse?> DepositAsync(Guid accountId, decimal amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");

        var account = await dbContext.Accounts
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account is null)
            return null;

        if (!account.IsActive)
            throw new InvalidOperationException("Account is not active.");

        account.CurrentBalance += amount;

        dbContext.Transfers.Add(new Transfer
        {
            SourceAccountId = null,
            DestinationAccountId = account.Id,
            Amount = amount,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        return account.ToDetails();
    }

    public async Task<IReadOnlyList<AccountSummaryResponse>> GetByCustomerAsync(Guid customerId)
    {
        return await dbContext.Accounts
            .Where(a => a.CustomerId == customerId)
            .Select(a => new AccountSummaryResponse(
                a.Id, a.Name, a.Number, a.Type, a.CurrentBalance, a.IsActive))
            .ToListAsync();
    }

    private async Task<string> GenerateUniqueNumberAsync()
    {
        while (true)
        {
            var number = Random.Shared.NextInt64(0, 10_000_000_000L).ToString("D10");
            var exists = await dbContext.Accounts.AnyAsync(a => a.Number == number);
            if (!exists)
                return number;
        }
    }
}
