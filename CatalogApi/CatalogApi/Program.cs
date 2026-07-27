using System.Text.Json.Serialization;
using CatalogApi.Context;
using CatalogApi.Extensions;
using CatalogApi.Repositories;
using CatalogApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
const string originsAllowed = "_originsAllowed";

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => 
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

builder.Services.AddDbContext<CatalogApiContext>(options =>
    options.UseNpgsql(connectionString));

BuilderAuthenticationService.ConfigureParameters(builder);
//TODO: remover essa linha para remover a instância do CORS, bem como a linha 11
BuilderAuthenticationService.AddCorsConfig(builder, originsAllowed);

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// aplica migrations pendentes automaticamente, tem que ficar depois da "build"
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<CatalogApiContext>();
//     db.Database.Migrate();
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.ConfigureExceptionHandler();

//TODO: remover essa linha para desabilitar o CORS
app.UseCors(originsAllowed);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();