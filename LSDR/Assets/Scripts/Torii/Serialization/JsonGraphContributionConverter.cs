using System;
using LSDR.SDK.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Torii.Serialization
{
    public class JsonGraphContributionConverter : JsonConverter<GraphContribution>
    {
        public override void WriteJson(JsonWriter writer, GraphContribution contribution, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(contribution.Dynamic);
            writer.WriteValue(contribution.Upper);
            writer.WriteEndArray();
        }

        public override GraphContribution ReadJson(JsonReader reader,
            Type objectType,
            GraphContribution existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray arr = JArray.Load(reader);
            return new GraphContribution(arr[index: 0].ToObject<int>(), arr[index: 1].ToObject<int>());
        }
    }
}
