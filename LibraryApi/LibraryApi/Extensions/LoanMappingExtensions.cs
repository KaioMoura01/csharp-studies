using LibraryApi.Dtos.Response;
using LibraryApi.Models;

namespace LibraryApi.Extensions;

public static class LoanMappingExtensions
{
    public static LoanResponse ToResponse(this Loan loan) => new(
        loan.Id,
        loan.LoanDate,
        loan.LatestReturnDate,
        loan.ReturnDate,
        loan.Returned,
        loan.BookId,
        loan.Book.Name,
        loan.UserId,
        loan.User.Name);

    public static IEnumerable<LoanResponse> ToResponse(this IEnumerable<Loan> loans)
        => loans.Select(l => l.ToResponse());
}
