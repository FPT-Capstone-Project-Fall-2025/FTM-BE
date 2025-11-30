using Newtonsoft.Json;

namespace FTM.Domain.Helpers
{
    public class PhoneSensitive : JsonConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var stringValue = reader.Value != null ? reader.Value.ToString() : string.Empty;
            return stringValue.FormatPhoneNumber();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
