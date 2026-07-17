using System.Globalization;
using System.Text;

namespace BankProject.Entities;

public abstract class Account(string ownerName, double amount)
{
    protected string OwnerAccount { get; set; } = ownerName;
    protected double Amount { get; set; } = amount;

    protected virtual double BankTax => 5d;

    public void ValidateValue(double amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero");
        }
    }

    public void Deposit(double amount)
    {
        ValidateValue(amount);
        
        Amount += amount;
        
        GetBalance();
    }

    public void Withdraw(double amount)
    {
        ValidateValue(amount);
        if (amount + BankTax > Amount)
        {
            throw new ArgumentException("Insufficient funds");
        }
        
        Amount -= (amount + BankTax);
        
        GetBalance();
    }

    public void GetBalance()
    {
        Console.WriteLine(this.ToString());
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(
            string.Concat(
                "A conta de: ", 
                OwnerAccount,
                " possui ", 
                Amount.ToString("F2", CultureInfo.InvariantCulture)
                ));
        
        return sb.ToString();
    }
}