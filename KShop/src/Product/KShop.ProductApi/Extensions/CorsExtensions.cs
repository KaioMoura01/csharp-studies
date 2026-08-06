namespace KShop.ProductApi.Extensions;

public static class CorsExtensions
{
    public const string FrontendPolicy = "Frontend";

    extension(WebApplicationBuilder builder)
    {
        public void ConfigureFrontendCors()
        {
            var origin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:5173";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(FrontendPolicy, policy =>
                    policy.WithOrigins(origin)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
        }
    }
}
