namespace BankLedgerApi.DTOs.Statements;

public record StatementQuery(
    DateOnly InitDate,
    DateOnly EndDate);
