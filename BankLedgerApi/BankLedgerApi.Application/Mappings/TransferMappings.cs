using BankLedgerApi.Application.DTOs.Common;
using BankLedgerApi.Application.DTOs.Transfers;
using BankLedgerApi.Domain.Models;

namespace BankLedgerApi.Application.Mappings;

public static class TransferMappings
{
    public static TransferResponse ToResponse(this Transfer transfer, Account source, Account destination) =>
        new(transfer.Id,
            transfer.TransactionId,
            new AccountRefDto(source.Id, source.Number),
            new AccountRefDto(destination.Id, destination.Number),
            transfer.Amount,
            transfer.Status,
            new DateTimeOffset(DateTime.SpecifyKind(transfer.CreatedAt, DateTimeKind.Utc)));
}
