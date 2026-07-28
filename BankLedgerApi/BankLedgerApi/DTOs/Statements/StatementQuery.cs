namespace BankLedgerApi.DTOs.Statements;

public record StatementQuery(
    DateOnly From,
    DateOnly To);
