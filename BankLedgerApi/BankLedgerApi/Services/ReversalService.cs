using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Common;
using BankLedgerApi.DTOs.Reversals;
using BankLedgerApi.Enums;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Services;

public class ReversalService(AppDbContext dbContext) : IReversalService
{
    public async Task<ReversalResponse?> ReverseAsync(Guid callerAccountId, Guid transferId)
    {
        var original = await dbContext.Transfers.FirstOrDefaultAsync(t => t.Id == transferId);
        if (original is null)
            return null;

        if (original.SourceAccountId is null)
            throw new InvalidOperationException("Deposits cannot be reversed.");

        if (original.Status == TransferStatusEnum.Reversed)
            throw new InvalidOperationException("Transfer has already been reversed.");

        if (original.Status != TransferStatusEnum.Completed)
            throw new InvalidOperationException("Only completed transfers can be reversed.");

        if (original.SourceAccountId != callerAccountId)
            throw new InvalidOperationException("Only the source account can reverse this transfer.");

        var source = await dbContext.Accounts.FirstAsync(a => a.Id == original.SourceAccountId.Value);
        var destination = await dbContext.Accounts.FirstAsync(a => a.Id == original.DestinationAccountId);

        if (!source.IsActive || !destination.IsActive)
            throw new InvalidOperationException("Both accounts must be active.");

        if (destination.CurrentBalance < original.Amount)
            throw new InvalidOperationException("Insufficient balance in the destination account to reverse.");

        await using var databaseTransaction = await dbContext.Database.BeginTransactionAsync();

        destination.CurrentBalance -= original.Amount;
        source.CurrentBalance += original.Amount;
        original.Status = TransferStatusEnum.Reversed;

        var reversal = new Transfer
        {
            SourceAccountId = destination.Id,
            DestinationAccountId = source.Id,
            Amount = original.Amount,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow,
            ReversedTransferId = original.Id
        };

        dbContext.Transfers.Add(reversal);
        await dbContext.SaveChangesAsync();
        await databaseTransaction.CommitAsync();

        return new ReversalResponse(
            original.Id,
            reversal.TransactionId,
            new AccountRefDto(destination.Id, destination.Number),
            new AccountRefDto(source.Id, source.Number),
            reversal.Amount,
            new DateTimeOffset(DateTime.SpecifyKind(reversal.CreatedAt, DateTimeKind.Utc)));
    }
}
