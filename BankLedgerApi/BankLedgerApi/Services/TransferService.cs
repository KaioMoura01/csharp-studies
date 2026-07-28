using BankLedgerApi.Context;
using BankLedgerApi.DTOs.Transfers;
using BankLedgerApi.Enums;
using BankLedgerApi.Mappings;
using BankLedgerApi.Models;
using BankLedgerApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankLedgerApi.Services;

public class TransferService(AppDbContext dbContext) : ITransferService
{
    public async Task<TransferResponse> ExecuteAsync(Guid sourceAccountId, CreateTransferRequest request)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");

        var source = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == sourceAccountId)
            ?? throw new InvalidOperationException("Source account not found.");

        var destination = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Number == request.DestinationAccountNumber)
            ?? throw new InvalidOperationException("Destination account not found.");

        if (destination.Id == source.Id)
            throw new InvalidOperationException("Source and destination accounts must be different.");

        if (!source.IsActive || !destination.IsActive)
            throw new InvalidOperationException("Both accounts must be active.");

        if (source.CurrentBalance < request.Amount)
            throw new InvalidOperationException("Insufficient balance.");

        await using var databaseTransaction = await dbContext.Database.BeginTransactionAsync();

        source.CurrentBalance -= request.Amount;
        destination.CurrentBalance += request.Amount;

        var transfer = new Transfer
        {
            SourceAccountId = source.Id,
            DestinationAccountId = destination.Id,
            Amount = request.Amount,
            TransactionId = Guid.NewGuid(),
            Status = TransferStatusEnum.Completed,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Transfers.Add(transfer);
        await dbContext.SaveChangesAsync();
        await databaseTransaction.CommitAsync();

        return transfer.ToResponse(source, destination);
    }
}
