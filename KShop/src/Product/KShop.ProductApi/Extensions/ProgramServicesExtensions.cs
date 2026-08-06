using KShop.ProductApi.Application;
using KShop.ProductApi.Application.Abstractions;
using KShop.ProductApi.Infrastructure;
using KShop.ProductApi.Security;

namespace KShop.ProductApi.Extensions;

public static class ProgramServicesExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureServices()
        {
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.ConfigureKeycloakAuthentication();
            builder.ConfigureFrontendCors();
        }
    }
}
