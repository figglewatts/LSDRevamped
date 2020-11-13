using LSDR.Entities.Original;
using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public class InteractiveObjectLuaScript : AbstractLuaScript
    {
        public InteractiveObjectLuaScript(Script script, InteractiveObject obj) : base(script)
        {
            loadFunctions();
            provideInteractiveObjectAPI(obj);
        }

        public const string START_FUNCTION_NAME = "start";
        public const string UPDATE_FUNCTION_NAME = "update";

        private DynValue _startFunc;
        private DynValue _updateFunc;

        public static InteractiveObjectLuaScript Load(string filePath, InteractiveObject obj)
        {
            var script = LuaEngine.CreateBaseAPI();
            LuaEngine.LoadScript(filePath, script);
            return new InteractiveObjectLuaScript(script, obj);
        }

        public void Start() { Script.Call(_startFunc); }

        public void Update() { Script.Call(_updateFunc); }

        private void loadFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _updateFunc = Script.Globals.Get(UPDATE_FUNCTION_NAME);
        }

        private void provideInteractiveObjectAPI(InteractiveObject obj) { Script.Globals["obj"] = obj; }
    }
}
