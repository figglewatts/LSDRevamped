using LSDR.Entities.Player;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class PlayerMovementProxy : AbstractLuaProxy<PlayerMovement>
    {
        [MoonSharpHidden]
        public PlayerMovementProxy(PlayerMovement target) : base(target) { }
    }
}
