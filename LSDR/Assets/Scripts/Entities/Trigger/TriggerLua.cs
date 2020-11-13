using System;
using LSDR.Lua;
using ProtoBuf;
using Torii.UnityEditor;
using UnityEngine;

namespace LSDR.Entities.Trigger
{
    public class TriggerLua : BaseEntity
    {
        [BrowseFileSystem(BrowseType.File, new[] {"Lua script", "lua"}, "script")]
        public string LuaScript;

        protected TriggerLuaLuaScript Script;

        private BoxCollider _collider;

        public void Start()
        {
            // create the collider
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.size = transform.localScale;
            _collider.isTrigger = true;

            Script = TriggerLuaLuaScript.Load(LuaScript);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            Script.Trigger();
        }

        public void OnDrawGizmos()
        {
            var position = transform.position;
            Gizmos.DrawIcon(position, "TriggerLua.png");
            Gizmos.color = Color.green;
            var localScale = transform.localScale;
            Gizmos.DrawWireCube(position, localScale);
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(position, localScale);
        }

        public override EntityMemento Save() { return new TriggerLuaMemento(this); }

        public override void Restore(EntityMemento memento, LevelEntities entities)
        {
            base.Restore(memento, entities);

            var triggerLuaMemento = (TriggerLuaMemento)memento;
            LuaScript = triggerLuaMemento.LuaScript;
            entities.Register(this);
        }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
    public class TriggerLuaMemento : EntityMemento
    {
        public string LuaScript;

        protected override Type EntityType => typeof(TriggerLua);

        public TriggerLuaMemento(TriggerLua state) : base(state) { LuaScript = state.LuaScript; }
    }
}
