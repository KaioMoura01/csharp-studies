namespace BankLedgerApi.DTOs.Statements;

public record StatementResponse(
    Guid AccountId,
    string AccountNumber,
    DateOnly From,
    DateOnly To,
    decimal OpeningBalance,
    decimal ClosingBalance,
    IReadOnlyList<StatementEntryResponse> Entries);
