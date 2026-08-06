using Microsoft.EntityFrameworkCore;
using MinimalCatalog.Context;
using MinimalCatalog.Models;

namespace MinimalCatalog.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this WebApplication app)
    {
        app.MapPost("/categories", async (Category category, AppDbContext db) =>
        {
            db.Categories?.Add(category);
            await db.SaveChangesAsync();

            return Results.Created($"/categories/{category.CategoryId}", category);
        });

        app.MapGet("/categories", async (AppDbContext db) =>
        {
            var categories = await db.Categories?.ToListAsync()!;
    
            return Results.Ok(categories);
        });

        app.MapGet("/categories/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var category = await db.Categories!.FindAsync(id)!;

            return category is null ? Results.NotFound() : Results.Ok(category);
        });

        app.MapPut("/categories", async (Category category, AppDbContext db) =>
        {
            var target = await db.Categories!.FindAsync(category.CategoryId);

            if (target is null) return Results.NotFound();

            target.Description = category.Description;
            target.Name = category.Name;
            await db.SaveChangesAsync();
    
            return Results.Ok(category);
        });

        app.MapDelete("/categories/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var category = await db.Categories!.FindAsync(id);

            if (category is null) return Results.NotFound();

            db.Categories?.Remove(category);
            await db.SaveChangesAsync();

            return Results.Ok(category);
        });
    }
}