using LSDR.SDK.Assets;
using LSDR.SDK.Lua;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class TriggerLua : BaseTrigger
    {
        public LuaScriptAsset Script;
        public string TriggerFunctionName = "onTrigger";
        public string TriggerExitFunctionName = "onTriggerExit";

        protected TriggerLuaLuaScript _luaScript;

        protected override Color _editorColour { get; } = new Color(r: 0, g: 0, b: 0.8f);

        public override void Init()
        {
            if (Script)
                _luaScript = new TriggerLuaLuaScript(LuaManager.Managed, Script, TriggerFunctionName,
                    TriggerExitFunctionName, this);
        }

        protected override void onTrigger(Collider player)
        {
            _luaScript.OnTrigger();
        }

        protected override void onTriggerExit(Collider player)
        {
            _luaScript.OnTriggerExit();
        }
    }
}
