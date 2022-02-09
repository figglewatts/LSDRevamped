using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream")]
    public class Dream : ScriptableObject
    {
        [Tooltip("The name of this dream.")] public string Name;

        [Tooltip("The author of this dream.")] public string Author;

        [Tooltip("How much visiting this dream affects the upper axis of the graph.")] [Range(-9, 9)]
        public int Upperness;

        [Tooltip("How much visiting this dream affects the dynamic axis of the graph.")] [Range(-9, 9)]
        public int Dynamicness;

        [Tooltip("Whether this dream can spawn the grey man.")]
        public bool GreyMan;

        [Tooltip("Whether this dream is linkable from random links.")]
        public bool Linkable;

        [Tooltip("Whether this dream should be spawned into on the first day of the journal.")]
        public bool FirstDay;

        [Tooltip("The list of environments this dream can potentially have.")]
        public List<DreamEnvironment> Environments;

        [Tooltip("The scene that comprises this dream.")]
        public SceneAsset DreamScene;
    }
}
