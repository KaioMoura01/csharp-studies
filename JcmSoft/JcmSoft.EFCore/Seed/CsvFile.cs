using System.Globalization;
using System.Text;

namespace JcmSoft.EFCore.Seed;

public static class CsvFile
{
    public static IReadOnlyList<CsvRow> Read(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Arquivo de seed não encontrado: {path}", path);

        var lines = File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (lines.Count == 0)
            throw new InvalidDataException($"Arquivo de seed vazio: {path}");

        var headers = SplitLine(lines[0]);
        var rows = new List<CsvRow>(lines.Count - 1);

        for (var i = 1; i < lines.Count; i++)
        {
            var values = SplitLine(lines[i]);
            if (values.Count != headers.Count)
            {
                throw new InvalidDataException(
                    $"{Path.GetFileName(path)} linha {i + 1}: esperado {headers.Count} colunas, encontrado {values.Count}.");
            }

            var fields = new Dictionary<string, string>(headers.Count, StringComparer.OrdinalIgnoreCase);
            for (var c = 0; c < headers.Count; c++)
                fields[headers[c]] = values[c];

            rows.Add(new CsvRow(fields, Path.GetFileName(path), i + 1));
        }

        return rows;
    }

    private static List<string> SplitLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (insideQuotes)
            {
                if (c != '"')
                {
                    current.Append(c);
                }
                else if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = false;
                }
            }
            else switch (c)
            {
                case '"':
                    insideQuotes = true;
                    break;
                case ',':
                    fields.Add(current.ToString().Trim());
                    current.Clear();
                    break;
                default:
                    current.Append(c);
                    break;
            }
        }

        fields.Add(current.ToString().Trim());
        return fields;
    }
}

public sealed class CsvRow(IReadOnlyDictionary<string, string> fields, string fileName, int lineNumber)
{
    public string GetString(string column)
    {
        var value = GetOptionalString(column);
        return value ?? throw new InvalidDataException($"{fileName} linha {lineNumber}: coluna '{column}' é obrigatória.");
    }

    public string? GetOptionalString(string column)
    {
        if (!fields.TryGetValue(column, out var value))
            throw new InvalidDataException($"{fileName} linha {lineNumber}: coluna '{column}' não existe.");
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public decimal GetDecimal(string column) =>
        decimal.Parse(GetString(column), NumberStyles.Number, CultureInfo.InvariantCulture);

    public DateOnly GetDate(string column) =>
        DateOnly.ParseExact(GetString(column), "yyyy-MM-dd", CultureInfo.InvariantCulture);

    public TEnum GetEnum<TEnum>(string column) where TEnum : struct, Enum =>
        Enum.Parse<TEnum>(GetString(column), ignoreCase: true);
}
