using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Torii.Serialization
{
    public class JsonColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color col, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(col.r);
            writer.WriteValue(col.g);
            writer.WriteValue(col.b);
            writer.WriteValue(col.a);
            writer.WriteEndArray();
        }

        public override Color ReadJson(JsonReader reader,
            Type objectType,
            Color existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            return new Color(arr[index: 0].ToObject<float>(), arr[index: 1].ToObject<float>(),
                arr[index: 2].ToObject<float>(), arr[index: 3].ToObject<float>());
        }
    }
}
