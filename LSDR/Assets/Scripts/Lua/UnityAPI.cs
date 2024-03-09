using System;
using LSDR.Lua.Proxies;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.Lua
{
    public class UnityAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine, Script script)
        {
            // register proxies
            UserData.RegisterProxyType<GameObjectProxy, GameObject>(go => new GameObjectProxy(go));
            UserData.RegisterProxyType<InputActionProxy, InputAction>(ia => new InputActionProxy(ia));
        }

        public static Vector3 Vector3(float x, float y, float z) => new Vector3(x, y, z);

        public static Color ColorRGB(float r, float g, float b) => new Color(r, g, b);
        public static Color ColorRGBA(float r, float g, float b, float a) => new Color(r, g, b, a);

        public static float DeltaTime() { return Time.deltaTime; }

        public static float CurrentTime() { return Time.realtimeSinceStartup; }
    }
}
