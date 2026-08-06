namespace KShop.ProductApi.Application.Abstractions;

public interface ICurrentUserContext
{
    string? UserSub { get; }
}
