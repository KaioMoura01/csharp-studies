using KShop.ProductApi.Infrastructure.Persistence;
using KShop.ProductApi.Domain.Models;
using KShop.ProductApi.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KShop.ProductApi.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public IQueryable<Product> Query() => context.Products;

    public Task<Product?> GetByIdAsync(Guid id) =>
        context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public void Add(Product product) => context.Products.Add(product);

    public void Remove(Product product) => context.Products.Remove(product);
}
