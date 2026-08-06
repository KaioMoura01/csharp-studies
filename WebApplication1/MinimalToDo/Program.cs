using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(
    opt => opt.UseInMemoryDatabase("TasksDb"));

var app = builder.Build();

app.MapGet("/", () => "Hello world!");

app.MapGet("/tasks", 
    async (AppDbContext db) => await db.Tasks.ToListAsync());

app.MapGet("/tasks/{id:guid}", async (Guid id,AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

app.MapGet("/tasks/concluded",
    async (AppDbContext db) => await db.Tasks.Where(t => t.Concluded).ToListAsync());

app.MapPost("/tasks", async (Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapPut("/tasks/{id:guid}", async (Guid id, Task task, AppDbContext db) =>
{
    var target = await db.Tasks.FindAsync(id);

    if (target is null) return Results.NotFound();

    target.Name = task.Name;
    target.Concluded = task.Concluded;

    await db.SaveChangesAsync();
    return Results.Ok(target);
});

app.MapDelete("/tasks/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    
    return Results.Ok(task);
});

app.MapOpenApi();
app.MapScalarApiReference();

app.Run();

internal class Task
{
    [BindNever]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool Concluded { get; set; }
}

internal class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
}