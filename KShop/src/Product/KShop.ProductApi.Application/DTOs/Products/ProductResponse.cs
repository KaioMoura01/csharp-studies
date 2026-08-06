namespace KShop.ProductApi.Application.DTOs.Products;

public record ProductResponse(
    Guid Id,
    string? Name,
    string? Description,
    string? ImageUrl,
    decimal Price,
    long Stock,
    Guid CategoryId,
    string? CategoryName);
