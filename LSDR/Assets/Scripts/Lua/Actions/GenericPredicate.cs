using System;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Actions
{
    [MoonSharpUserData]
    public class GenericPredicate : IPredicate
    {
        protected Func<bool> _predicate;

        public GenericPredicate(Func<bool> predicate) { _predicate = predicate; }


        public bool Predicate() { return _predicate(); }
        public void Begin() { }
    }
}
