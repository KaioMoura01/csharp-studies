using LibraryApi.Context;
using LibraryApi.Repository.Interfaces;

namespace LibraryApi.Repository;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IBook? _book;
    private IUser? _user;
    private ILoan? _loan;
    
    public AppDbContext Context { get; } = context;

    public IBook Books => _book ??= new BookRepository(Context);
    public IUser Users => _user ??= new UserRepository(Context);
    public ILoan Loans => _loan ??= new LoanRepository(Context);
    
    public async Task Commit()
    {
        await Context.SaveChangesAsync();
    }
}