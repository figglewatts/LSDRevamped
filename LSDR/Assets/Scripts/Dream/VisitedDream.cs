using Newtonsoft.Json;

namespace LSDR.Dream
{
    [JsonObject]
    public struct VisitedDream
    {
        public string Name;
        public string Author;

        public static implicit operator VisitedDream(SDK.Data.Dream dream)
        {
            return new VisitedDream
            {
                Name = dream.Name,
                Author = dream.Author
            };
        }
    }
}
