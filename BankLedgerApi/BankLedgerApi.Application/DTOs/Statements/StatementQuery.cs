namespace BankLedgerApi.Application.DTOs.Statements;

public record StatementQuery(
    DateOnly InitDate,
    DateOnly EndDate);
