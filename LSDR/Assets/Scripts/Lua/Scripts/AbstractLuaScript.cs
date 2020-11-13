using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public abstract class AbstractLuaScript
    {
        protected readonly Script Script;

        protected AbstractLuaScript(Script script) { Script = script; }
    }
}
