using System.Collections.Generic;
using System.Linq;
using Torii.Util;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream journal")]
    public class DreamJournal : ScriptableObject
    {
        [Tooltip("The name of this journal.")] public string Name;

        [Tooltip("The author of this journal.")]
        public string Author;

        [Tooltip("The list of dreams contained in this journal.")]
        public List<Dream> Dreams;

        [Tooltip("Map determining where the player spawns for each graph space.")]
        public GraphSpawnMap GraphSpawnMap;

        public IEnumerable<Dream> LinkableDreams => Dreams.Where(d => d.Linkable);

        public Dream GetLinkable() => RandUtil.RandomListElement(LinkableDreams);

        public Dream GetFirstDream() => RandUtil.RandomListElement(Dreams.Where(d => d.FirstDay));

        public Dream GetDreamFromGraph(int x, int y) => GraphSpawnMap.Get(x, y);
    }
}
