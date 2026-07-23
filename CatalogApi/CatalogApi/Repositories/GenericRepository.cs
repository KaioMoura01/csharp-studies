using System.Linq.Expressions;
using CatalogApi.Context;
using CatalogApi.Enums;
using CatalogApi.Models;
using CatalogApi.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Repositories;

public class GenericRepository<T>(CatalogApiContext context):IGenericRepository<T> where T : class, INamed
{
    protected readonly CatalogApiContext Context = context;
    
    public IEnumerable<T> ListAll(GenericParameters? parameters = null)
    {
        var query = Context.Set<T>().AsNoTracking();

        if (parameters is null)
            return query.ToList();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
            query = query.Where(x => x.Name != null &&
                                     x.Name.ToLower().Contains(parameters.Search.ToLower()));

        query = parameters.OrderByName == OrderEnum.Desc
            ? query.OrderByDescending(x => x.Name)
            : query.OrderBy(x => x.Name);

        return query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return  Context.Set<T>().FirstOrDefault(predicate);
    }

    public T Create(T entity)
    {
        Context.Set<T>().Add(entity);
        return entity;
    }

    public T Update(T entity)
    {
        Context.Set<T>().Update(entity);
        return entity;
    }

    public bool Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
        return true;
    }
}