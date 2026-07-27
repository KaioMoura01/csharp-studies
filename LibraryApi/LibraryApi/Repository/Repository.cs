using System.Linq.Expressions;
using LibraryApi.Context;
using LibraryApi.Enums;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repository;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;

    public virtual async Task<IEnumerable<T>> ListAll(GenericParameters? parameters = null)
    {
        var query = Context.Set<T>().AsNoTracking();
        if (parameters is null) return await query.ToListAsync();

        return await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<T?> Get(Expression<Func<T, bool>> predicate)
        => await Context.Set<T>().FirstOrDefaultAsync(predicate);

    public T Create(T entity) { Context.Set<T>().Add(entity); return entity; }
    
    public T Update(T entity) { Context.Set<T>().Update(entity); return entity; }
    
    public bool Delete(T entity) { Context.Set<T>().Remove(entity); return true; }
}