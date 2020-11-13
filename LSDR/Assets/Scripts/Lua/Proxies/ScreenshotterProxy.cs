using LSDR.Visual;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class ScreenshotterProxy : AbstractLuaProxy<Screenshotter>
    {
        [MoonSharpHidden]
        public ScreenshotterProxy(Screenshotter target) : base(target) { }

        public void TakeScreenshot() { _target.TakeScreenshot(); }
    }
}
