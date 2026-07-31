using BankLedgerApi.Domain.Enums;

namespace BankLedgerApi.Application.DTOs.Statements;

public record StatementEntryResponse(
    Guid TransferId,
    Guid TransactionId,
    DateTimeOffset Timestamp,
    EntryDirectionEnum Direction,
    decimal Amount,
    string CounterpartyAccountNumber,
    string CounterpartyName,
    decimal BalanceAfter,
    TransferStatusEnum Status);
