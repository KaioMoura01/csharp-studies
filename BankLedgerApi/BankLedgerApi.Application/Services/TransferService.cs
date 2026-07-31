using BankLedgerApi.Application.DTOs.Transfers;
using BankLedgerApi.Application.Mappings;
using BankLedgerApi.Application.Security;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Domain.Enums;
using BankLedgerApi.Domain.Models;
using BankLedgerApi.Domain.Repositories;

namespace BankLedgerApi.Application.Services;

public class TransferService(
    IAccountRepository accountRepository,
    ITransferRepository transferRepository,
    ICustomerRepository customerRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : ITransferService
{
    public async Task<TransferResponse> ExecuteAsync(Guid sourceAccountId, CreateTransferRequest request)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Amount must be greater than zero.");

        var source = await accountRepository.GetByIdAsync(sourceAccountId)
            ?? throw new InvalidOperationException("Source account not found.");

        var customer = await customerRepository.GetByIdAsync(source.CustomerId)
            ?? throw new InvalidOperationException("Source account not found.");

        if (passwordHasher.VerifyPassword(customer.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            throw new InvalidOperationException("Invalid password.");

        var destination = await accountRepository.GetByNumberAsync(request.DestinationAccountNumber)
            ?? throw new InvalidOperationException("Destination account not found.");

        if (destination.Id == source.Id)
            throw new InvalidOperationException("Source and destination accounts must be different.");

        if (!source.IsActive || !destination.IsActive)
            throw new InvalidOperationException("Both accounts must be active.");

        if (source.CurrentBalance < request.Amount)
            throw new InvalidOperationException("Insufficient balance.");

        await using var transaction = await unitOfWork.BeginTransactionAsync();

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

        transferRepository.Add(transfer);
        await unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return transfer.ToResponse(source, destination);
    }
}
