namespace BankProject.Entities;

public class SavingAccount(string ownerName, double amount, double interestRate):Account(ownerName, amount)
{
    public double InterestRate { get; set; } =  interestRate;

    protected override double BankTax { get; } = 0d;

    public void EarnInterest()
    {
        Amount += (InterestRate/100) * Amount;
    }
}