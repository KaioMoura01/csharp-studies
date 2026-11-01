using Microsoft.AspNetCore.Http;
using TodoForSso.TodoApi.Application.Abstractions;
using TodoForSso.TodoApi.Domain;

namespace TodoForSso.TodoApi.Infrastructure.Auth;

public class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private System.Security.Claims.ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new DomainException("Nenhum usuário autenticado no contexto atual.");

    public string UserId => User.FindFirst("sub")?.Value
        ?? throw new DomainException("Token não possui a claim 'sub'.");

    public string? Username => User.FindFirst("preferred_username")?.Value;

    public bool IsAdmin => User.IsInRole("admin");
}
