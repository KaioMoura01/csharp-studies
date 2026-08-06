namespace KShop.ProductApi.Application.DTOs.Categories;

public record CreateCategoryRequest(
    string? Name,
    string? Description);
