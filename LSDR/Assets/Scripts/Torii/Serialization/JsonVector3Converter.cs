using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Torii.Serialization
{
    public class JsonVector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 vec3, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(vec3.x);
            writer.WriteValue(vec3.y);
            writer.WriteValue(vec3.z);
            writer.WriteEndArray();
        }

        public override Vector3 ReadJson(JsonReader reader,
            Type objectType,
            Vector3 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            return new Vector3(arr[0].ToObject<float>(), arr[1].ToObject<float>(), arr[2].ToObject<float>());
        }
    }
}
