using LSDR.Dream;
using LSDR.Entities.Dream;
using LSDR.Entities.Original;
using LSDR.Lua.Proxies;
using LSDR.Visual;
using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public class LSDAPI
    {
        static LSDAPI()
        {
            // register proxies
            UserData.RegisterProxyType<ScreenshotterProxy, Screenshotter>(screenshotter =>
                new ScreenshotterProxy(screenshotter));
            UserData.RegisterProxyType<InteractiveObjectProxy, InteractiveObject>(r => new InteractiveObjectProxy(r));
            UserData.RegisterProxyType<DreamSystemProxy, DreamSystem>(r => new DreamSystemProxy(r));

            // register types
            UserData.RegisterType<TODAnimation>();
            UserData.RegisterType<TextureSet>();
            LuaEngine.RegisterGlobalObject(UserData.CreateStatic<TextureSet>(), "TextureSet");
        }

        public static void Register() { }
    }
}
