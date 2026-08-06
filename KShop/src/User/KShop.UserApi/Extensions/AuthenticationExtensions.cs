using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KShop.UserApi.Extensions;

public static class AuthenticationExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureKeycloakAuthentication()
        {
            var authority = builder.Configuration["Keycloak:Authority"];
            var issuer = builder.Configuration["Keycloak:Issuer"] ?? authority;
            var audience = builder.Configuration["Keycloak:Audience"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = audience;
                    options.RequireHttpsMetadata = false; // apenas ambiente dev
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                    };
                });

            builder.Services.AddAuthorization();
        }
    }
}
