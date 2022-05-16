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

        public Dream GetFirstDream()
        {
            var firstDayDreams = Dreams.Where(d => d.FirstDay).ToList();
            if (firstDayDreams.Count <= 0) return RandUtil.RandomListElement(Dreams);
            
            return RandUtil.RandomListElement(firstDayDreams);
        }

        public Dream GetDreamFromGraph(int x, int y)
        {
            if (GraphSpawnMap != null)
            {
                return GraphSpawnMap.Get(x, y);
            }
            else
            {
                return RandUtil.RandomListElement(Dreams);
            }
        }
    }
}
