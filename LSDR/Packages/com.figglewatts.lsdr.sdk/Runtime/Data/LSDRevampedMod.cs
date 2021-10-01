using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Mod")]
    public class LSDRevampedMod : ScriptableObject
    {
        [Tooltip("The name of this mod.")] public string Name;

        [Tooltip("The author of this mod.")] public string Author;

        [Tooltip("The dream journals contained in this mod.")]
        public List<DreamJournal> Journals;
    }
}
