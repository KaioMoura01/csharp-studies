using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Application.DTOs.Reversals;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class ReversalService(
    IAccountRepository accountRepository,
    ITransferRepository transferRepository,
    ICustomerRepository customerRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IReversalService
{
    public async Task<ReversalResponse?> ReverseAsync(Guid callerAccountId, Guid transferId, string password)
    {
        var original = await transferRepository.GetByIdAsync(transferId);
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

        var source = await accountRepository.GetByIdAsync(original.SourceAccountId.Value)
            ?? throw new InvalidOperationException("Source account not found.");
        var destination = await accountRepository.GetByIdAsync(original.DestinationAccountId)
            ?? throw new InvalidOperationException("Destination account not found.");

        var customer = await customerRepository.GetByIdAsync(source.CustomerId)
            ?? throw new InvalidOperationException("Source account not found.");

        if (passwordHasher.VerifyPassword(customer.PasswordHash, password) == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Invalid password.");

        if (!source.IsActive || !destination.IsActive)
            throw new InvalidOperationException("Both accounts must be active.");

        if (destination.CurrentBalance < original.Amount)
            throw new InvalidOperationException("Insufficient balance in the destination account to reverse.");

        await using var transaction = await unitOfWork.BeginTransactionAsync();

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

        transferRepository.Add(reversal);
        await unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return new ReversalResponse(
            original.Id,
            reversal.TransactionId,
            new AccountRefDto(destination.Id, destination.Number),
            new AccountRefDto(source.Id, source.Number),
            reversal.Amount,
            new DateTimeOffset(DateTime.SpecifyKind(reversal.CreatedAt, DateTimeKind.Utc)));
    }
}
