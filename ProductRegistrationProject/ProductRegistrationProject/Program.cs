using ProductRegistrationProject.Entities;
using ProductRegistrationProject.Misc;

var products = new List<Product>();

Console.WriteLine("=== Welcome to ProductRegistrationProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Add national product");
    Console.WriteLine("2 - Add imported product");
    Console.WriteLine("3 - Add used product");
    Console.WriteLine("4 - List products");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var name = Utils.ReadString("Name: ");
                var price = Utils.ReadPositiveDouble("Price: ");

                products.Add(new NationalProduct(name, price));
                Console.WriteLine("National product added successfully!");
                break;
            }
            case 2:
            {
                var name = Utils.ReadString("Name: ");
                var price = Utils.ReadPositiveDouble("Price: ");
                var importDuty = Utils.ReadPositiveDouble("Import duty: ");

                products.Add(new ImportedProduct(name, price, importDuty));
                Console.WriteLine("Imported product added successfully!");
                break;
            }
            case 3:
            {
                var name = Utils.ReadString("Name: ");
                var price = Utils.ReadPositiveDouble("Price: ");
                var fabricationDate = Utils.ReadDate("Fabrication date (dd/MM/yyyy): ");

                products.Add(new UsedProduct(name, price, fabricationDate));
                Console.WriteLine("Used product added successfully!");
                break;
            }
            case 4:
                if (products.Count == 0)
                {
                    Console.WriteLine("No products registered yet.");
                    break;
                }

                foreach (var product in products)
                {
                    Console.WriteLine(product);
                }
                break;
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}
