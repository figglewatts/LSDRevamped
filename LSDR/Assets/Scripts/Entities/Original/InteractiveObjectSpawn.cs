using System;
using System.IO;
using libLSD.Formats;
using LSDR.Visual;
using ProtoBuf;
using Torii.Resource;
using Torii.UnityEditor;
using Torii.Util;
using UnityEngine;

namespace LSDR.Entities.Original
{
    public class InteractiveObjectSpawn : BaseEntity
    {
        [BrowseFileSystem(BrowseType.File, new[] {"LBD file", "lbd"}, "lbd")]
        public string LBDFile;

        public int EntityNumber;
        public int IdleAnimation;
        public bool PlayIdleAnimation;

        [BrowseFileSystem(BrowseType.File, new[] {"Lua script", "lua"}, "script")]
        public string LuaScript;

        [NonSerialized] public GameObject SpawnedObject;

        public override EntityMemento Save() { return new InteractiveObjectSpawnMemento(this); }

        public override void Restore(EntityMemento memento, LevelEntities entities)
        {
            base.Restore(memento, entities);

            var objMemento = (InteractiveObjectSpawnMemento)memento;
            LBDFile = objMemento.LBDFile;
            EntityNumber = objMemento.EntityNumber;
            IdleAnimation = objMemento.IdleAnimation;
            PlayIdleAnimation = objMemento.PlayIdleAnimation;
            LuaScript = objMemento.LuaScript;
            entities.Register(this);

            if (Application.isPlaying) spawnObject();
        }

#if UNITY_EDITOR
        public void CreateObject()
        {
            if (SpawnedObject != null) return;

            var lbdPath = PathUtil.Combine(Application.streamingAssetsPath, LBDFile);
            LBD lbd = ResourceManager.Load<LBD>(lbdPath, "scene");
            Material mat = new Material(Shader.Find("LSDR/RevampedDiffuse"));
            SpawnedObject =
                InteractiveObject.Create(lbd, EntityNumber, mat, EntityID, IdleAnimation, PlayIdleAnimation, LuaScript);
            SpawnedObject.transform.SetParent(transform);
            SpawnedObject.transform.localPosition = Vector3.zero;
        }

        public void RemoveObject() { DestroyImmediate(SpawnedObject); }
#endif

        public void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawIcon(position, "InteractiveObjectSpawn.png");
        }

        private void spawnObject()
        {
            if (SpawnedObject != null) return;

            var lbdPath = PathUtil.Combine(Application.streamingAssetsPath, LBDFile);
            LBD lbd = ResourceManager.Load<LBD>(lbdPath, "scene");
            Material mat = new Material(DreamSystem.GetShader(alpha: false));
            SpawnedObject =
                InteractiveObject.Create(lbd, EntityNumber, mat, EntityID, IdleAnimation, PlayIdleAnimation, LuaScript);
            SpawnedObject.transform.position = transform.position;
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class InteractiveObjectSpawnMemento : EntityMemento
    {
        public string LBDFile;
        public int EntityNumber;
        public int IdleAnimation;
        public bool PlayIdleAnimation;
        public string LuaScript;

        protected override Type EntityType => typeof(InteractiveObjectSpawn);

        public InteractiveObjectSpawnMemento(InteractiveObjectSpawn state) : base(state)
        {
            LBDFile = state.LBDFile;
            EntityNumber = state.EntityNumber;
            IdleAnimation = state.IdleAnimation;
            PlayIdleAnimation = state.PlayIdleAnimation;
            LuaScript = state.LuaScript;
        }
    }
}
