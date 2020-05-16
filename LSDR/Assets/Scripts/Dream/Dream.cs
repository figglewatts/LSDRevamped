using System.Collections.Generic;
using LSDR.Util;
using Newtonsoft.Json;
using ProtoBuf;
using Torii.Util;

namespace LSDR.Dream
{
    /// <summary>
    /// Dream is used as a data container for Dream information.
    /// </summary>
    [JsonObject]
    [ProtoContract]
    public class Dream
    {
        /// <summary>
        /// The name of this Dream.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }
        
        /// <summary>
        /// The author of this Dream.
        /// </summary>
        [ProtoMember(2)]
        public string Author { get; set; }

        /// <summary>
        /// The type of this Dream. Used to determine how to load the Dream.
        /// </summary>
        [ProtoMember(3)]
        public DreamType Type { get; set; } = DreamType.Revamped;
        
        /// <summary>
        /// What effect this dream has on the 'upper' axis of the graph, when visited.
        /// </summary>
        [ProtoMember(4)]
        public int Upperness { get; set; }
        
        /// <summary>
        /// What effect this dream has on the 'dynamic' axis of the graph, when visited.
        /// </summary>
        [ProtoMember(5)]
        public int Dynamicness { get; set; }

        /// <summary>
        /// The path to the raw level file to load for this Dream.
        /// </summary>
        [ProtoMember(6)]
        public string Level { get; set; }

        /// <summary>
        /// The path to the LBD folder to load for this Dream.
        /// </summary>
        [ProtoMember(7)]
        public string LBDFolder { get; set; }
        
        /// <summary>
        /// Whether or not this dream can spawn the grey man.
        /// </summary>
        [ProtoMember(8)]
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

        /// <summary>
        /// Get a random environment from this dream.
        /// </summary>
        /// <returns>The random environment.</returns>
        public DreamEnvironment RandomEnvironment() { return RandUtil.RandomListElement(Environments); }
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
