using LSDR.Entities.Player;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class PlayerRotationProxy : AbstractLuaProxy<PlayerRotation>
    {
        [MoonSharpHidden]
        public PlayerRotationProxy(PlayerRotation target) : base(target) { }
    }
}
