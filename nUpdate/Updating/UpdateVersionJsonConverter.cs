using Newtonsoft.Json;

namespace nUpdate.Updating;

/// <summary>Writes an <see cref="UpdateVersion" /> as its canonical string and reads it back.</summary>
public sealed class UpdateVersionJsonConverter : JsonConverter<UpdateVersion>
{
    public override void WriteJson(JsonWriter writer, UpdateVersion? value, JsonSerializer serializer)
    {
        if (writer is null)
            throw new ArgumentNullException(nameof(writer));
        if (value is null)
            writer.WriteNull();
        else
            writer.WriteValue(value.ToString());
    }

    public override UpdateVersion? ReadJson(JsonReader reader, Type objectType, UpdateVersion? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader is null)
            throw new ArgumentNullException(nameof(reader));
        if (reader.TokenType == JsonToken.Null)
            return null;
        if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException($"A version must be a string, not {reader.TokenType}.");
        var text = (string)reader.Value!;
        return UpdateVersion.TryParse(text, out var version) ? version : throw new JsonSerializationException($"\"{text}\" is not a valid version. {UpdateVersion.FormatDescription}");
    }
}
