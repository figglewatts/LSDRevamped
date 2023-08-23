using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class TriggerLuaLuaScript : AbstractStartUpdateLuaScript
    {
        private DynValue _triggerFunc;
        private DynValue _triggerExitFunc;

        public TriggerLuaLuaScript(ILuaEngine engine, LuaScriptAsset asset, string triggerFunctionName,
            string triggerExitFunctionName) : base(engine,
            asset)
        {
            loadTriggerFunctions(triggerFunctionName, triggerExitFunctionName);
            Start();
        }

        public void OnTrigger() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerFunc); }); }

        public void OnTriggerExit() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerExitFunc); }); }

        private void loadTriggerFunctions(string triggerFunctionName, string triggerExitFunctionName)
        {
            _triggerFunc = Script.Globals.Get(triggerFunctionName);
            _triggerExitFunc = Script.Globals.Get(triggerExitFunctionName);
        }
    }
}
