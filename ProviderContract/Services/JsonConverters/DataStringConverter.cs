using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ProviderContract.Services.JsonConverters
{
    /// <summary>
    /// Десериализует объект JSON в строку
    /// </summary>
    public class DataStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            JRaw raw = JRaw.Create(reader);
            return raw.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string s = (string)value;
            writer.WriteRawValue(s);
        }
    }
}
