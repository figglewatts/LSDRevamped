namespace LSDR.SDK.Lua.Actions
{
    public interface IPredicate
    {
        bool Predicate();
        void Begin();
    }
}
