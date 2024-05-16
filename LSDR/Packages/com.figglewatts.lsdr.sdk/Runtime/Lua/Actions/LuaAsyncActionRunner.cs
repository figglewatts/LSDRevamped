using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Lua.Actions
{
    public class LuaAsyncActionRunner : MonoBehaviour
    {
        public enum LuaAsyncActionRunnerMode
        {
            OneShot,
            Looping
        }

        private IEnumerator _actionsToRun;
        private LuaAsyncAction _currentAction;
        private bool _firstPredicateBegun;
        private bool _looping;

        private LuaAsyncAction _rootAction;
        private bool _running;

        public void Update()
        {
            // if we're not running or there is no root action, don't update
            if (!_running || _rootAction == null) return;

            // if the timescale is zero we don't want to run
            if (Math.Abs(Time.timeScale) < float.Epsilon) return;

            // start the first predicate in the sequence
            if (!_firstPredicateBegun)
            {
                _rootAction.UntilPredicate?.Begin();
                _firstPredicateBegun = true;
            }

            // run the actions
            if (_actionsToRun == null) _actionsToRun = runActions();
            _actionsToRun.MoveNext();
        }

        public LuaAsyncAction Do(Action action)
        {
            LuaAsyncAction actionObj = new LuaAsyncAction(action, this);
            _rootAction = actionObj;
            _currentAction = _rootAction;
            return actionObj;
        }

        public LuaAsyncAction WaitUntil(IPredicate condition)
        {
            var action = new LuaAsyncAction(() => { }, this).Until(condition);
            _rootAction = action;
            _currentAction = _rootAction;
            return action;
        }

        public void Begin(LuaAsyncActionRunnerMode mode)
        {
            switch (mode)
            {
                default:
                    _running = true;
                    break;
                case LuaAsyncActionRunnerMode.Looping:
                    _running = true;
                    _looping = true;
                    break;
            }
        }

        public void Stop()
        {
            stopRunning();
        }

        private IEnumerator runActions()
        {
            _currentAction = _rootAction;
            while (true)
            {
                // if we've stopped running
                if (_running == false) break;

                // if we're at the end
                if (_currentAction == null)
                {
                    if (_looping)
                    {
                        // if we're looping then start from the beginning again
                        _currentAction = _rootAction;
                        _rootAction.UntilPredicate?.Begin();
                        Debug.Log("looping");
                    }
                    else
                    {
                        // otherwise simply stop running
                        stopRunning();
                        break;
                    }
                }

                // perform the action
                _currentAction.Action?.Invoke();

                // if we have a predicate, we need to check if it's satisfied or not
                if (_currentAction.UntilPredicate != null && !_currentAction.UntilPredicate.Predicate())
                {
                    // defer execution, we need to wait until the predicate is satisfied
                    Debug.Log("waiting for predicate");
                    yield return null;
                    continue;
                }

                // if we didn't have a predicate, or if we had a predicate and it was satisfied, then we can move to
                // the next action
                Debug.Log($"switching to next action: {_currentAction.Chained}");
                switchToAction(_currentAction.Chained);
            }
        }

        private void stopRunning()
        {
            _running = false;
            _firstPredicateBegun = false;
            _actionsToRun = null;
        }

        private void switchToAction(LuaAsyncAction next)
        {
            _currentAction = next;
            next?.UntilPredicate?.Begin();
        }
    }
}
