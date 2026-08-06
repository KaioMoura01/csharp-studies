using System.Reflection;
using KShop.ProductApi.Application.Services;
using KShop.ProductApi.Application.Services.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace KShop.ProductApi.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
