using System;
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

        public AssetBundle SourceBundle { get; protected set; }

        public bool IsBuiltIn => SourceBundle == null;

        public void SetSourceBundle(AssetBundle bundle)
        {
            SourceBundle = bundle;
        }

        public ResourceRequest GetDreamPrefabAsync(Dream dream)
        {
            if (IsBuiltIn)
            {
                return Resources.LoadAsync<GameObject>(dream.DreamPrefabPath);
            }
            else
            {
                return SourceBundle.LoadAssetAsync<GameObject>(dream.DreamPrefabPath);
            }
        }

        public DreamJournal GetJournal(int journalIdx)
        {
            if (journalIdx >= Journals.Count)
            {
                journalIdx = Journals.Count - 1;
            }
            else if (journalIdx < 0)
            {
                journalIdx = 0;
            }

            return Journals[journalIdx];
        }
    }
}
