using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public class TriggerLuaLuaScript : AbstractLuaScript
    {
        public TriggerLuaLuaScript(Script script) : base(script) { loadFunctions(); }

        public const string TRIGGER_FUNCTION_NAME = "onTrigger";

        private DynValue _triggerFunc;

        public static TriggerLuaLuaScript Load(string filePath)
        {
            var script = LuaEngine.CreateBaseAPI();
            LuaEngine.LoadScript(filePath, script);
            return new TriggerLuaLuaScript(script);
        }

        public void Trigger() { Script.Call(_triggerFunc); }

        private void loadFunctions() { _triggerFunc = Script.Globals.Get(TRIGGER_FUNCTION_NAME); }
    }
}
