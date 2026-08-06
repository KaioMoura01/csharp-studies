using Microsoft.EntityFrameworkCore;
using MinimalCatalog.Context;
using MinimalCatalog.Endpoints;
using MinimalCatalog.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddOpenApi();
// builder.Services.AddMapster();

var app = builder.Build();

//TODO implementar os DTO e Mappings
app.MapCategoriesEndpoints();
app.MapProductsEndpoints();


app.MapOpenApi();
app.MapScalarApiReference();

app.Run();