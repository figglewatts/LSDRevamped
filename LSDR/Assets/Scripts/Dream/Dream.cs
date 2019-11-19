using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSDR.Dream
{
    /// <summary>
    /// Dream is used as a data container for Dream information.
    /// </summary>
    [JsonObject]
    public class Dream
    {
        /// <summary>
        /// The name of this Dream.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The author of this Dream.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The type of this Dream. Used to determine how to load the Dream.
        /// </summary>
        public DreamType Type { get; set; } = DreamType.Revamped;
        
        /// <summary>
        /// What effect this dream has on the 'upper' axis of the graph, when visited.
        /// </summary>
        public int Upperness { get; set; }
        
        /// <summary>
        /// What effect this dream has on the 'dynamic' axis of the graph, when visited.
        /// </summary>
        public int Dynamicness { get; set; }

        /// <summary>
        /// The path to the raw level file to load for this Dream. Can be an LBD or a TMAP.
        /// </summary>
        public string Level { get; set; }
        
        /// <summary>
        /// Whether or not this dream can spawn the grey man.
        /// </summary>
        public bool GreyMan { get; set; }

        /// <summary>
        /// The list of environments that this dream can have.
        /// </summary>
        public List<DreamEnvironment> Environments { get; set; }
        
        /// <summary>
        /// The tiling mode of this dream if it's a legacy dream.
        /// </summary>
        public LegacyTileMode LegacyTileMode { get; set; }

        /// <summary>
        /// The width of the tilemap if it's a legacy dream and the tiling mode is horizontal.
        /// </summary>
        public int TileWidth { get; set; } = 1;

        public Dream()
        {
            Environments = new List<DreamEnvironment>();
        }
    }

    /// <summary>
    /// The DreamType enum is used to store whether this is a Legacy or a Revamped dream.
    /// This will decide whether to load the dream as a bunch of LBD files, or as a TMAP.
    /// </summary>
    public enum DreamType
    {
        Legacy,
        Revamped
    }

    /// <summary>
    /// Legacy dreams loaded from LBD files support two tiling modes - vertical and horizontal. This enum contains
    /// the two possible tiling modes.
    /// </summary>
    public enum LegacyTileMode
    {
        Vertical,
        Horizontal
    }
}
