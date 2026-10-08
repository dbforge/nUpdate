using System.Globalization;
using nUpdate.Updating;

namespace nUpdate.Ui;

internal static class ChangelogFormatter
{
    public static string Format(IEnumerable<PackageInfo> packages, CultureInfo culture, string newLine = "\n")
    {
        if (packages is null)
            throw new ArgumentNullException(nameof(packages));
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        var blocks = packages
            .OrderBy(p => p.Version)
            .Select(p => $"{p.Version}:{newLine}{p.GetChangelog(culture)}");
        return string.Join(newLine + newLine, blocks);
    }
}
