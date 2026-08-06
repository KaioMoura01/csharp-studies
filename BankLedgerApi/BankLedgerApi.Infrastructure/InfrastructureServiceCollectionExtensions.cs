using BankLedgerApi.Application.Security;
using BankLedgerApi.Domain.Repositories;
using BankLedgerApi.Infrastructure.Persistence;
using BankLedgerApi.Infrastructure.Repositories;
using BankLedgerApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankLedgerApi.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
