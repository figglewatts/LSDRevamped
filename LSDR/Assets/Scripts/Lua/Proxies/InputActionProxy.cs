using MoonSharp.Interpreter;
using UnityEngine.InputSystem;

namespace LSDR.Lua.Proxies
{
    public class InputActionProxy : AbstractLuaProxy<InputAction>
    {
        [MoonSharpHidden]
        public InputActionProxy(InputAction target) : base(target) { }

        public bool IsPressed => _target.IsPressed();
        public bool WasPressed => _target.WasPressedThisFrame();
        public bool WasReleased => _target.WasReleasedThisFrame();
    }
}
