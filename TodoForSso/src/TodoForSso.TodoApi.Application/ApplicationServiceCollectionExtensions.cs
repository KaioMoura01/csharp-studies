using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using TodoForSso.TodoApi.Application.Mapping;
using TodoForSso.TodoApi.Application.Services;

namespace TodoForSso.TodoApi.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfig).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
