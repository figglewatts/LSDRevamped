using LSDR.SDK.Assets;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public abstract class AbstractLuaScript
    {
        protected readonly LuaScriptAsset _scriptAsset;

        protected AbstractLuaScript(ILuaEngine engine, LuaScriptAsset script)
        {
            _scriptAsset = script;
            Script = engine.CreateBaseAPI();
        }

        protected virtual void compile()
        {
            _scriptAsset.Compile(Script);
        }

        public Script Script { get; protected set; }
    }
}
