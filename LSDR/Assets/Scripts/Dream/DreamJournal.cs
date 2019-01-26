using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dream
{
    [JsonObject]
    public class DreamJournal
    {
        public string Name { get; set; }
        
        public string Author { get; set; }
        
        public List<string> LinkableDreams { get; set; }
        
        public List<string> FirstDream { get; set; }
        
        public List<string> FirstSpawn { get; set; }
        
        public string GraphMapTexture { get; set; }
    }
}
