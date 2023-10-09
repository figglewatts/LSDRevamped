using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public sealed class InteractiveObjectLuaScript : AbstractStartUpdateLuaScript
    {
        public const string INTERACT_FUNCTION_NAME = "interact";

        private readonly InteractiveObject _interactiveObject;
        private DynValue _interactFunc;


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

        private void loadInteractiveObjectFunctions()
        {
            _interactFunc = Script.Globals.Get(INTERACT_FUNCTION_NAME);
        }
    }
}
