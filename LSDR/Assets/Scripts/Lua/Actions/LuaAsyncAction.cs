using System;

namespace LSDR.Lua.Actions
{
    public class LuaAsyncAction
    {
        public readonly Action Action;

        /// <summary>
        /// The predicate of how long this action should run. If it returns true, the action should no longer
        /// be run. By default (i.e. if .Until(...) is not called on this action, it will always return true,
        /// meaning the action is only ever called once.
        /// </summary>
        public IPredicate UntilPredicate { get; private set; }

        public LuaAsyncAction Chained { get; private set; }

        protected readonly LuaAsyncActionRunner _runner;

        public LuaAsyncAction(Action action, LuaAsyncActionRunner runner)
        {
            Action = action;
            _runner = runner;
        }

        public LuaAsyncAction Until(IPredicate predicate)
        {
            if (predicate == null) return this;

            UntilPredicate = predicate;
            return this;
        }

        public LuaAsyncAction Then(Action action)
        {
            var newAction = new LuaAsyncAction(action, _runner);
            Chained = newAction;
            return newAction;
        }

        public void ThenFinish() { _runner.Begin(LuaAsyncActionRunner.LuaAsyncActionRunnerMode.OneShot); }

        public void ThenLoop() { _runner.Begin(LuaAsyncActionRunner.LuaAsyncActionRunnerMode.Looping); }
    }
}
