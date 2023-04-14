using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class TriggerLuaLuaScript : AbstractLuaScript
    {
        public const string START_FUNCTION_NAME = "start";
        public const string TRIGGER_FUNCTION_NAME = "onTrigger";

        protected DynValue _startFunc;
        private DynValue _triggerFunc;

        public TriggerLuaLuaScript(ILuaEngine engine, LuaScriptAsset asset) : base(engine, asset)
        {
            loadFunctions();

            Start();
        }

        public void Start() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_startFunc); }); }

        public void OnTrigger() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_triggerFunc); }); }

        private void loadFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _triggerFunc = Script.Globals.Get(TRIGGER_FUNCTION_NAME);
        }
    }
}
