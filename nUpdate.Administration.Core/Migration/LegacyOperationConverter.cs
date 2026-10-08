using System.Globalization;
using Newtonsoft.Json.Linq;
using nUpdate.Operations;

namespace nUpdate.Administration.Core.Migration;

/// <summary>The typed operations of a legacy operation list and what could not be converted.</summary>
public sealed class LegacyOperationConversion
{
    public LegacyOperationConversion(IReadOnlyList<Operation> operations, IReadOnlyList<string> warnings)
    {
        Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        Warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
    }

    public IReadOnlyList<Operation> Operations { get; }

    /// <summary>One sentence per operation or registry value that is left out, numbered like the old list.</summary>
    public IReadOnlyList<string> Warnings { get; }
}

/// <summary>
///     Converts operations stored by nUpdate 3.x and 4.x (<c>Area</c>, <c>Method</c>, <c>Value</c>, <c>Value2</c>) into
///     the typed operations of the package manifest.
/// </summary>
public static class LegacyOperationConverter
{
    private static readonly string[] Areas = ["Files", "Registry", "Processes", "Services", "Scripts"];
    private static readonly string[] Methods = ["Create", "Delete", "Rename", "SetValue", "DeleteValue", "Start", "Stop", "Execute"];

    /// <summary>Converts a list; entries that are not objects, name an unknown combination or carry unreadable registry values are reported and left out.</summary>
    public static LegacyOperationConversion Convert(JArray? operations)
    {
        var result = new List<Operation>();
        var warnings = new List<string>();
        var number = 0;
        foreach (var item in operations ?? [])
        {
            number++;
            if (item is not JObject legacy)
            {
                warnings.Add($"Operation {number} cannot be read and is left out.");
                continue;
            }

            Operation? operation;
            try
            {
                operation = Convert(legacy, number, warnings);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException or InvalidCastException or ArgumentException)
            {
                warnings.Add($"Operation {number} cannot be read and is left out: {ex.Message}");
                continue;
            }

            if (operation is not null)
                result.Add(operation);
            else
                warnings.Add($"Operation {number} ({Describe(legacy["Area"], Areas)} {Describe(legacy["Method"], Methods)}) has no counterpart in nUpdate 5 and is left out.");
        }

        return new LegacyOperationConversion(result, warnings);
    }

    /// <returns>The typed operation, or <c>null</c> for an unknown area/method combination.</returns>
    public static Operation? Convert(JObject legacy)
    {
        ArgumentNullException.ThrowIfNull(legacy);
        return Convert(legacy, 1, []);
    }

    private static Operation? Convert(JObject legacy, int number, List<string> warnings)
    {
        var area = Name(legacy["Area"], Areas);
        var method = Name(legacy["Method"], Methods);
        var value = Text(legacy["Value"]);
        var value2 = legacy["Value2"];
        var before = legacy.Value<bool?>("ExecuteBeforeReplacingFiles") ?? false;

        Operation? operation = (area, method) switch
        {
            ("Files", "Delete") => new DeleteFilesOperation { Directory = value, Files = Strings(value2) },
            ("Files", "Rename") => new RenameFileOperation { Path = value, NewName = Text(value2) },
            ("Registry", "Create") => new CreateRegistryKeysOperation { Key = value, SubKeys = Strings(value2) },
            ("Registry", "Delete") => new DeleteRegistryKeysOperation { Key = value, SubKeys = Strings(value2) },
            ("Registry", "SetValue") => new SetRegistryValuesOperation { Key = value, Values = RegistryValues(value2, number, warnings) },
            ("Registry", "DeleteValue") => new DeleteRegistryValuesOperation { Key = value, Names = Strings(value2) },
            ("Processes", "Start") => new StartProcessOperation { Path = value, Arguments = Text(value2) },
            ("Processes", "Stop") => new TerminateProcessOperation { ProcessName = value },
            ("Services", "Start") => new StartServiceOperation { ServiceName = value, Arguments = Strings(value2) },
            ("Services", "Stop") => new StopServiceOperation { ServiceName = value },
            _ => null,
        };
        if (operation is not null)
            operation.RunBeforeFileReplacement = before;
        return operation;
    }

