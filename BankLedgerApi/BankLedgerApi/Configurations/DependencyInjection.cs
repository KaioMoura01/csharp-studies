using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using BankLedgerApi.Context;
using BankLedgerApi.Models;
using BankLedgerApi.Services;
using BankLedgerApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    private static void InjectDb(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    }

    private static void InjectServices(IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<Account>, PasswordHasher<Account>>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IStatementService, StatementService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
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
                policy.WithOrigins("https://apirequest.io")
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
        InjectDb(builder);
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
