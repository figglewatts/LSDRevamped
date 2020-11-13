namespace LSDR.Lua.Actions
{
    public interface IPredicate
    {
        bool Predicate();
        void Begin();
    }
}
