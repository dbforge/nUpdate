using System.Globalization;

namespace nUpdate.Ui;

/// <summary>Formats byte counts the way file managers do ("1.5 MB").</summary>
internal static class ByteSizeFormatter
{
    private static readonly string[] Units = ["bytes", "KB", "MB", "GB", "TB", "PB"];

    public static string Format(long bytes, CultureInfo? culture = null)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        culture ??= CultureInfo.InvariantCulture;
        if (bytes < 1024)
            return $"{bytes.ToString(culture)} {Units[0]}";

        double value = bytes;
        var unit = 0;
        while (value >= 1024 && unit < Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        var digits = value >= 100 ? 0 : value >= 10 ? 1 : 2;
        return $"{value.ToString("F" + digits.ToString(CultureInfo.InvariantCulture), culture)} {Units[unit]}";
    }
}
