using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public abstract class AbstractLuaScript
    {
        protected readonly ILuaEngine _engine;
        protected readonly LuaScriptAsset _scriptAsset;

        protected AbstractLuaScript(ILuaEngine engine, LuaScriptAsset script)
        {
            _engine = engine;
            _scriptAsset = script;
            Script = _scriptAsset.Compile(_engine);
        }

        public Script Script { get; protected set; }
    }
}