    private static string? Name(JToken? token, string[] names)
    {
        if (token is { Type: JTokenType.Integer })
        {
            var index = token.Value<int>();
            return index >= 0 && index < names.Length ? names[index] : null;
        }

        return names.FirstOrDefault(n => string.Equals(n, Text(token), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>The name of an area or method for a warning: the known name, else what the file says.</summary>
    private static string Describe(JToken? token, string[] names) => Name(token, names) ?? (Text(token) is { Length: > 0 } raw ? raw : "?");

    private static List<string> Strings(JToken? token) => token switch
    {
        JArray array => array.Select(t => t.ToString()).ToList(),
        _ => Text(token) is { Length: > 0 } single ? [single] : [],
    };

    private static List<RegistryValue> RegistryValues(JToken? token, int number, List<string> warnings)
    {
        var values = new List<RegistryValue>();
        foreach (var item in (token as JArray ?? []).OfType<JObject>())
        {
            var name = First(item, "Name", "name", "Item1")?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                warnings.Add($"Operation {number}: a registry value without a name is left out.");
                continue;
            }

            var kind = Kind(First(item, "Kind", "kind", "Item3"));
            if (RegistryValue(name, kind, First(item, "Value", "value", "Item2")) is { } value)
                values.Add(value);
            else
                warnings.Add($"Operation {number}: the registry value \"{name}\" cannot be read as {kind} and is left out.");
        }

        return values;
    }

    private static RegistryValue? RegistryValue(string name, RegistryValueKind kind, JToken? value)
    {
        try
        {
            return kind switch
            {
                RegistryValueKind.DWord => Operations.RegistryValue.DWord(name, Number(value)),
                RegistryValueKind.QWord => Operations.RegistryValue.QWord(name, Number(value)),
                RegistryValueKind.MultiString => Operations.RegistryValue.MultiString(name, value is JArray array ? array.Select(t => t.ToString()).ToArray() : Split(value)),
                RegistryValueKind.Binary => Operations.RegistryValue.Binary(name, Bytes(value)),
                RegistryValueKind.ExpandString => Operations.RegistryValue.ExpandString(name, Text(value)),
                _ => Operations.RegistryValue.String(name, Text(value)),
            };
        }
        catch (Exception ex) when (ex is FormatException or OverflowException)
        {
            return null;
        }
    }

    private static RegistryValueKind Kind(JToken? token)
    {
        // Microsoft.Win32.RegistryValueKind: String = 1, ExpandString = 2, Binary = 3, DWord = 4, MultiString = 7, QWord = 11.
        if (token is { Type: JTokenType.Integer })
        {
            return token.Value<int>() switch
            {
                2 => RegistryValueKind.ExpandString,
                3 => RegistryValueKind.Binary,
                4 => RegistryValueKind.DWord,
                7 => RegistryValueKind.MultiString,
                11 => RegistryValueKind.QWord,
                _ => RegistryValueKind.String,
            };
        }

        return Enum.TryParse<RegistryValueKind>(Text(token), ignoreCase: true, out var kind) ? kind : RegistryValueKind.String;
    }

    /// <summary>The text of a token: strings as they are, other values as compact JSON, missing and null as empty.</summary>
    private static string Text(JToken? value) => value switch
    {
        null => string.Empty,
        { Type: JTokenType.Null } => string.Empty,
        { Type: JTokenType.String } => value.ToString(),
        _ => value.ToString(Newtonsoft.Json.Formatting.None),
    };

    private static long Number(JToken? value) => value is { Type: JTokenType.Integer } ? value.Value<long>() : long.Parse(Text(value), NumberStyles.Integer, CultureInfo.InvariantCulture);

    private static string[] Split(JToken? value) => Text(value).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static byte[] Bytes(JToken? value)
    {
        if (value is JArray array)
            return array.Select(t => byte.Parse(t.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture)).ToArray();
        var text = Text(value);
        var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.All(p => byte.TryParse(p, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            return parts.Select(p => byte.Parse(p, CultureInfo.InvariantCulture)).ToArray();
        return System.Convert.FromBase64String(text); // Newtonsoft wrote byte arrays as Base64
    }

    private static JToken? First(JObject json, params string[] names)
    {
        foreach (var candidate in names)
        {
            if (json[candidate] is { } token)
                return token;
        }

        return null;
    }
}
