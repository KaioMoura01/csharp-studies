namespace BankLedgerApi.DTOs.Transfers;

public record CreateTransferRequest(
    string DestinationAccountNumber,
    decimal Amount);
