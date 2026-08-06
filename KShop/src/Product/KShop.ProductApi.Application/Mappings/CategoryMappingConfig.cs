using KShop.ProductApi.Application.DTOs.Categories;
using KShop.ProductApi.Domain.Models;
using Mapster;

namespace KShop.ProductApi.Application.Mappings;

public class CategoryMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryResponse>();

        config.NewConfig<CreateCategoryRequest, Category>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Products);

        config.NewConfig<UpdateCategoryRequest, Category>()
            .Ignore(dest => dest.Products);
    }
}
