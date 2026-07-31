using BankLedgerApi.Application.DTOs.Transfers;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface ITransferService
{
    Task<TransferResponse> ExecuteAsync(Guid sourceAccountId, CreateTransferRequest request);
}
