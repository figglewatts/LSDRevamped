using System;
using System.Collections.Generic;
using LSDR.SDK.Assets;
using LSDR.SDK.Audio;
using LSDR.SDK.Lua;
using LSDR.SDK.Util;
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

        [Tooltip("The offset into the environments this dream starts at.")]
        public int EnvironmentOffset = 0;

        [Tooltip("The list of environments this dream can potentially have.")]
        public DreamEnvironmentSequence Environments;

        [Tooltip("Whether or not this dream has environment effects enabled.")]
        public bool HasEnvironmentEffects = true;

        [Tooltip("The prefab that comprises this dream.")]
        [AssetName(typeof(GameObject))]
        public string DreamPrefabPath;

        [Tooltip("The song library this dream uses to play music.")]
        public AbstractSongLibrary SongLibrary;

        [Tooltip("The script to run with this dream.")]
        public LuaScriptAsset DreamScript;

        [NonSerialized] protected DreamLuaScript _dreamLuaScript;

        public void CreateScript()
        {
            if (DreamScript == null) return;
            _dreamLuaScript = new DreamLuaScript(LuaManager.Managed, DreamScript, this);
            _dreamLuaScript.Start();
        }

        public void UpdateScript()
        {
            _dreamLuaScript?.Update();
        }

        public DreamEnvironment RandomEnvironment()
        {
            // if the dream doesn't have environments, just return a default one
            if (Environments.Environments.Count <= 0) return CreateInstance<DreamEnvironment>();
            return RandUtil.RandomListElement(Environments.Environments);
        }

        public DreamEnvironment ChooseEnvironment(int dayNum)
        {
            // if the dream doesn't have an environment sequence, return a default one
            if (!Environments) return CreateInstance<DreamEnvironment>();
            
            // if the dream doesn't have environments, just return a default one
            if (Environments.Environments.Count <= 0) return CreateInstance<DreamEnvironment>();

            int toChoose = (dayNum - 1 + EnvironmentOffset) % Environments.Environments.Count;
            return Environments.Environments[toChoose];
        }

        public override string ToString()
        {
            return $"{Name} ({Author})";
        }
    }
}
