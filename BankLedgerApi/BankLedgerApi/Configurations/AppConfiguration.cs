using Scalar.AspNetCore;

namespace BankLedgerApi.Configurations;

public static class AppConfiguration
{
    public static void ApplyConfiguration(WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors("ApiClients");

        app.UseRateLimiter();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}