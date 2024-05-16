using System;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua.Actions
{
    [MoonSharpUserData]
    public class GenericPredicate : IPredicate
    {
        protected Func<bool> _predicate;
        protected Action _begin;

        public GenericPredicate(Func<bool> predicate, Action begin)
        {
            _predicate = predicate;
            _begin = begin;
        }


        public bool Predicate() { return _predicate(); }
        public void Begin() { _begin(); }
    }
}
