namespace BankProject.Entities;

public class BusinessAccount(string ownerName, double initialAmount, double loanLimit):Account(ownerName, initialAmount)
{
   private double LoanLimit { get; set; } = loanLimit;
   private double Debt { get; set; } = 0;
   protected override double BankTax { get; } = 10d;

   public void Loan(double amount)
   {
      ValidateValue(amount);

      if (amount > LoanLimit)
      {
         throw new ArgumentException($"Loan limit must be less than {LoanLimit}");
      }
      
      Amount += amount;
      Debt += amount + BankTax;
      LoanLimit -= amount;
      
      GetBalance();
   }
   
   public void GetDebt()
   {
      Console.WriteLine($"Debt: {Debt}");
   }
}