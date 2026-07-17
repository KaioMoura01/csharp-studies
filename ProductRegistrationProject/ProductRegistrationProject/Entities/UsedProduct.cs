namespace ProductRegistrationProject.Entities;

public class UsedProduct(string name, double price, DateTime fabricationDate):
    Product(name, price)
{
    private DateTime FabricationDate { get; set; } = fabricationDate;

    public override string ToString()
    {
        return string.Concat(
            base.ToString(),
            " FabricationDate: ",
            FabricationDate.ToString("dd/MM/yyyy"));
    }
}