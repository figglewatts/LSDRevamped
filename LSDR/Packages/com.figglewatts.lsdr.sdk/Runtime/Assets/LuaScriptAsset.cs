using System;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.SDK.Assets
{
    public class LuaScriptAsset : ScriptableObject
    {
        [HideInInspector] public string ScriptText;

        public void Compile(Script script)
        {
            try
            {
                HandleLuaErrorsFor(() => { script.DoString(ScriptText); });
            }
            catch (InternalErrorException e)
            {
                Debug.LogError(e.ToString());
                throw new LuaException("Internal error", e);
            }
            catch (SyntaxErrorException e)
            {
                var message = $"Lua Script ({this}) Syntax Error: {e.DecoratedMessage}";
                Debug.LogError(message);
            }
        }

        public void HandleLuaErrorsFor(Action action)
        {
            try
            {
                action();
            }
            catch (ScriptRuntimeException e)
            {
                var message = $"Lua Script ({this}) Runtime Error: {e.DecoratedMessage}";
                Debug.LogError(message);
            }
        }

        public T HandleLuaErrorsFor<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (ScriptRuntimeException e)
            {
                var message = $"Lua Script ({this}) Runtime Error: {e.DecoratedMessage}";
                Debug.LogError(message);
                Debug.LogException(new LuaException(message, e));
                return default;
            }
        }
    }
}
