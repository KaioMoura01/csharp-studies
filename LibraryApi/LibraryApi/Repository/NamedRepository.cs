using LibraryApi.Context;
using LibraryApi.Enums;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repository;

public class NamedRepository<T>(AppDbContext context) : Repository<T>(context)
    where T : class, INamed
{
    public override async Task<IEnumerable<T>> ListAll(GenericParameters? parameters = null)
    {
        var query = Context.Set<T>().AsNoTracking();
        if (parameters is null) return await query.ToListAsync();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search));
        }

        query = parameters.OrderByName == OrderEnum.Desc
            ? query.OrderByDescending(x => x.Name)
            : query.OrderBy(x => x.Name);

        return await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }
}