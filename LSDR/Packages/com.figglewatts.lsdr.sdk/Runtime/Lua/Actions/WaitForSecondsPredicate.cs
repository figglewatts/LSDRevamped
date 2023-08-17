using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.SDK.Lua.Actions
{
    [MoonSharpUserData]
    public class WaitForSecondsPredicate : IPredicate
    {
        protected readonly float _numSeconds;
        protected float _startTime;

        public WaitForSecondsPredicate(float seconds) { _numSeconds = seconds; }

        public bool Predicate() { return Time.time > _startTime + _numSeconds; }

        public void Begin() { _startTime = Time.time; }
    }
}
