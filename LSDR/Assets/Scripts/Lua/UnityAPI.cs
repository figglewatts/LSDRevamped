using LSDR.Lua.Proxies;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua
{
    public class UnityAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine)
        {
            // register proxies
            UserData.RegisterProxyType<GameObjectProxy, GameObject>(go => new GameObjectProxy(go));
        }

        public static Vector3 Vector3(float x, float y, float z) { return new Vector3(x, y, z); }

        public static float DeltaTime() { return Time.deltaTime; }
    }
}
