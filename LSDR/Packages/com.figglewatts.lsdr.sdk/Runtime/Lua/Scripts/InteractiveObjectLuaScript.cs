using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class InteractiveObjectLuaScript : AbstractLuaScript
    {
        public const string START_FUNCTION_NAME = "start";
        public const string UPDATE_FUNCTION_NAME = "update";
        public const string INTERACT_FUNCTION_NAME = "interact";

        protected readonly InteractiveObject _interactiveObject;
        protected DynValue _interactFunc;
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

        public void Interact() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_interactFunc); }); }

        private void loadFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _updateFunc = Script.Globals.Get(UPDATE_FUNCTION_NAME);
            _interactFunc = Script.Globals.Get(INTERACT_FUNCTION_NAME);
        }
    }
}
