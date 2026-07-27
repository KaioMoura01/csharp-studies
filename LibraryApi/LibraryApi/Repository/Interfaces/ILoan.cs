using LibraryApi.Models;

namespace LibraryApi.Repository.Interfaces;

public interface ILoan : IRepository<Loan>
{
    Task<IEnumerable<Loan>> ListAllWithDetails(GenericParameters? parameters = null);
    Task<Loan?> GetWithDetails(Guid id);
    Task<IEnumerable<Loan>> GetOverdue();
    Task<int> CountActiveByUser(Guid userId);
    Task<bool> HasActiveLoans(Guid bookId);
}
