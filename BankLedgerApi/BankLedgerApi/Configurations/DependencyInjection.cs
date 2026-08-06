using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using BankLedgerApi.Application.Multitenancy;
using BankLedgerApi.Application.Services;
using BankLedgerApi.Application.Services.Interfaces;
using BankLedgerApi.Infrastructure;
using BankLedgerApi.Infrastructure.Security;
using BankLedgerApi.Multitenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace BankLedgerApi.Configurations;

public static class DependencyInjection
{
    private static void ConfigureScalar(IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste the JWT token here (without the 'Bearer ' prefix)."
                };

                return Task.CompletedTask;
            });
        });
    }

    private static void InjectServices(IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IStatementService, StatementService>();
        services.AddScoped<IReversalService, ReversalService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITenantService, TenantService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var settings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
                };
            });

        services.AddAuthorization();
    }

    private static void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ApiClients", policy =>
                policy.WithOrigins("https://apirequest.io","http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
    }

    private static void ConfigureRateLimiting(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("RateLimiting").Get<RateLimitSettings>()!;

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var path = context.Request.Path;
                if (path.StartsWithSegments("/scalar") || path.StartsWithSegments("/openapi"))
                    return RateLimitPartition.GetNoLimiter("docs");

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                        QueueLimit = settings.QueueLimit
                    });
            });
        });
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddInfrastructure(builder.Configuration);
        InjectServices(services);
        ConfigureAuthentication(builder);
        ConfigureCors(services);
        ConfigureRateLimiting(builder);
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        ConfigureScalar(services);
    }
}
