using KShop.ProductApi.Application.Abstractions;

namespace KShop.ProductApi.Security;

public class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public string? UserSub => httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
}
