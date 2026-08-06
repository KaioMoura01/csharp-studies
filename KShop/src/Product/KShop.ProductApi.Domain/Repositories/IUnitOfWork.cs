namespace KShop.ProductApi.Domain.Repositories;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync();
}
