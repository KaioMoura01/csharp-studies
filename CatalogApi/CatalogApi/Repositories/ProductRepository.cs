using CatalogApi.Context;
using CatalogApi.DTOs;
using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Repositories;

public class ProductRepository(CatalogApiContext context):GenericRepository<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
    {
        return await Context.Set<Product>().Where(p => p.CategoryId == categoryId).ToListAsync();
    }
}