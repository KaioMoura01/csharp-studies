namespace BankLedgerApi.Application.Multitenancy;

public interface ITenantContext
{
    Guid? TenantId { get; }
}
