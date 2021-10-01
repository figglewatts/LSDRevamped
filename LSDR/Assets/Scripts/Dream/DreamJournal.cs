using System.Collections.Generic;
using LSDR.SDK.Data;
using LSDR.Util;
using Newtonsoft.Json;
using Torii.Util;

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
        /// The mapping of which graph squares spawn the player into what dreams.
        /// </summary>
        public GraphSpawnMap GraphSpawnMap { get; set; }

        /// <summary>
        /// The folder to shuffle music from.
        /// </summary>
        public string MusicFolder { get; set; }

        /// <summary>
        /// Get a random linkable dream from the pool of linkable dreams.
        /// </summary>
        /// <returns>The random dream.</returns>
        public string GetLinkableDream() { return RandUtil.RandomListElement(LinkableDreams); }

        /// <summary>
        /// Get a random dream from the pool of first day dreams.
        /// </summary>
        /// <returns>The random dream.</returns>
        public string GetFirstDream() { return RandUtil.RandomListElement(FirstDream); }

        /// <summary>
        /// Use coordinates on the graph spawn map to get a dream to load.
        /// </summary>
        /// <param name="x">X coord on the graph.</param>
        /// <param name="y">Y coord on the graph.</param>
        /// <returns>The dream from the graph.</returns>
        public string GetDreamFromGraph(int x, int y)
        {
            string graphDreamPath = GraphSpawnMap.Get(x, y);
            if (string.IsNullOrWhiteSpace(graphDreamPath))
            {
                return GetLinkableDream();
            }

            return graphDreamPath;
        }

        public DreamJournal()
        {
            LinkableDreams = new List<string>();
            FirstDream = new List<string>();
            GraphSpawnMap = new GraphSpawnMap();
            MusicFolder = "music/lsdr";
        }
    }
}
