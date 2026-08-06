using BankLedgerApi.Application.Multitenancy;
using BankLedgerApi.Multitenancy;
using Scalar.AspNetCore;

namespace BankLedgerApi.Configurations;

public static class AppConfiguration
{
    private static readonly string[] ExemptPathPrefixes = ["/health", "/openapi", "/scalar"];

    public static void ApplyConfiguration(WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseCors("ApiClients");

        app.UseRateLimiter();

        app.Use(async (context, next) =>
        {
            var isExempt = ExemptPathPrefixes.Any(prefix => context.Request.Path.StartsWithSegments(prefix))
                || (context.Request.Path.StartsWithSegments("/tenants") && HttpMethods.IsPost(context.Request.Method));

            if (!isExempt)
            {
                var tenantContext = context.RequestServices.GetRequiredService<ITenantContext>();
                if (tenantContext.TenantId is null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = $"Missing or invalid '{HttpTenantContext.HeaderName}' header."
                    });
                    return;
                }
            }

            await next();
        });

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}