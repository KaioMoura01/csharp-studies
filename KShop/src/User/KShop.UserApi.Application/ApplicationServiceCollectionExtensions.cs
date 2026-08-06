using System.Reflection;
using KShop.UserApi.Application.Services;
using KShop.UserApi.Application.Services.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace KShop.UserApi.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
