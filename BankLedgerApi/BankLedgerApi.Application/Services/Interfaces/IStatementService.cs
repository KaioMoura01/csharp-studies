using BankLedgerApi.Application.DTOs.Statements;

namespace BankLedgerApi.Application.Services.Interfaces;

public interface IStatementService
{
    Task<StatementResponse?> GetAsync(Guid accountId, StatementQuery query);
}
