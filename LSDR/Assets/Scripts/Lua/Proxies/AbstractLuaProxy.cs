using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public abstract class AbstractLuaProxy<T>
    {
        protected readonly T _target;

        [MoonSharpHidden]
        protected AbstractLuaProxy(T target) { _target = target; }
    }
}
