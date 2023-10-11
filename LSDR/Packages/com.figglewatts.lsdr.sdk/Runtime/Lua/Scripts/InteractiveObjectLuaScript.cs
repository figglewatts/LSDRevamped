using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public sealed class InteractiveObjectLuaScript : AbstractStartUpdateLuaScript
    {
        public const string INTERACT_FUNCTION_NAME = "interact";
        public const string INTERVAL_UPDATE_FUNCTION_NAME = "intervalUpdate";

        private readonly InteractiveObject _interactiveObject;
        private DynValue _interactFunc;
        private DynValue _intervalUpdateFunc;

        public InteractiveObjectLuaScript(ILuaEngine engine, LuaScriptAsset asset, InteractiveObject interactiveObject)
            : base(engine, asset)
        {
            _interactiveObject = interactiveObject;
            Script.Globals["this"] = _interactiveObject;
            compile();
            loadInteractiveObjectFunctions();

            Start();
        }

        public void Interact()
        {
            if (_interactFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_interactFunc); });
        }

        public void IntervalUpdate()
        {
            if (_intervalUpdateFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_intervalUpdateFunc); });
        }

        private void loadInteractiveObjectFunctions()
        {
            _interactFunc = Script.Globals.Get(INTERACT_FUNCTION_NAME);
            _intervalUpdateFunc = Script.Globals.Get(INTERVAL_UPDATE_FUNCTION_NAME);
        }
    }
}
