using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Statements;

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
