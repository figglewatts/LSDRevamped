using LSDR.SDK.Assets;
using LSDR.SDK.Lua;
using LSDR.SDK.Lua.Actions;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(LuaAsyncActionRunner))]
    public class InteractiveObject : BaseEntity
    {
        public LuaScriptAsset Script;
        protected Animator _animator;
        protected InteractiveObjectLuaScript _luaScript;
        public LuaAsyncActionRunner Action { get; protected set; }

        public override void Start()
        {
            base.Start();

            _animator = GetComponent<Animator>();
            Action = GetComponent<LuaAsyncActionRunner>();

            if (Script) _luaScript = new InteractiveObjectLuaScript(LuaManager.Managed, Script, this);
        }

        public void Update() { _luaScript.Update(); }
    }
}
