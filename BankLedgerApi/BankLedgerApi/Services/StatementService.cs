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
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account is null)
            return null;

        var fromDate = query.From.ToDateTime(TimeOnly.MinValue);
        var toDateExclusive = query.To.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var openingBalance = await dbContext.Transfers
            .Where(t => t.Status == TransferStatusEnum.Completed
                        && t.CreatedAt < fromDate
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .SumAsync(t => t.DestinationAccountId == accountId ? t.Amount : -t.Amount);

        var transfers = await dbContext.Transfers
            .Where(t => t.Status == TransferStatusEnum.Completed
                        && t.CreatedAt >= fromDate
                        && t.CreatedAt < toDateExclusive
                        && (t.SourceAccountId == accountId || t.DestinationAccountId == accountId))
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();

        var counterpartyIds = transfers
            .Select(t => t.DestinationAccountId == accountId ? t.SourceAccountId : t.DestinationAccountId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var counterpartyNumbers = await dbContext.Accounts
            .Where(a => counterpartyIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Number);

        var entries = new List<StatementEntryResponse>();
        var runningBalance = openingBalance;

        foreach (var transfer in transfers)
        {
            var isCredit = transfer.DestinationAccountId == accountId;
            runningBalance += isCredit ? transfer.Amount : -transfer.Amount;

            Guid? counterpartyId = isCredit ? transfer.SourceAccountId : transfer.DestinationAccountId;
            var counterpartyNumber = counterpartyId is null
                ? "Deposit"
                : counterpartyNumbers.GetValueOrDefault(counterpartyId.Value, "unknown");

            entries.Add(new StatementEntryResponse(
                transfer.TransactionId,
                new DateTimeOffset(DateTime.SpecifyKind(transfer.CreatedAt, DateTimeKind.Utc)),
                isCredit ? EntryDirectionEnum.Credit : EntryDirectionEnum.Debit,
                transfer.Amount,
                counterpartyNumber,
                runningBalance,
                transfer.Status));
        }

        return new StatementResponse(
            account.Id,
            account.Number,
            query.From,
            query.To,
            openingBalance,
            runningBalance,
            entries);
    }
}
