using CatalogApi.DTOs;
using CatalogApi.Models;

namespace CatalogApi.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
}