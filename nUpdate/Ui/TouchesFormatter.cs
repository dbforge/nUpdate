using nUpdate.Localization;
using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate.Ui;

/// <summary>Describes what the selected packages will touch on the client, for the confirmation dialog.</summary>
internal static class TouchesFormatter
{
    /// <summary>The areas the files of the packages for the platform touch, each once.</summary>
    public static IReadOnlyList<string> Describe(IEnumerable<PackageInfo> packages, string platform, UpdateTexts texts)
    {
        if (packages is null)
            throw new ArgumentNullException(nameof(packages));
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));

        return packages
            .SelectMany(p => p.FindFile(platform)?.Touches ?? [])
            .Distinct()
            .OrderBy(area => area)
            .Select(area => Describe(area, texts))
            .ToList();
    }

    public static string Describe(OperationArea area, UpdateTexts texts)
    {
        if (texts is null)
            throw new ArgumentNullException(nameof(texts));

        return area switch
        {
            OperationArea.Files => texts.TouchesFiles,
            OperationArea.Registry => texts.TouchesRegistry,
            OperationArea.Processes => texts.TouchesProcesses,
            OperationArea.Services => texts.TouchesServices,
            _ => throw new ArgumentOutOfRangeException(nameof(area)),
        };
    }
}
