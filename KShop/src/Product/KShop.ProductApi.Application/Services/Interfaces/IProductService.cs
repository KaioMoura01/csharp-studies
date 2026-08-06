using KShop.ProductApi.Application.DTOs.Products;

namespace KShop.ProductApi.Application.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(Guid id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteAsync(Guid id);
}
