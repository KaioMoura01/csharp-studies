using KShop.ProductApi.Application.DTOs.Categories;

namespace KShop.ProductApi.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse?> GetByIdAsync(Guid id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<bool> DeleteAsync(Guid id);
}
