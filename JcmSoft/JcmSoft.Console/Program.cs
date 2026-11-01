using JcmSoft.EFCore.Context;
using JcmSoft.EFCore.Seed;
using Microsoft.EntityFrameworkCore;

using var context = new AppDbContext();

var recreationControl = AskForRecreate();
CreateDataBase(context, recreationControl);

Console.WriteLine("Finish, press any key to close\n");
Console.ReadKey();

return;

bool AskForRecreate()
{
    while (true)
    {
        Console.Write("Regenerate the database? (y/n, default n): ");
        switch (Console.ReadLine()?.Trim().ToLowerInvariant())
        {
            case "y" or "yes": return true;
            case "n" or "no" or "" or null: return false;
            default: Console.WriteLine("Invalid input, try again"); break;
        }
    }
}

void CreateDataBase(AppDbContext appDb, bool shouldRecreate)
{
    if (!shouldRecreate) return;
    
    appDb.Database.EnsureDeleted();
    Console.WriteLine("Creating a database...\n");
    appDb.Database.EnsureCreated();

    Console.WriteLine("Seeding the database from CSV files...\n");
    DatabaseSeeder.Seed(appDb);
}
