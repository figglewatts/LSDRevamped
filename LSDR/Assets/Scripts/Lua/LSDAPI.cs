using System;
using LSDR.Lua.Proxies;
using LSDR.SDK;
using LSDR.SDK.Audio;
using LSDR.SDK.Entities;
using LSDR.SDK.Lua;
using LSDR.Visual;
using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public class LSDAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine, Script script)
        {
            // register proxies
            UserData.RegisterProxyType<ScreenshotterProxy, Screenshotter>(screenshotter =>
                new ScreenshotterProxy(screenshotter));
            UserData.RegisterProxyType<InteractiveObjectProxy, InteractiveObject>(r => new InteractiveObjectProxy(r));
            //UserData.RegisterProxyType<DreamSystemProxy, DreamSystem>(r => new DreamSystemProxy(r));

            // register types
            engine.RegisterEnum<TextureSet>();
            engine.RegisterEnum<SongStyle>();
            UserData.RegisterType<SongAsset>();
            UserData.RegisterType<SongListAsset>();
        }

        public static BaseEntity GetEntity(string id)
        {
            return EntityIndex.Instance.Get(id).GetComponent<BaseEntity>();
        }
    }
}
