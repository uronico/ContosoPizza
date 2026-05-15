using System.Globalization;

namespace ContosoPizza.Services;

public static class SalesSummaryReport
{

    public static void GenerateSalesSummaryReport(string reportFilePath, IEnumerable<string> salesFilePaths)
    {
        ArgumentNullException.ThrowIfNull(reportFilePath);
        ArgumentNullException.ThrowIfNull(salesFilePaths);

        var details = new List<(string FileName, decimal TotalSales)>();
        decimal actualTotal = 0m;

        foreach (var salesFilePath in salesFilePaths)
        {
            if (string.IsNullOrWhiteSpace(salesFilePath))
            {
                continue;
            }

            var fileTotal = GetSalesTotalFromFile(salesFilePath);
            details.Add((Path.GetFileName(salesFilePath) ?? salesFilePath, fileTotal));
            actualTotal += fileTotal;
        }

        var lines = new List<string>
        {
            "Sales Summary",
            "----------------------------",
            $" Total Sales: {FormatCurrency(actualTotal)}",
            string.Empty,
            " Details:"
        };

        lines.AddRange(details.Select(d => $"  {d.FileName}: {FormatCurrency(d.TotalSales)}"));

        File.WriteAllLines(reportFilePath, lines);
    }

    public static decimal GetSalesTotalFromFile(string salesFilePath)
    {
        ArgumentNullException.ThrowIfNull(salesFilePath);

        if (!File.Exists(salesFilePath))
        {
            return 0m;
        }

        decimal total = 0m;
        foreach (var line in File.ReadLines(salesFilePath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var trimmed = line.Trim();
            if (decimal.TryParse(trimmed, NumberStyles.Currency | NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-US"), out var value) ||
                decimal.TryParse(trimmed.Replace("$", string.Empty).Replace(",", string.Empty), NumberStyles.Number | NumberStyles.AllowParentheses, CultureInfo.InvariantCulture, out value))
            {
                total += value;
            }
        }

        return total;
    }

    private static string FormatCurrency(decimal amount)
        => amount.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
}
