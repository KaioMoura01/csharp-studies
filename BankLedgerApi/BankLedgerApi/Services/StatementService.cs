using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Statements;
using BankLedgerApi.Enums;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Services;

public class StatementService(AppDbContext dbContext) : IStatementService
{
    public async Task<StatementResponse?> GetAsync(Guid accountId, StatementQuery query)
    {
        if (query.InitDate > query.EndDate)
            throw new InvalidOperationException("Start date must not be after end date.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (query.InitDate > today || query.EndDate > today)
            throw new InvalidOperationException("Dates must not be in the future.");

        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account is null)
            return null;

        var fromDate = query.InitDate.ToDateTime(TimeOnly.MinValue);
        var toDateExclusive = query.EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var openingBalance = await dbContext.Transfers
            .Where(t => (t.Status == TransferStatusEnum.Completed || t.Status == TransferStatusEnum.Reversed)
                        && t.CreatedAt < fromDate
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .SumAsync(t => t.DestinationAccountId == accountId ? t.Amount : -t.Amount);

        var transfers = await dbContext.Transfers
            .Where(t => (t.Status == TransferStatusEnum.Completed || t.Status == TransferStatusEnum.Reversed)
                        && t.CreatedAt >= fromDate
                        && t.CreatedAt < toDateExclusive
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .OrderByDescending( t => t.CreatedAt)
            .ToListAsync();

        var counterpartyIds = transfers
            .Select(t => t.DestinationAccountId == accountId ? t.SourceAccountId : t.DestinationAccountId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var counterparties = await dbContext.Accounts
            .Include(a => a.Customer)
            .Where(a => counterpartyIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => new { a.Number, OwnerName = a.Customer.Name });

        var entries = new List<StatementEntryResponse>();
        var runningBalance = openingBalance;

        foreach (var transfer in transfers)
        {
            var isCredit = transfer.DestinationAccountId == accountId;
            runningBalance += isCredit ? transfer.Amount : -transfer.Amount;

            Guid? counterpartyId = isCredit ? transfer.SourceAccountId : transfer.DestinationAccountId;
            var counterparty = counterpartyId is null
                ? null
                : counterparties.GetValueOrDefault(counterpartyId.Value);

            entries.Add(new StatementEntryResponse(
                transfer.Id,
                transfer.TransactionId,
                new DateTimeOffset(DateTime.SpecifyKind(transfer.CreatedAt, DateTimeKind.Utc)),
                isCredit ? EntryDirectionEnum.Credit : EntryDirectionEnum.Debit,
                transfer.Amount,
                counterparty?.Number ?? "Deposit",
                counterparty?.OwnerName ?? "Depósito",
                runningBalance,
                transfer.Status));
        }

        return new StatementResponse(
            account.Id,
            account.Number,
            query.InitDate,
            query.EndDate,
            openingBalance,
            runningBalance,
            entries);
    }
}
