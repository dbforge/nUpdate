// UriConverter.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;
using Newtonsoft.Json;

namespace nUpdate
{
    internal class UriConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return new Uri((string) reader.Value);
                case JsonToken.Null:
                    return null;
            }

            throw new InvalidOperationException(
                "Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (null == value)
            {
                writer.WriteNull();
                return;
            }

            var uri = value as Uri;
            if (uri == null)
                throw new InvalidOperationException(
                    "Unhandled case for UriConverter. Check to see if this converter has been applied to the wrong serialization type.");
            writer.WriteValue(uri.OriginalString);
        }
    }
}