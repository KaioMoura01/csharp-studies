namespace LibraryApi.DTOs.Request;

public record CreateLoanRequest(
    Guid UserId,
    Guid BookId
);
