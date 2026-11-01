using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

namespace TodoForSso.TodoApi.Extensions;

public static class AuthenticationExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureKeycloakAuthentication()
        {
            builder.Services.AddKeycloakWebApiAuthentication(
                builder.Configuration,
                options =>
                {
                    options.RequireHttpsMetadata = false; // apenas ambiente dev
                    options.MapInboundClaims = false; // preserva "sub"/"preferred_username" como vêm do Keycloak

                    // AuthServerUrl aponta para o hostname interno do Docker (usado para buscar as
                    // chaves JWKS). Os tokens emitidos ao navegador trazem o hostname externo no "iss",
                    // então validamos o issuer separadamente via Keycloak:Issuer.
                    var issuer = builder.Configuration["Keycloak:Issuer"];
                    if (!string.IsNullOrWhiteSpace(issuer))
                    {
                        options.TokenValidationParameters.ValidIssuer = issuer;
                    }
                });

            builder.Services.AddKeycloakAuthorization(options =>
                options.EnableRolesMapping = RolesClaimTransformationSource.Realm);

            builder.Services.AddAuthorization();
        }
    }
}
