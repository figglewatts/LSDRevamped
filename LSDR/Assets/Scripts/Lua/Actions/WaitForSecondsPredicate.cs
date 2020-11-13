using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Actions
{
    [MoonSharpUserData]
    public class WaitForSecondsPredicate : IPredicate
    {
        protected float _startTime;
        protected readonly float _numSeconds;

        public WaitForSecondsPredicate(float seconds) { _numSeconds = seconds; }

        public bool Predicate() { return Time.time > _startTime + _numSeconds; }

        public void Begin() { _startTime = Time.time; }
    }
}
