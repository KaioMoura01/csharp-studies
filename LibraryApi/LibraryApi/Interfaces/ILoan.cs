using LibraryApi.Models;

namespace LibraryApi.Interfaces;

public interface ILoan : IRepository<Loan>
{
    // Consultas que precisam de User e Book carregados (Include).
    Task<IEnumerable<Loan>> ListAllWithDetails(GenericParameters? parameters = null);
    Task<Loan?> GetWithDetails(Guid id);

    // Empréstimos com mais de 3 semanas e ainda não devolvidos.
    Task<IEnumerable<Loan>> GetOverdue();

    // Regras de negócio consultadas pelos controllers.
    Task<int> CountActiveByUser(Guid userId);
    Task<bool> HasActiveLoans(Guid bookId);
}
