using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class TriggerLuaLuaScript : AbstractStartUpdateLuaScript
    {
        public const string TRIGGER_FUNCTION_NAME = "onTrigger";

        private DynValue _triggerFunc;

        public TriggerLuaLuaScript(ILuaEngine engine, LuaScriptAsset asset) : base(engine, asset)
        {
            loadTriggerFunctions();
            Start();
        }

        public void OnTrigger() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerFunc); }); }

        private void loadTriggerFunctions()
        {
            _triggerFunc = Script.Globals.Get(TRIGGER_FUNCTION_NAME);
        }
    }
}
