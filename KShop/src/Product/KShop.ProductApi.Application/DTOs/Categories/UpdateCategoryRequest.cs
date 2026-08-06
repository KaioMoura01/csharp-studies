namespace KShop.ProductApi.Application.DTOs.Categories;

public record UpdateCategoryRequest(
    Guid Id,
    string? Name,
    string? Description);
