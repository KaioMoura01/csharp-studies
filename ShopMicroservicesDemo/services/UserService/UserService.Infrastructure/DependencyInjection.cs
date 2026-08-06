using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        return services;
    }
}
