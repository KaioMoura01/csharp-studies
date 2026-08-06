using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Infrastructure.Grpc;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        services.AddSingleton(new UserServiceGrpcOptions
        {
            Address = configuration["UserService:Address"] ?? "http://localhost:5229",
        });
        services.AddSingleton<IUserServiceClient, UserServiceGrpcClient>();

        return services;
    }
}
