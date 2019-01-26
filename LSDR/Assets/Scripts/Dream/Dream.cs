using Newtonsoft.Json;

namespace Dream
{
    [JsonObject]
    public class Dream
    {
        public string Name { get; set; }
        
        public string Author { get; set; }
        
        public DreamType Type { get; set; }
        
        public int Upperness { get; set; }
        
        public int Dynamicness { get; set; }
        
        public string Level { get; set; }
        
        public bool GreyMan { get; set; }
    }

    public enum DreamType
    {
        Legacy,
        Revamped
    }
}
