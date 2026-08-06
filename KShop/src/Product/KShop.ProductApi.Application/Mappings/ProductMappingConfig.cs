using KShop.ProductApi.Application.DTOs.Products;
using KShop.ProductApi.Domain.Models;
using Mapster;

namespace KShop.ProductApi.Application.Mappings;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null);

        config.NewConfig<CreateProductRequest, Product>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Category);

        config.NewConfig<UpdateProductRequest, Product>()
            .Ignore(dest => dest.Category);
    }
}
