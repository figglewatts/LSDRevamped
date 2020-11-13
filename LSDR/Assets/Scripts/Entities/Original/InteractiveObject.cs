using System;
using libLSD.Formats;
using LSDR.Lua;
using LSDR.Lua.Actions;
using UnityEngine;

namespace LSDR.Entities.Original
{
    [RequireComponent(typeof(TODAnimator))]
    [RequireComponent(typeof(LuaAsyncActionRunner))]
    [RequireComponent(typeof(AudioSource))]
    public class InteractiveObject : MonoBehaviour
    {
        public InteractiveObjectData Data;
        public int IdleAnimation = 0;
        public bool PlayIdleAnimation = false;
        public InteractiveObjectLuaScript Script;
        public TODAnimator Animator;
        public LuaAsyncActionRunner ActionRunner;
        public AudioSource AudioSource;

        public void Awake()
        {
            Animator = GetComponent<TODAnimator>();
            ActionRunner = GetComponent<LuaAsyncActionRunner>();
            AudioSource = GetComponent<AudioSource>();
            initAudioSource(AudioSource);
        }

        public void Start() { Script?.Start(); }

        public void Update() { Script?.Update(); }

        public static GameObject Create(LBD lbd,
            int momIndex,
            Material material,
            string objName,
            int idleAnimation = 0,
            bool playIdleAnimation = false,
            string scriptPath = null)
        {
            if (!lbd.MML.HasValue)
            {
                throw new ArgumentException("Provided LBD did not have entities!", nameof(lbd));
            }

            if (momIndex >= lbd.MML.Value.MOMs.Length)
            {
                throw new ArgumentException(
                    $"LBD only has {lbd.MML.Value.NumberOfMOMs} entities, can't load entity '{momIndex}'");
            }

            if (idleAnimation >= lbd.MML.Value.MOMs[momIndex].MOS.TODs.Length)
            {
                Debug.LogWarning(
                    $"Entity only has {lbd.MML.Value.MOMs[momIndex].MOS.TODs.Length} animations, " +
                    $"can't set idle animation to animation '{idleAnimation}'. Defaulting to '0' instead.");
                idleAnimation = 0;
            }

            GameObject obj = new GameObject(objName);
            var objData = new InteractiveObjectData(lbd.MML.Value.MOMs[momIndex], material);

            var animator = obj.AddComponent<TODAnimator>();
            animator.SetAnimation(objData.Animations[idleAnimation]);
            if (playIdleAnimation) animator.Resume();

            var objScript = obj.AddComponent<InteractiveObject>();
            objScript.Data = objData;

            // load the Lua script if it's given
            if (!string.IsNullOrEmpty(scriptPath))
            {
                objScript.Script = InteractiveObjectLuaScript.Load(scriptPath, objScript);
            }

            return obj;
        }

        private void initAudioSource(AudioSource source)
        {
            // use 3D sound
            source.spatialBlend = 1;
        }
    }
}
