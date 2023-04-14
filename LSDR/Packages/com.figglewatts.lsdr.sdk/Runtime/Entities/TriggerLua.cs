using LSDR.SDK.Assets;
using LSDR.SDK.Lua;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class TriggerLua : BaseTrigger
    {
        public LuaScriptAsset Script;

        protected TriggerLuaLuaScript _luaScript;

        public override void Start()
        {
            base.Start();

            if (Script) _luaScript = new TriggerLuaLuaScript(LuaManager.Managed, Script);
        }

        protected override void onTrigger(Collider player) { _luaScript.OnTrigger(); }
    }
}
