using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Torii.Serialization
{
    public class JsonQuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter writer, Quaternion q, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(q.x);
            writer.WriteValue(q.y);
            writer.WriteValue(q.z);
            writer.WriteValue(q.w);
            writer.WriteEndArray();
        }

        public override Quaternion ReadJson(JsonReader reader,
            Type objectType,
            Quaternion existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            return new Quaternion(arr[0].ToObject<float>(), arr[1].ToObject<float>(), 
                arr[2].ToObject<float>(), arr[3].ToObject<float>());
        }
    }
}
