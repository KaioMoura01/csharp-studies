using BankLedgerApi.Application.Multitenancy;

namespace BankLedgerApi.Multitenancy;

public class HttpTenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    public const string HeaderName = "X-Tenant-Id";

    public Guid? TenantId
    {
        get
        {
            var header = httpContextAccessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();
            return Guid.TryParse(header, out var id) ? id : null;
        }
    }
}
