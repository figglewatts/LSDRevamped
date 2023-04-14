using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace LSDR.Dream
{
    /// <summary>
    ///     A DreamSequence is a sequence of dreams visited in a day of play.
    /// </summary>
    [JsonObject]
    public class DreamSequence
    {
        /// <summary>
        ///     The list of visited dreams this day.
        /// </summary>
        public readonly List<VisitedDream> Visited = new List<VisitedDream>();

        /// <summary>
        ///     Additional points to add to the dynamic score.
        /// </summary>
        public int DynamicModifier;

        /// <summary>
        ///     Additional points to add to the upper score.
        /// </summary>
        public int UpperModifier;

        /// <summary>
        ///     The total upper score across all of the visited dreams.
        /// </summary>
        [JsonIgnore]
        public int UpperScore { get { return Mathf.Clamp(Visited.Sum(d => d.Upperness) + UpperModifier, -9, 9); } }

        /// <summary>
        ///     The total dynamic score across all of the visited dreams.
        /// </summary>
        [JsonIgnore]
        public int DynamicScore
        {
            get { return Mathf.Clamp(Visited.Sum(d => d.Dynamicness) + DynamicModifier, -9, 9); }
        }

        [JsonObject]
        public struct VisitedDream
        {
            public string Name;
            public string Author;
            public int Upperness;
            public int Dynamicness;

            public static implicit operator VisitedDream(SDK.Data.Dream dream)
            {
                return new VisitedDream
                {
                    Name = dream.Name,
                    Author = dream.Author,
                    Upperness = dream.Upperness,
                    Dynamicness = dream.Dynamicness
                };
            }
        }
    }
}
