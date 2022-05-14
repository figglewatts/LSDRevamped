using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using UnityEngine;

namespace LSDR.Dream
{
    /// <summary>
    /// A DreamSequence is a sequence of dreams visited in a day of play.
    /// </summary>
    [ProtoContract]
    public class DreamSequence
    {
        /// <summary>
        /// The list of visited dreams this day.
        /// </summary>
        [ProtoMember(1)] public readonly List<SDK.Data.Dream> Visited = new List<SDK.Data.Dream>();

        /// <summary>
        /// The total upper score across all of the visited dreams.
        /// </summary>
        [ProtoIgnore]
        public int UpperScore { get { return Mathf.Clamp(Visited.Sum(d => d.Upperness) + UpperModifier, -9, 9); } }

        /// <summary>
        /// The total dynamic score across all of the visited dreams.
        /// </summary>
        [ProtoIgnore]
        public int DynamicScore
        {
            get { return Mathf.Clamp(Visited.Sum(d => d.Dynamicness) + DynamicModifier, -9, 9); }
        }

        /// <summary>
        /// Additional points to add to the upper score.
        /// </summary>
        [ProtoMember(2)] public int UpperModifier;

        /// <summary>
        /// Additional points to add to the dynamic score.
        /// </summary>
        [ProtoMember(3)] public int DynamicModifier;
    }
}
