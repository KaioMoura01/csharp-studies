using BankLedgerApi.DTOs.Transfers;

namespace BankLedgerApi.Services.Interfaces;

public interface ITransferService
{
    Task<TransferResponse> ExecuteAsync(Guid sourceAccountId, CreateTransferRequest request);
}
