using BankProject.Entities;
using BankProject.Misc;

Account? account = null;

Console.WriteLine("=== Welcome to BankProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Create a saving account");
    Console.WriteLine("2 - Create a business account");
    Console.WriteLine("3 - Deposit");
    Console.WriteLine("4 - Withdraw");
    Console.WriteLine("5 - Check account");
    Console.WriteLine("6 - Earn interest (saving account)");
    Console.WriteLine("7 - Request a loan (business account)");
    Console.WriteLine("8 - Check debt (business account)");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadConsoleInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var ownerName = Utils.ReadConsoleString("Owner's name: ");
                var initialFunds = Utils.ReadConsoleDouble("Initial funds: ");
                var interestRate = Utils.ReadConsoleDouble("Interest rate (%): ");

                account = new SavingAccount(ownerName, initialFunds, interestRate);
                Console.WriteLine("Saving account created successfully!");
                break;
            }
            case 2:
            {
                var ownerName = Utils.ReadConsoleString("Owner's name: ");
                var initialFunds = Utils.ReadConsoleDouble("Initial funds: ");
                var loanLimit = Utils.ReadConsoleDouble("Loan limit: ");

                account = new BusinessAccount(ownerName, initialFunds, loanLimit);
                Console.WriteLine("Business account created successfully!");
                break;
            }
            case 3:
            {
                var amount = Utils.ReadConsoleDouble("Deposit amount: ");
                RequireAccount().Deposit(amount);
                break;
            }
            case 4:
            {
                var amount = Utils.ReadConsoleDouble("Withdraw amount: ");
                RequireAccount().Withdraw(amount);
                break;
            }
            case 5:
                RequireAccount().GetBalance();
                break;
            case 6:
            {
                if (RequireAccount() is SavingAccount saving)
                {
                    saving.EarnInterest();
                    saving.GetBalance();
                }
                else
                {
                    Console.WriteLine("Earning interest is only available for saving accounts.");
                }
                break;
            }
            case 7:
            {
                if (RequireAccount() is BusinessAccount business)
                {
                    var amount = Utils.ReadConsoleDouble("Loan amount: ");
                    business.Loan(amount);
                }
                else
                {
                    Console.WriteLine("Loans are only available for business accounts.");
                }
                break;
            }
            case 8:
            {
                if (RequireAccount() is BusinessAccount business)
                {
                    business.GetDebt();
                }
                else
                {
                    Console.WriteLine("Debt is only available for business accounts.");
                }
                break;
            }
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (ArgumentException e)
    {
        Console.WriteLine($"Operation error: {e.Message}");
    }
    catch (InvalidOperationException e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}

Account RequireAccount() =>
    account ?? throw new InvalidOperationException("No account created yet. Create an account first (options 1 or 2).");
