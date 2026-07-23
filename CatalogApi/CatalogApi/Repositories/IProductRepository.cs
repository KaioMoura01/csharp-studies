using CatalogApi.Models;

namespace CatalogApi.Repositories;

public interface IProductRepository:IGenericRepository<Product>
{
    IEnumerable<Product> GetProductsByCategory(int categoryId);
}