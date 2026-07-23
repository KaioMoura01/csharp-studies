using System.Net;
using CatalogApi.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace CatalogApi.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                        // TODO: para reabilitar o stacktrace, basta reabilitar a linha abaixo
                        // Trace = contextFeature.Error.StackTrace
                    }.ToString());
                }
            });
        });
    }
}