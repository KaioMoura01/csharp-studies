namespace LibraryApi.Interfaces;

public interface IUnitOfWork
{
    IBook Books { get; }
    IUser Users { get; }
    ILoan Loans { get; }
    Task Commit();
}