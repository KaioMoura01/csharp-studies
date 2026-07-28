using BankLedgerApi.DTOs.Statements;

namespace BankLedgerApi.Services.Interfaces;

public interface IStatementService
{
    Task<StatementResponse?> GetAsync(Guid accountId, StatementQuery query);
}
