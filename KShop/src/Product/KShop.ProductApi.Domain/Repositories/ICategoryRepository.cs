using KShop.ProductApi.Domain.Models;

namespace KShop.ProductApi.Domain.Repositories;

public interface ICategoryRepository
{
    IQueryable<Category> Query();
    Task<Category?> GetByIdAsync(Guid id);
    void Add(Category category);
    void Remove(Category category);
}
