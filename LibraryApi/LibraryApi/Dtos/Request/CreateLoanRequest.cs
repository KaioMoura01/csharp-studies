namespace LibraryApi.Dtos.Request;

public record CreateLoanRequest(
    Guid UserId,
    Guid BookId
);
