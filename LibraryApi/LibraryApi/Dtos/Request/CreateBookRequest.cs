namespace LibraryApi.Dtos.Request;

public record CreateBookRequest(
    string Name,
    string Description,
    string Publisher,
    int YearOfPublication,
    int TotalQuantity
);
