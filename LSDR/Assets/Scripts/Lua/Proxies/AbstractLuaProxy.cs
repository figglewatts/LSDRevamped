namespace LSDR.Lua.Proxies
{
    public abstract class AbstractLuaProxy<T>
    {
        protected T _target;

        protected AbstractLuaProxy(T target) { _target = target; }
    }
}
