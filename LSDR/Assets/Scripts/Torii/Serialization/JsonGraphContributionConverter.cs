using System;
using LSDR.SDK.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Torii.Serialization
{
    public class JsonGraphContributionConverter : JsonConverter<GraphContribution>
    {
        public override void WriteJson(JsonWriter writer, GraphContribution value, JsonSerializer serializer)
        {
            JObject obj = new JObject
            {
                { "contribution", new JArray(value.Dynamic, value.Upper) },
                { "position", new JArray(value.PlayerPosition.x, value.PlayerPosition.y, value.PlayerPosition.z) },
                { "yRotation", value.PlayerYRotation }
            };
            obj.WriteTo(writer);
        }

        public override GraphContribution ReadJson(JsonReader reader, Type objectType, GraphContribution existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            try
            {
                JObject obj = JObject.Load(reader);

                if (!obj.ContainsKey("contribution"))
                {
                    throw new JsonSerializationException("missing required key 'contribution' in object");
                }

                var contributionArray = obj["contribution"]!.ToObject<int[]>();
                float[] positionArray = obj.ContainsKey("position")
                    ? obj["position"]!.ToObject<float[]>()
                    : new float[] { 0, 0, 0 };
                float yRotation = obj.ContainsKey("yRotation") ? obj["yRotation"]!.ToObject<float>() : 0f;

                Vector3 position = new Vector3(positionArray[0], positionArray[1], positionArray[2]);

                GraphContribution graphContribution =
                    new GraphContribution(contributionArray[0], contributionArray[1], position, yRotation);

                return graphContribution;
            }
            catch (JsonReaderException)
            {
                // if the object was invalid it's very likely to be data in the old format, so try loading it like that
                // i.e. without player position and rotation
                JArray arr = JArray.Load(reader);
                return new GraphContribution(arr[index: 0].ToObject<int>(), arr[index: 1].ToObject<int>());
            }
        }
    }
}
