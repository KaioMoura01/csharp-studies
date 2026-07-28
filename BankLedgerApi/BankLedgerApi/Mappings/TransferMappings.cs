using BankLedgerApi.DTOs.Common;
using BankLedgerApi.DTOs.Transfers;
using BankLedgerApi.Models;

namespace BankLedgerApi.Mappings;

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
