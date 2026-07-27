using LibraryApi.Context;
using LibraryApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
BuilderService.ProgramServices(builder.Services);

var app = builder.Build();


app.MapOpenApi();

app.MapScalarApiReference();

app.UseAuthorization();

app.MapControllers();

app.Run();