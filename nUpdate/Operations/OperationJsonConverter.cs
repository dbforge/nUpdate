using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nUpdate.Operations;

/// <summary>Reads an <see cref="Operation" /> by its <c>type</c> discriminator. Writing needs no converter: <see cref="Operation.Type" /> is a normal property.</summary>
public sealed class OperationJsonConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof(Operation).IsAssignableFrom(objectType);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));
        if (serializer is null)
            throw new ArgumentNullException(nameof(serializer));
        if (reader.TokenType == JsonToken.Null)
            return null;

        var json = JObject.Load(reader);
        var type = json["type"]?.Type == JTokenType.String ? (string)json["type"]! : null;
        if (type is null)
            throw new JsonSerializationException("An operation needs a \"type\".");
        if (!Operation.Types.TryGetValue(type, out var operationType))
            throw new JsonSerializationException($"\"{type}\" is not a known operation type.");

        var operation = (Operation)Activator.CreateInstance(operationType)!;
        using var objectReader = json.CreateReader();
        serializer.Populate(objectReader, operation);
        return operation;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotSupportedException();
}
