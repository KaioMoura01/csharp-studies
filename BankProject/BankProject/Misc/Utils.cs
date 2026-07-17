namespace BankProject.Misc;

using System.Globalization;

public static class Utils
{
    public static string FormatMoney(double value)
    {
        return value.ToString("F2", CultureInfo.InvariantCulture);
    }

    public static DateTime ReadConsoleDate(string label, string dateFormat = "dd/MM/yyyy HH:mm")
    {
        DateTime value;
        bool success;
        do
        {
            var str = ReadConsoleString(label);
            success = DateTime.TryParseExact(
                str,
                dateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);

            if (!success)
                Console.WriteLine("Invalid input, try again.");
        } while (!success);

        return value;
    }
    
    public static string ReadConsoleString(string label)
    {
        string value;
        do
        {
            Console.Write(label);
            value = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(value))
                Console.WriteLine("Invalid input, try again.");
        } while (string.IsNullOrEmpty(value));

        return value;

    }

    public static double ReadConsoleDouble(string label)
    {
        double value;
        bool success;
        do
        {
            var str = ReadConsoleString(label);
            success = double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            if (!success)
                Console.WriteLine("Invalid input, try again.");
        } while (!success);

        return value;
    }
    
    public static int ReadConsoleInt(string label)
    {
        int value;
        bool success;
        do
        {
            var str = ReadConsoleString(label);
            success = int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            if (!success)
                Console.WriteLine("Invalid input, try again.");
        } while (!success);

        return value;
    }
}
