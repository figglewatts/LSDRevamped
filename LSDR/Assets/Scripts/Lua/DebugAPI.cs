using System;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua
{
    public class DebugAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine, Script script) { }

        public static void Log(string msg) { Debug.Log(msg); }

        public static void LogWarning(string msg) { Debug.LogWarning(msg); }

        public static void LogError(string msg) { Debug.LogError(msg); }
    }
}
