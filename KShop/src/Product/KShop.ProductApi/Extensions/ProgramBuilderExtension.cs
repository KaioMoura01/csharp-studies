using KShop.ProductApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace KShop.ProductApi.Extensions;

public static class ProgramBuilderExtension
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureApp()
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

            app.Run();
        }
    }
}
