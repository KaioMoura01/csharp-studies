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
    
    public async Task<IEnumerable<T>> ListAll(GenericParameters? parameters = null)
    {
        var query = Context.Set<T>().AsNoTracking();

        if (parameters is null)
            return await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(search));
        }

        query = parameters.OrderByName == OrderEnum.Desc
            ? query.OrderByDescending(x => x.Name)
            : query.OrderBy(x => x.Name);

        return await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<T?> Get(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().FirstOrDefaultAsync(predicate);
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