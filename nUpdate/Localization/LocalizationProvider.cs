using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;

namespace nUpdate.Localization;

/// <summary>
///     Loads <see cref="UpdateTexts" /> from the embedded language files or from custom files on disk.
/// </summary>
internal sealed class LocalizationProvider
{
    private static readonly string[] IntegratedCultureNames = ["de-AT", "de-CH", "de-DE", "en", "it-IT", "zh-CN"];

    private readonly IFileSystem _fileSystem;

    public LocalizationProvider()
        : this(new FileSystem())
    {
    }

    public LocalizationProvider(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    /// <summary>The culture used when none is specified.</summary>
    public static CultureInfo DefaultCulture => new("en");

    /// <summary>The cultures shipped inside the library. Created on demand so that a host without culture data still loads the type.</summary>
    public static IReadOnlyList<CultureInfo> IntegratedCultures => IntegratedCultureNames.Select(name => new CultureInfo(name)).ToArray();

    public static bool IsIntegratedCulture(CultureInfo culture) =>
        culture is not null && IntegratedCultureNames.Contains(culture.Name, StringComparer.OrdinalIgnoreCase);

    /// <summary>Returns whether the culture is either integrated or has a custom file registered.</summary>
    public static bool IsAvailable(CultureInfo culture, IReadOnlyDictionary<CultureInfo, string>? customFiles) =>
        IsIntegratedCulture(culture) || TryGetCustomFile(culture, customFiles) is not null;

    /// <summary>
    ///     The culture whose texts are used for the given one: itself when available, else the nearest available parent
    ///     (<c>de-LI</c> falls back to <c>de</c> and then to the German files shipped as <c>de-DE</c>), else English.
    /// </summary>
    public static CultureInfo Resolve(CultureInfo culture, IReadOnlyDictionary<CultureInfo, string>? customFiles)
    {
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));
        for (var current = culture; !string.IsNullOrEmpty(current.Name); current = current.Parent)
        {
            if (IsAvailable(current, customFiles))
                return current;
            if (current.Name.IndexOf('-') < 0)
            {
                // A neutral culture without texts of its own takes a specific one of the same language, the "home" region first.
                var candidates = IntegratedCultureNames.Concat(customFiles?.Keys.Select(c => c.Name) ?? []).Where(name => name.StartsWith(current.Name + "-", StringComparison.OrdinalIgnoreCase)).ToList();
                var sibling = candidates.FirstOrDefault(name => string.Equals(name, current.Name + "-" + current.Name.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase)) ?? candidates.FirstOrDefault();
                if (sibling is not null)
                    return new CultureInfo(sibling);
            }
        }

        return DefaultCulture;
    }

    /// <summary>Loads the texts of the culture. Custom files take precedence over integrated ones.</summary>
    /// <exception cref="ArgumentException">The culture is neither integrated nor registered as a custom file.</exception>
    public UpdateTexts Load(CultureInfo culture, IReadOnlyDictionary<CultureInfo, string>? customFiles = null)
    {
        if (culture is null)
            throw new ArgumentNullException(nameof(culture));

        var customFile = TryGetCustomFile(culture, customFiles);
        if (customFile is not null)
        {
            if (!_fileSystem.File.Exists(customFile))
                throw new FileNotFoundException($"The localization file \"{customFile}\" for culture \"{culture.Name}\" does not exist.", customFile);
            return Parse(_fileSystem.File.ReadAllText(customFile), culture);
        }

        using var stream = typeof(LocalizationProvider).GetTypeInfo().Assembly
            .GetManifestResourceStream($"nUpdate.Localization.{Resolve(culture)}.json")
            ?? throw new ArgumentException($"The culture \"{culture.Name}\" is not available. Register a custom localization file for it.", nameof(culture));
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd(), culture);
    }

    private static string Resolve(CultureInfo culture) =>
        IntegratedCultureNames.FirstOrDefault(name => string.Equals(name, culture.Name, StringComparison.OrdinalIgnoreCase)) ?? culture.Name;

    private static string? TryGetCustomFile(CultureInfo culture, IReadOnlyDictionary<CultureInfo, string>? customFiles)
    {
        if (culture is null || customFiles is null)
            return null;
        foreach (var pair in customFiles)
        {
            if (string.Equals(pair.Key.Name, culture.Name, StringComparison.OrdinalIgnoreCase))
                return pair.Value;
        }

        return null;
    }

    private static UpdateTexts Parse(string json, CultureInfo culture)
    {
        try
        {
            return Serializer.Deserialize<UpdateTexts>(json)
                   ?? throw new InvalidDataException($"The localization file for \"{culture.Name}\" is empty.");
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            throw new InvalidDataException($"The localization file for \"{culture.Name}\" is not valid JSON.", ex);
        }
    }
}
