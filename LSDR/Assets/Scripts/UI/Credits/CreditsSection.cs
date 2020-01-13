using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LSDR.UI.Credits
{
    [JsonObject]
    public class CreditsSection
    {
        public string SectionTitle;
        public string SectionImagePath;
        public List<string> Names;
        
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool SortNames;
    }
}
