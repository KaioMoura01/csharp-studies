using System.Linq.Expressions;
using LibraryApi.Models;

namespace LibraryApi.Repository.Interfaces;

public interface IRepository<T> where T: class
{
    Task<IEnumerable<T>> ListAll(GenericParameters? parameters = null);
    Task<T?> Get(Expression<Func<T, bool>> predicate);
    T Create(T entity);
    T Update(T entity);
    bool Delete(T entity);
}