using LSDR.Game;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class PauseSystemProxy : AbstractLuaProxy<PauseSystem>
    {
        [MoonSharpHidden]
        public PauseSystemProxy(PauseSystem target) : base(target) { }

        public bool IsPaused => _target.Paused;
    }
}
