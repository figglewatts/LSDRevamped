using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public interface ILuaAPI
    {
        void Register(ILuaEngine engine, Script script);
    }
}
