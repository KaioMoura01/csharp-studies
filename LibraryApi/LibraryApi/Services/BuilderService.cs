using LibraryApi.Interfaces;
using LibraryApi.Models;
using LibraryApi.Repository;
using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Services;

public static class BuilderService
{
    public static void ProgramServices(IServiceCollection services)
    {
        services.AddScoped<IUser, UserRepository>();
        services.AddScoped<IBook, BookRepository>();
        services.AddScoped<ILoan, LoanRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Hashing de senha (implementação do ASP.NET Core Identity).
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}
