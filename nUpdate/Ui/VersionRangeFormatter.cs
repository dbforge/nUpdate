using nUpdate.Updating;

namespace nUpdate.Ui;

internal static class VersionRangeFormatter
{
    public static string Format(IEnumerable<UpdateVersion> versions)
    {
        if (versions is null)
            throw new ArgumentNullException(nameof(versions));

        var list = versions.ToList();
        if (list.Count == 0)
            return string.Empty;
        if (list.Count <= 2)
            return string.Join(", ", list.OrderBy(v => v).Select(v => v.ToString()));
        return $"{UpdateVersion.Min(list)} - {UpdateVersion.Max(list)}";
    }
}
