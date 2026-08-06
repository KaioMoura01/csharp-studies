using KShop.ProductApi.Application.DTOs.Categories;
using KShop.ProductApi.Domain.Models;
using KShop.ProductApi.Domain.Repositories;
using KShop.ProductApi.Application.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace KShop.ProductApi.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        return await unitOfWork.Categories.Query()
            .ProjectToType<CategoryResponse>()
            .ToListAsync();
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id);
        return category?.Adapt<CategoryResponse>();
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = request.Adapt<Category>();

        unitOfWork.Categories.Add(category);
        await unitOfWork.SaveChangesAsync();

        return category.Adapt<CategoryResponse>();
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id);
        if (category is null) return null;

        request.Adapt(category);
        await unitOfWork.SaveChangesAsync();

        return category.Adapt<CategoryResponse>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id);
        if (category is null) return false;

        unitOfWork.Categories.Remove(category);
        await unitOfWork.SaveChangesAsync();
        return true;
    }
}
