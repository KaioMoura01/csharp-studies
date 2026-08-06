using KShop.ProductApi.Application.Abstractions;
using KShop.ProductApi.Domain.Repositories;
using KShop.ProductApi.Infrastructure.Grpc;
using KShop.ProductApi.Infrastructure.Persistence;
using KShop.ProductApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KShop.ProductApi.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton(new UserServiceGrpcOptions
        {
            Address = configuration["UserApi:GrpcAddress"] ?? "http://localhost:8081",
        });
        services.AddSingleton<IUserServiceClient, UserServiceGrpcClient>();

        return services;
    }
}
