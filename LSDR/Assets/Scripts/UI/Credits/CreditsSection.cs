using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace LSDR.UI.Credits
{
    [JsonObject]
    public class CreditsSection
    {
        public List<string> Names;
        public string SectionImagePath;
        public string SectionTitle;

        [DefaultValue(value: true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool SortNames;
    }
}
