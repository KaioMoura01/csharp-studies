using System.Linq.Expressions;
using CatalogApi.Models;
using CatalogApi.Pagination;

namespace CatalogApi.Repositories;

public interface IGenericRepository<T> where T : class, INamed
{
    Task<IEnumerable<T>> ListAll(GenericParameters? parameters = null);
    Task<T?> Get(Expression<Func<T, bool>> predicate);
    T Create(T entity);
    T Update(T entity);
    bool Delete(T entity);
}