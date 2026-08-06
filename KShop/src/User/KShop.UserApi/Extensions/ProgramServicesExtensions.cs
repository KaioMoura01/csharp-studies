using KShop.UserApi.Application;
using KShop.UserApi.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KShop.UserApi.Extensions;

public static class ProgramServicesExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureServices()
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(8080, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
                options.ListenAnyIP(8081, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddGrpc();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.ConfigureKeycloakAuthentication();
            builder.ConfigureFrontendCors();
        }
    }
}
