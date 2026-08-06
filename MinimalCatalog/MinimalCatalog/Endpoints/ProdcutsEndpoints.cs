using Microsoft.EntityFrameworkCore;
using MinimalCatalog.Context;
using MinimalCatalog.Models;

namespace MinimalCatalog.Endpoints;

public static class ProdcutsEndpoints
{
    public static void MapProductsEndpoints(this WebApplication app)
    {
        app.MapPost("/products", async (Product product, AppDbContext db) =>
        {
            db.Products?.Add(product);
            await db.SaveChangesAsync();

            return Results.Created($"/products/{product.ProductId}", product);
        });

        app.MapGet("/products", async (AppDbContext db) =>
        {
            var products = await db.Products?.ToListAsync()!;
    
            return Results.Ok(products);
        });

        app.MapGet("/products/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var product = await db.Products!.FindAsync(id)!;

            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        app.MapPut("/products", async (Product product, AppDbContext db) =>
        {
            var target = await db.Products!.FindAsync(product.ProductId);

            if (target is null) return Results.NotFound();

            //TODO implementar as outras características a serem mudadas no PUT
            target.Description = product.Description;
            target.Name = product.Name;
            await db.SaveChangesAsync();
    
            return Results.Ok(product);
        });

        app.MapDelete("/products/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var product = await db.Products!.FindAsync(id);

            if (product is null) return Results.NotFound();

            db.Products?.Remove(product);
            await db.SaveChangesAsync();

            return Results.Ok(product);
        });
    }
}