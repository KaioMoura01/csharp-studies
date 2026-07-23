namespace CatalogApi.Repositories;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    void Commit();
}