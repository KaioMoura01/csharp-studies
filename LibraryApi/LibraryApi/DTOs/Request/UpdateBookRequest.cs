namespace LibraryApi.DTOs.Request;

public record UpdateBookRequest(
    string Name,
    string Description,
    string Publisher,
    int YearOfPublication,
    int TotalQuantity
);
