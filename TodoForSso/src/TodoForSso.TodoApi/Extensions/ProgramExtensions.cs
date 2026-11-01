using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoForSso.TodoApi.Application;
using TodoForSso.TodoApi.Infrastructure;
using TodoForSso.TodoApi.Infrastructure.Persistence;

namespace TodoForSso.TodoApi.Extensions;

public static class ProgramExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureServices()
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
            builder.Services.AddOpenApi();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            builder.ConfigureKeycloakAuthentication();
            builder.ConfigureFrontendCors();
        }
    }

    extension(WebApplicationBuilder builder)
    {
        public WebApplication ConfigureApp()
        {
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            }

            app.MapOpenApi();
            app.MapScalarApiReference();

            app.UseCors(CorsExtensions.FrontendPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
