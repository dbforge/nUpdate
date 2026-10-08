using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using nUpdate.Operations;
using nUpdate.Updating;

namespace nUpdate;

/// <summary>
///     The JSON conventions every nUpdate file format shares: camelCase names, enums as camelCase strings, versions as
///     canonical strings, operations with a <c>type</c> discriminator, dates in ISO 8601.
/// </summary>
internal static class Serializer
{
    public static JsonSerializerSettings Settings { get; } = Create();

    public static T? Deserialize<T>(string content) => JsonConvert.DeserializeObject<T>(content, Settings);

    public static T? Deserialize<T>(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
        return Deserialize<T>(reader.ReadToEnd());
    }

    public static string Serialize(object? value, bool indented = false) =>
        JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None, Settings);

    private static JsonSerializerSettings Create()
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateParseHandling = DateParseHandling.None,
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        };
        settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        settings.Converters.Add(new UpdateVersionJsonConverter());
        settings.Converters.Add(new OperationJsonConverter());
        settings.Converters.Add(new RegistryValueJsonConverter());
        return settings;
    }
}
