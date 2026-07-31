namespace BankLedgerApi.Application.DTOs.Transfers;

public record CreateTransferRequest(
    string DestinationAccountNumber,
    decimal Amount,
    string Password);
