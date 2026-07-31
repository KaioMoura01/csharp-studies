using BankLedgerApi.Application.DTOs.Statements;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class StatementService(
    IAccountRepository accountRepository,
    ITransferRepository transferRepository) : IStatementService
{
    public async Task<StatementResponse?> GetAsync(Guid accountId, StatementQuery query)
    {
        if (query.InitDate > query.EndDate)
            throw new InvalidOperationException("Start date must not be after end date.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (query.InitDate > today || query.EndDate > today)
            throw new InvalidOperationException("Dates must not be in the future.");

        var account = await accountRepository.GetByIdAsync(accountId);
        if (account is null)
            return null;

        var fromDate = query.InitDate.ToDateTime(TimeOnly.MinValue);
        var toDateExclusive = query.EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var priorActivity = await transferRepository.GetActivityBeforeAsync(accountId, fromDate);
        var openingBalance = priorActivity.Sum(t => t.DestinationAccountId == accountId ? t.Amount : -t.Amount);

        var transfers = await transferRepository.GetActivityInRangeAsync(accountId, fromDate, toDateExclusive);

        var counterpartyIds = transfers
            .Select(t => t.DestinationAccountId == accountId ? t.SourceAccountId : t.DestinationAccountId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var counterpartyAccounts = await accountRepository.GetByIdsWithCustomerAsync(counterpartyIds);
        var counterparties = counterpartyAccounts.ToDictionary(a => a.Id, a => new { a.Number, OwnerName = a.Customer.Name });

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
