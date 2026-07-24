using CatalogApi.Context;

namespace CatalogApi.Repositories;

public class UnitOfWork(CatalogApiContext context) : IUnitOfWork
{
    private ICategoryRepository? _categoriesRepo;
    private IProductRepository? _productsRepo;

    public CatalogApiContext Context { get; } = context;

    public ICategoryRepository Categories => _categoriesRepo ??= new CategoryRepository(Context);
    
    public IProductRepository Products => _productsRepo ??= new ProductRepository(Context);
    
    public async Task Commit()
    {
        await Context.SaveChangesAsync();
    }
}