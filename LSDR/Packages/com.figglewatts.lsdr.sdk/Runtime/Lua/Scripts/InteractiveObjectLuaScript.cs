using LSDR.SDK.Assets;
using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public class InteractiveObjectLuaScript : AbstractStartUpdateLuaScript
    {
        public const string INTERACT_FUNCTION_NAME = "interact";

        protected readonly InteractiveObject _interactiveObject;
        protected DynValue _interactFunc;


        public InteractiveObjectLuaScript(ILuaEngine engine, LuaScriptAsset asset, InteractiveObject interactiveObject)
            : base(engine, asset)
        {
            _interactiveObject = interactiveObject;
            loadInteractiveObjectFunctions();
            Script.Globals["this"] = _interactiveObject;

            Start();
        }

        public void Interact() { _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_interactFunc); }); }

        private void loadInteractiveObjectFunctions()
        {
            _interactFunc = Script.Globals.Get(INTERACT_FUNCTION_NAME);
        }
    }
}
