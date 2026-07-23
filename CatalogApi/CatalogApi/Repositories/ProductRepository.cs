using CatalogApi.Context;
using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Repositories;

public class ProductRepository(CatalogApiContext context):GenericRepository<Product>(context), IProductRepository
{
    public IEnumerable<Product> GetProductsByCategory(int categoryId)
    {
        return Context.Set<Product>().Where(p => p.CategoryId == categoryId);
    }
}