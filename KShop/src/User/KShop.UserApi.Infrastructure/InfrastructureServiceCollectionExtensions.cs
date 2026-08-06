using KShop.UserApi.Application.Abstractions;
using KShop.UserApi.Domain.Repositories;
using KShop.UserApi.Infrastructure.Keycloak;
using KShop.UserApi.Infrastructure.Persistence;
using KShop.UserApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KShop.UserApi.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton(new KeycloakAdminOptions
        {
            AdminUrl = configuration["Keycloak:AdminUrl"] ?? "http://localhost:8080",
            Realm = configuration["Keycloak:Realm"] ?? "kshop",
            AdminUsername = configuration["Keycloak:AdminUsername"] ?? "admin",
            AdminPassword = configuration["Keycloak:AdminPassword"] ?? "admin",
        });
        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

        return services;
    }
}
