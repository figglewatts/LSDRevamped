using LSDR.InputManagement;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.Lua.Proxies
{
    public class ControlSchemeLoaderSystemProxy : AbstractLuaProxy<ControlSchemeLoaderSystem>
    {
        [MoonSharpHidden]
        public ControlSchemeLoaderSystemProxy(ControlSchemeLoaderSystem target) : base(target) { }

        public ControlScheme Current => _target.Current;

        public Vector2 GetMove()
        {
            return _target.InputActions.Game.Move.ReadValue<Vector2>();
        }

        public Vector2 GetLook()
        {
            return _target.InputActions.Game.Look.ReadValue<Vector2>();
        }

        public float GetStrafe()
        {
            return _target.InputActions.Game.Strafe.ReadValue<float>();
        }

        public InputAction Interact => _target.InputActions.Game.Interact;
        public InputAction Run => _target.InputActions.Game.Run;
        public InputAction LookUp => _target.InputActions.Game.LookUp;
        public InputAction LookDown => _target.InputActions.Game.LookDown;
    }
}
