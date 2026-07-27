using LibraryApi.Context;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Repository;

public class LoanRepository(AppDbContext context) : Repository<Loan>(context), ILoan
{
    private const int OverdueDays = 21;

    public async Task<IEnumerable<Loan>> ListAllWithDetails(GenericParameters? parameters = null)
    {
        var query = Details().AsNoTracking().OrderBy(l => l.LoanDate);
        if (parameters is null) return await query.ToListAsync();

        return await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<Loan?> GetWithDetails(Guid id)
        => await Details().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Loan>> GetOverdue()
    {
        var limit = DateTime.UtcNow.AddDays(-OverdueDays);
        return await Details()
            .AsNoTracking()
            .Where(l => l.ReturnDate == null && l.LoanDate <= limit)
            .OrderBy(l => l.LoanDate)
            .ToListAsync();
    }

    public async Task<int> CountActiveByUser(Guid userId)
        => await Context.Set<Loan>().CountAsync(l => l.UserId == userId && l.ReturnDate == null);

    public async Task<bool> HasActiveLoans(Guid bookId)
        => await Context.Set<Loan>().AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);

    private IQueryable<Loan> Details()
        => Context.Set<Loan>()
            .Include(l => l.Book)
            .Include(l => l.User);
}
