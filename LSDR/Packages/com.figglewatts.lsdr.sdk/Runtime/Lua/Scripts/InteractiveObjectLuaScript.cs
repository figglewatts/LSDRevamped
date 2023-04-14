using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class InteractiveObjectLuaScript : AbstractLuaScript
    {
        public const string START_FUNCTION_NAME = "start";
        public const string UPDATE_FUNCTION_NAME = "update";

        protected readonly InteractiveObject _interactiveObject;
        protected DynValue _startFunc;
        protected DynValue _updateFunc;

        public InteractiveObjectLuaScript(ILuaEngine engine, LuaScriptAsset asset, InteractiveObject interactiveObject)
            : base(engine, asset)
        {
            _interactiveObject = interactiveObject;
            loadFunctions();
            Script.Globals["this"] = _interactiveObject;

            Start();
        }

        public void Start() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_startFunc); }); }

        public void Update() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_updateFunc); }); }

        private void loadFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _updateFunc = Script.Globals.Get(UPDATE_FUNCTION_NAME);
        }
    }
}
