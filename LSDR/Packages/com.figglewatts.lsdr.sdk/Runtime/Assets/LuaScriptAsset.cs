using System;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.SDK.Assets
{
    public class LuaScriptAsset : ScriptableObject
    {
        [HideInInspector] public string ScriptText;

        public Script Compile(ILuaEngine engine)
        {
            Script baseApi = engine.CreateBaseAPI();

            try
            {
                HandleLuaErrorsFor(() => { baseApi.DoString(ScriptText); });
            }
            catch (InternalErrorException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (SyntaxErrorException e)
            {
                Console.WriteLine($"Lua Syntax Error: {e.DecoratedMessage}");
            }

            return baseApi;
        }

        public void HandleLuaErrorsFor(Action action)
        {
            try
            {
                action();
            }
            catch (ScriptRuntimeException e)
            {
                Debug.LogError($"Lua Script Runtime Error: {e.DecoratedMessage}");
            }
        }
    }
}
