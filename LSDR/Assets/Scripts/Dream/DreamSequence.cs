using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace LSDR.Dream
{
    /// <summary>
    /// A DreamSequence is a sequence of dreams visited in a day of play.
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DreamSequence
    {
        /// <summary>
        /// The list of visited dreams this day.
        /// </summary>
        public readonly List<Dream> Visited = new List<Dream>();

        /// <summary>
        /// The total upper score across all of the visited dreams.
        /// </summary>
        [ProtoIgnore]
        public int UpperScore { get { return Visited.Sum(d => d.Upperness); } }
        
        /// <summary>
        /// The total dynamic score across all of the visited dreams.
        /// </summary>
        [ProtoIgnore]
        public int DynamicScore { get { return Visited.Sum(d => d.Dynamicness); } }
    }
}
