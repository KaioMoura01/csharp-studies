namespace LibraryApi.DTOs.Response;

public record BookResponse(
    Guid Id,
    string Name,
    string Description,
    string Publisher,
    int YearOfPublication,
    int TotalQuantity,
    int Stock
);
