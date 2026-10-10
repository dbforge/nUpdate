using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nUpdate.Operations;

/// <summary>The data type of a registry value.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name",
    Justification = "The names are the registry's own.")]
public enum RegistryValueKind
{
    [EnumMember(Value = "string")] String,

    [EnumMember(Value = "expandString")] ExpandString,

    [EnumMember(Value = "dword")] DWord,

    [EnumMember(Value = "qword")] QWord,

    [EnumMember(Value = "multiString")] MultiString,

    [EnumMember(Value = "binary")] Binary,
}

/// <summary>
///     One registry value of a <see cref="SetRegistryValuesOperation" />. The CLR type of <see cref="Value" /> follows
///     <see cref="Kind" />: <see cref="string" /> for strings, <see cref="long" /> for DWORD and QWORD, <c>string[]</c>
///     for multi-strings and <c>byte[]</c> for binary values.
/// </summary>
[JsonConverter(typeof(RegistryValueJsonConverter))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name",
    Justification = "The factory methods are named after the registry kinds.")]
public sealed class RegistryValue(string name, RegistryValueKind kind, object? value)
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    public RegistryValueKind Kind { get; } = kind;

    public object? Value { get; } = Check(kind, value);

    public static RegistryValue String(string name, string value) => new(name, RegistryValueKind.String, value);

    public static RegistryValue ExpandString(string name, string value) =>
        new(name, RegistryValueKind.ExpandString, value);

    public static RegistryValue DWord(string name, long value) => new(name, RegistryValueKind.DWord, value);

    public static RegistryValue QWord(string name, long value) => new(name, RegistryValueKind.QWord, value);

    public static RegistryValue MultiString(string name, params string[] values) =>
        new(name, RegistryValueKind.MultiString, values);

    public static RegistryValue Binary(string name, byte[] value) => new(name, RegistryValueKind.Binary, value);

    private static object? Check(RegistryValueKind kind, object? value)
    {
        var valid = kind switch
        {
            RegistryValueKind.String or RegistryValueKind.ExpandString => value is null or string,
            RegistryValueKind.DWord => value is long and >= int.MinValue and <= uint.MaxValue,
            RegistryValueKind.QWord => value is long,
            RegistryValueKind.MultiString => value is string[],
            RegistryValueKind.Binary => value is byte[],
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
        return valid
            ? value
            : throw new ArgumentException(
                $"A {kind} registry value cannot hold {(value is long number ? number.ToString(System.Globalization.CultureInfo.InvariantCulture) : value?.GetType().Name ?? "null")}.",
                nameof(value));
    }
}

/// <summary>Writes a <see cref="RegistryValue" /> as <c>{ name, kind, value }</c> with the value typed by its kind.</summary>
internal sealed class RegistryValueJsonConverter : JsonConverter<RegistryValue>
{
    public override void WriteJson(JsonWriter writer, RegistryValue? value, JsonSerializer serializer)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(value.Name);
        writer.WritePropertyName("kind");
        writer.WriteValue(KindName(value.Kind));
        writer.WritePropertyName("value");
        switch (value.Kind)
        {
            case RegistryValueKind.DWord:
            case RegistryValueKind.QWord:
                writer.WriteValue((long)value.Value!);
                break;
            case RegistryValueKind.MultiString:
                writer.WriteStartArray();
                foreach (var item in (string[])value.Value!)
                    writer.WriteValue(item);
                writer.WriteEndArray();
                break;
            case RegistryValueKind.Binary:
                writer.WriteValue(Convert.ToBase64String((byte[])value.Value!));
                break;
            default:
                writer.WriteValue((string?)value.Value);
                break;
        }

        writer.WriteEndObject();
    }

    public override RegistryValue? ReadJson(JsonReader reader, Type objectType, RegistryValue? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));
        if (reader.TokenType == JsonToken.Null)
            return null;

        var json = JObject.Load(reader);
        var name = json["name"]?.Type == JTokenType.String
            ? (string)json["name"]!
            : throw new JsonSerializationException("A registry value needs a \"name\".");
        var kindText = json["kind"]?.Type == JTokenType.String
            ? (string)json["kind"]!
            : throw new JsonSerializationException($"The registry value \"{name}\" needs a \"kind\".");
        var kind = ParseKind(kindText) ??
                   throw new JsonSerializationException($"\"{kindText}\" is not a registry value kind.");
        var token = json["value"];
        try
        {
            return new RegistryValue(name, kind, ReadValue(kind, token, name));
        }
        catch (FormatException ex)
        {
            throw new JsonSerializationException($"The registry value \"{name}\" is not valid Base64.", ex);
        }
        catch (ArgumentException ex)
        {
            throw new JsonSerializationException($"The registry value \"{name}\" is out of range: {ex.Message}", ex);
        }
    }

    private static readonly Dictionary<RegistryValueKind, string> Names = new()
    {
        [RegistryValueKind.String] = "string",
        [RegistryValueKind.ExpandString] = "expandString",
        [RegistryValueKind.DWord] = "dword",
        [RegistryValueKind.QWord] = "qword",
        [RegistryValueKind.MultiString] = "multiString",
        [RegistryValueKind.Binary] = "binary",
    };

    private static readonly Dictionary<string, RegistryValueKind> Kinds =
        Names.ToDictionary(pair => pair.Value, pair => pair.Key, StringComparer.OrdinalIgnoreCase);

    private static string KindName(RegistryValueKind kind) => Names[kind];

    private static RegistryValueKind? ParseKind(string text) => Kinds.TryGetValue(text, out var kind) ? kind : null;

    private static object? ReadValue(RegistryValueKind kind, JToken? token, string name)
    {
        if (token is null || token.Type == JTokenType.Null)
            return kind is RegistryValueKind.String or RegistryValueKind.ExpandString
                ? null
                : throw new JsonSerializationException($"The registry value \"{name}\" has no value.");
        switch (kind)
        {
            case RegistryValueKind.DWord:
            case RegistryValueKind.QWord:
                return long.TryParse(token.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out var number)
                    ? number
                    : throw new JsonSerializationException(
                        $"The registry value \"{name}\" is not a number in the range of a {kind}.");
            case RegistryValueKind.MultiString:
                return token.Type == JTokenType.Array
                    ? token.Select(item => item.ToString()).ToArray()
                    : throw new JsonSerializationException(
                        $"The registry value \"{name}\" must be an array of strings.");
            case RegistryValueKind.Binary:
                return Convert.FromBase64String(token.ToString());
            default:
                return token.Type == JTokenType.String ? (string)token! : token.ToString();
        }
    }
}
