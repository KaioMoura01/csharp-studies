namespace LibraryApi.DTOs.Response;

public record LoanResponse(
    Guid Id,
    DateTime LoanDate,
    DateTime LatestReturnDate,
    DateTime? ReturnDate,
    bool Returned,
    Guid BookId,
    string BookName,
    Guid UserId,
    string UserName
);
