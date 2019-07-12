using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dream
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
        /// If not empty, then force the spawn point to a random choice of spawn points given. This will only work in
        /// Revamped dreams, as Legacy dreams do not support spawn points.
        /// </summary>
        public List<string> FirstSpawn { get; set; }
        
        /// <summary>
        /// The path in the 'textures' directory to the graph spawn texture. This decides which dream in the journal
        /// to spawn on based on the last graph position.
        /// </summary>
        public string GraphMapTexture { get; set; }
    }
}
