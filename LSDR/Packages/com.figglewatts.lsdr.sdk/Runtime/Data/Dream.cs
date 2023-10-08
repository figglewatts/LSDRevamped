using System.Collections.Generic;
using LSDR.SDK.Audio;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream")]
    public class Dream : ScriptableObject
    {
        [Tooltip("The name of this dream.")] public string Name;

        [Tooltip("The author of this dream.")] public string Author;

        [Tooltip("Whether this dream can spawn the grey man.")]
        public bool GreyMan;

        [Tooltip("Whether this dream is linkable from random links.")]
        public bool Linkable;

        [Tooltip("The weighting of linking to this dream among the other linkable dreams.")] [HideInInspector]
        public float LinkWeighting = 1;

        [Tooltip("Whether this dream should be spawned into on the first day of the journal.")]
        public bool FirstDay;

        [Tooltip("The list of environments this dream can potentially have.")]
        public List<DreamEnvironment> Environments;

        [Tooltip("The prefab that comprises this dream.")]
        public GameObject DreamPrefab;

        [Tooltip("The song library this dream uses to play music.")]
        public AbstractSongLibrary SongLibrary;

        public DreamEnvironment ChooseEnvironment(int dayNum)
        {
            // if the dream doesn't have environments, just return a default one
            if (Environments.Count <= 0) return CreateInstance<DreamEnvironment>();

            int toChoose = (dayNum - 1) % Environments.Count;
            return Environments[toChoose];
        }

        public override string ToString()
        {
            return $"{Name} ({Author})";
        }
    }
}
