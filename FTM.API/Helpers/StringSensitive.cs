﻿using Newtonsoft.Json;

namespace FTM.API.Helpers
{
    public class StringSensitive : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var stringValue = reader.Value != null ? reader.Value.ToString() : string.Empty;
            return stringValue.RemoveExtraWhiteSpace();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
