using BankLedgerApi.Enums;

namespace BankLedgerApi.DTOs.Statements;

public record StatementEntryResponse(
    Guid TransactionId,
    DateTimeOffset Timestamp,
    EntryDirectionEnum Direction,
    decimal Amount,
    string CounterpartyAccountNumber,
    decimal BalanceAfter,
    TransferStatusEnum Status);
