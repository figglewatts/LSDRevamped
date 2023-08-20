using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public abstract class AbstractStartUpdateLuaScript : AbstractLuaScript
    {
        public const string START_FUNCTION_NAME = "start";
        public const string UPDATE_FUNCTION_NAME = "update";

        private DynValue _startFunc;
        private DynValue _updateFunc;

        protected AbstractStartUpdateLuaScript(ILuaEngine engine, LuaScriptAsset script) : base(engine, script)
        {
            loadStartUpdateFunctions();
        }

        public void Start() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_startFunc); }); }

        public void Update() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_updateFunc); }); }

        private void loadStartUpdateFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _updateFunc = Script.Globals.Get(UPDATE_FUNCTION_NAME);
        }
    }
}
