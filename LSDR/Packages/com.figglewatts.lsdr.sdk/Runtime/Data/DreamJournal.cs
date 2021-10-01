using System.Collections.Generic;
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
    }
}
