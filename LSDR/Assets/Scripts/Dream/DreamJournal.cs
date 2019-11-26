using System.Collections.Generic;
using LSDR.Util;
using Newtonsoft.Json;

namespace LSDR.Dream
{
    /// <summary>
    /// DreamJournal is used to store all information about a dream journal (set of dreams to use when playing).
    /// </summary>
    [JsonObject]
    public class DreamJournal
    {
        /// <summary>
        /// The name of this dream journal.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The author of this dream journal.
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// The list of dreams you can get to by linking. These will be chosen from randomly. You cannot link to the
        /// dream you are currently in.
        /// </summary>
        public List<string> LinkableDreams { get; set; }
        
        /// <summary>
        /// The dream to spawn on on the first day of the journal. If empty, then a random dream from LinkableDreams.
        /// If multiple, then a random choice from the dreams given.
        /// </summary>
        public List<string> FirstDream { get; set; }

        /// <summary>
        /// Get a random linkable dream from the pool of linkable dreams.
        /// </summary>
        /// <returns>The random dream.</returns>
        public string GetLinkableDream() { return RandUtil.RandomListElement(LinkableDreams); }

        public DreamJournal()
        {
            LinkableDreams = new List<string>();
            FirstDream = new List<string>();
        }
    }
}
