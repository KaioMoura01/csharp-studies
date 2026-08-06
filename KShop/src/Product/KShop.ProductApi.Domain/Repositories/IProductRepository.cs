using KShop.ProductApi.Domain.Models;

namespace KShop.ProductApi.Domain.Repositories;

public interface IProductRepository
{
    IQueryable<Product> Query();
    Task<Product?> GetByIdAsync(Guid id);
    void Add(Product product);
    void Remove(Product product);
}
