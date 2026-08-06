using KShop.ProductApi.Application.Abstractions;
using KShop.ProductApi.Application.DTOs.Products;
using KShop.ProductApi.Domain.Models;
using KShop.ProductApi.Domain.Repositories;
using KShop.ProductApi.Application.Services.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace KShop.ProductApi.Application.Services;

public class ProductService(
    IUnitOfWork unitOfWork,
    IUserServiceClient userServiceClient,
    ICurrentUserContext currentUserContext) : IProductService
{
    public async Task<List<ProductResponse>> GetAllAsync()
    {
        return await unitOfWork.Products.Query()
            .Include(p => p.Category)
            .ProjectToType<ProductResponse>()
            .ToListAsync();
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);
        return product?.Adapt<ProductResponse>();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var sub = currentUserContext.UserSub;
        var user = sub is null ? null : await userServiceClient.GetUserBySubAsync(sub, CancellationToken.None);
        if (user is null)
        {
            throw new UnrecognizedUserException(sub);
        }

        var product = request.Adapt<Product>();

        unitOfWork.Products.Add(product);
        await unitOfWork.SaveChangesAsync();

        var created = await unitOfWork.Products.GetByIdAsync(product.Id);
        return created!.Adapt<ProductResponse>();
    }

    public async Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);
        if (product is null) return null;

        request.Adapt(product);
        await unitOfWork.SaveChangesAsync();

        return product.Adapt<ProductResponse>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await unitOfWork.Products.GetByIdAsync(id);
        if (product is null) return false;

        unitOfWork.Products.Remove(product);
        await unitOfWork.SaveChangesAsync();
        return true;
    }
}
