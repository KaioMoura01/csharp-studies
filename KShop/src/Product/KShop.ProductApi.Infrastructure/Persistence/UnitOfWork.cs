using KShop.ProductApi.Domain.Repositories;

namespace KShop.ProductApi.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext context, IProductRepository products, ICategoryRepository categories)
    : IUnitOfWork
{
    public IProductRepository Products { get; } = products;
    public ICategoryRepository Categories { get; } = categories;

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
