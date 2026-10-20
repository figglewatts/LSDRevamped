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

        [Tooltip("How long a dream that starts here lasts in seconds. 0 for a random length.")]
        public float DreamLengthSeconds = 0;

        [Tooltip("Walking speed in this dream, in units per second.")]
        public float WalkSpeed = 1.25f;

        [Tooltip("The offset into the environments this dream starts at.")]
        public int EnvironmentOffset = 0;

        [Tooltip("The list of environments this dream can potentially have.")]
        public DreamEnvironmentSequence Environments;

        [Tooltip("4 groups of environments to pick from by day instead of the list. Leave empty to use the list.")]
        public List<DreamEnvironmentSequence> EnvironmentGroups;

        [NonSerialized] public int EnvironmentGroup = -1;

        // the four kinds of sky a dream's environment groups hold, in this order
        public const int DAY_SKIES = 0;
        public const int PALE_SKIES = 1;
        public const int NIGHT_SKIES = 2;
        public const int DARK_SKIES = 3;

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
            EnvironmentGroup = -1;

            // if the dream doesn't have environments, just return a default one
            if (Environments.Environments.Count <= 0) return CreateInstance<DreamEnvironment>();
            return RandUtil.RandomListElement(Environments.Environments);
        }

        // each day moves on to the next kind of sky, and each dream is offset from the day so they don't all match
        public DreamEnvironment ChooseEnvironment(int dayNum, int dreamIndex)
        {
            EnvironmentGroup = -1;
            if (EnvironmentGroups == null || EnvironmentGroups.Count != 4) return ChooseEnvironment(dayNum);

            int n = dayNum + dreamIndex;
            EnvironmentGroup = n % EnvironmentGroups.Count;
            List<DreamEnvironment> environments = EnvironmentGroups[EnvironmentGroup].Environments;
            return environments[n % environments.Count];
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
