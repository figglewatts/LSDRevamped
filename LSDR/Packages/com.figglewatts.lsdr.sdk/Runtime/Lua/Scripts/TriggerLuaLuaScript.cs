using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public sealed class TriggerLuaLuaScript : AbstractStartUpdateLuaScript
    {
        private DynValue _triggerFunc;
        private DynValue _triggerExitFunc;
        private readonly TriggerLua _triggerLua;

        public TriggerLuaLuaScript(ILuaEngine engine, LuaScriptAsset asset, string triggerFunctionName,
            string triggerExitFunctionName, TriggerLua triggerLua) : base(engine,
            asset)
        {
            _triggerLua = triggerLua;
            Script.Globals["this"] = triggerLua;

            compile();
            loadTriggerFunctions(triggerFunctionName, triggerExitFunctionName);
            Start();
        }

        public void OnTrigger()
        {
            if (_triggerFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerFunc); });
        }

        public void OnTriggerExit()
        {
            if (_triggerExitFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerExitFunc); });
        }

        private void loadTriggerFunctions(string triggerFunctionName, string triggerExitFunctionName)
        {
            _triggerFunc = Script.Globals.Get(triggerFunctionName);
            _triggerExitFunc = Script.Globals.Get(triggerExitFunctionName);
        }
    }
}
