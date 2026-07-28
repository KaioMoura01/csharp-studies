using LibraryApi.Configurations;
using LibraryApi.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

DependencyInjection.ProgramServices(builder.Services);
DependencyInjection.ConfigureJwtAuthentication(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();