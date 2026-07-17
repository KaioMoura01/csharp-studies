using System.Globalization;

namespace SalesReportProject.Misc;

public static class Utils
{
    public static string ReadString(string label)
    {
        string value;
        do
        {
            Console.Write(label);
            value = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("Invalid input, try again.");
        } while (string.IsNullOrWhiteSpace(value));

        return value;
    }

    public static int ReadInt(string label)
    {
        int value;
        bool ok;
        do
        {
            var str = ReadString(label);
            ok = int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            if (!ok)
                Console.WriteLine("Invalid number, try again.");
        } while (!ok);

        return value;
    }

    public static double ReadDouble(string label)
    {
        double value;
        bool ok;
        do
        {
            var str = ReadString(label);
            ok = double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            if (!ok)
                Console.WriteLine("Invalid number, try again.");
        } while (!ok);

        return value;
    }

    public static DateTime ReadDate(string label, string dateFormat = "dd/MM/yyyy")
    {
        DateTime value;
        bool ok;
        do
        {
            var str = ReadString(label);
            ok = DateTime.TryParseExact(
                str,
                dateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);
            if (!ok)
                Console.WriteLine($"Invalid date (expected {dateFormat}), try again.");
        } while (!ok);

        return value;
    }
}