using LSDR.Entities.Player;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class PlayerCameraRotationProxy : AbstractLuaProxy<PlayerCameraRotation>
    {
        [MoonSharpHidden]
        public PlayerCameraRotationProxy(PlayerCameraRotation target) : base(target) { }

        public bool Enabled
        {
            get => _target.enabled;
            set => _target.enabled = value;
        }
    }
}
