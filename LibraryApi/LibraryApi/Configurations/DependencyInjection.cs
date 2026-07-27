using System.Security.Claims;
using System.Text;
using LibraryApi.Models;
using LibraryApi.Repository;
using LibraryApi.Repository.Interfaces;
using LibraryApi.Services;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace LibraryApi.Configurations;

public static class DependencyInjection
{
    private static string GetArgument(WebApplicationBuilder builder, string path)
    {
        return builder.Configuration[path] 
            ?? throw new ArgumentException($"{path} is required.");
    }

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
    
    public static void ProgramServices(IServiceCollection services)
    {
        services.AddControllers();
        ConfigureScalar(services);
        services.AddScoped<IUser, UserRepository>();
        services.AddScoped<IBook, BookRepository>();
        services.AddScoped<ILoan, LoanRepository>();
        // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IToken, TokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }

    public static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
    {
        var secretKey = GetArgument(builder, "JWT:SecretKey");
        var issuer = GetArgument(builder, "JWT:Issuer");
        var audience = GetArgument(builder, "JWT:Audience");
        
       builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });
       builder.Services.AddAuthorization();
    }
}
