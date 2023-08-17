using LSDR.SDK.Lua;
using UnityEngine;

namespace LSDR.Lua
{
    public class MiscAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine) { }

        public static void Log(string msg) { Debug.Log(msg); }

        public static void LogWarning(string msg) { Debug.LogWarning(msg); }

        public static void LogError(string msg) { Debug.LogError(msg); }
    }
}
