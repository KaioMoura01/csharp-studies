using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoForSso.TodoApi.Application.Abstractions;
using TodoForSso.TodoApi.Application.Services;
using TodoForSso.TodoApi.Domain;
using TodoForSso.TodoApi.Infrastructure.Auth;
using TodoForSso.TodoApi.Infrastructure.Keycloak;
using TodoForSso.TodoApi.Infrastructure.Persistence;

namespace TodoForSso.TodoApi.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();

        services.Configure<KeycloakAdminOptions>(configuration.GetSection("Keycloak"));
        services.AddHttpClient<KeycloakAdminTokenProvider>((sp, client) =>
        {
            var adminUrl = configuration["Keycloak:AdminUrl"]
                ?? throw new InvalidOperationException("Keycloak:AdminUrl não configurado.");
            client.BaseAddress = new Uri(adminUrl);
        });
        services.AddScoped<IUserAdminService, KeycloakAdminService>();

        return services;
    }
}
