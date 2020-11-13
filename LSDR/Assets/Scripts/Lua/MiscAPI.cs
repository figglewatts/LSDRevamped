using UnityEngine;

namespace LSDR.Lua
{
    public class MiscAPI
    {
        public static void Log(string msg) { Debug.Log(msg); }

        public static void LogWarning(string msg) { Debug.LogWarning(msg); }

        public static void LogError(string msg) { Debug.LogError(msg); }
    }
}
