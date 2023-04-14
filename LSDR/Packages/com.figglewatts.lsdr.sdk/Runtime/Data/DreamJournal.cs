using System.Collections.Generic;
using System.Linq;
using LSDR.SDK.Util;
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

        public Dream GetLinkable(Dream current)
        {
            return RandUtil.RandomListElement(LinkableDreams.Where(d => d != current));
        }

        public Dream GetFirstDream()
        {
            List<Dream> firstDayDreams = Dreams.Where(d => d.FirstDay).ToList();
            if (firstDayDreams.Count <= 0) return RandUtil.RandomListElement(Dreams);

            return RandUtil.RandomListElement(firstDayDreams);
        }

        public Dream GetDreamFromGraph(int x, int y)
        {
            if (GraphSpawnMap != null)
            {
                Dream dream = GraphSpawnMap.Get(x, y);
                if (dream != null) return dream;
            }

            return RandUtil.RandomListElement(Dreams);
        }
    }
}
