using KShop.ProductApi.Infrastructure.Persistence;
using KShop.ProductApi.Domain.Models;
using KShop.ProductApi.Domain.Repositories;

namespace KShop.ProductApi.Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public IQueryable<Category> Query() => context.Categories;

    public async Task<Category?> GetByIdAsync(Guid id) => await context.Categories.FindAsync(id);

    public void Add(Category category) => context.Categories.Add(category);

    public void Remove(Category category) => context.Categories.Remove(category);
}
