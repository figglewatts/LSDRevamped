using System;
using System.Linq;
using LSDR.Dream;
using LSDR.Entities.Player;
using LSDR.Lua.Proxies;
using LSDR.SDK;
using LSDR.SDK.Animation;
using LSDR.SDK.Audio;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Entities;
using LSDR.SDK.Lua;
using LSDR.Visual;
using MoonSharp.Interpreter;
using UnityEngine;

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
            UserData.RegisterProxyType<PlayerMovementProxy, PlayerMovement>(e => new PlayerMovementProxy(e));
            UserData.RegisterProxyType<PlayerCameraRotationProxy, PlayerCameraRotation>(e =>
                new PlayerCameraRotationProxy(e));
            UserData.RegisterProxyType<PlayerMovementProxy, PlayerMovement>(e => new PlayerMovementProxy(e));
            UserData.RegisterProxyType<DreamSystemProxy, DreamSystem>(r => new DreamSystemProxy(r));
            UserData.RegisterProxyType<DreamAudioProxy, DreamAudio>(e => new DreamAudioProxy(e));
            UserData.RegisterProxyType<DreamProxy, SDK.Data.Dream>(e => new DreamProxy(e));

            // register types
            engine.RegisterEnum<TextureSet>();
            engine.RegisterEnum<SongStyle>();
            UserData.RegisterType<SongAsset>();
            UserData.RegisterType<SongListAsset>();
            UserData.RegisterType<DreamEnvironment>();
            UserData.RegisterType<AnimatedObject>();
        }

        public static GameObject GetEntity(string id)
        {
            return EntityIndex.Instance.Get(id);
        }

        public static bool IsDayEven()
        {
            return DreamControlManager.Managed.CurrentDay % 2 == 0;
        }

        public static bool IsWeekDay(int number) => (DreamControlManager.Managed.CurrentDay - 1) % 7 == number - 1;

        public static bool IsDayLinear(int slope, int constant) =>
            (DreamControlManager.Managed.CurrentDay - constant) % slope == 0;

        public static void SetCanControlPlayer(bool state)
        {
            DreamControlManager.Managed.SetCanControlPlayer(state);
        }

        public static SDK.Data.Dream GetDreamByName(string dreamName)
        {
            var dream = DreamControlManager.Managed.GetDreamsFromJournal().FirstOrDefault(d => d.Name == dreamName);
            var journal = DreamControlManager.Managed.GetCurrentJournal();
            if (dream == null)
            {
                Debug.LogError($"unable to find dream with name '{dreamName}' in journal '{journal}'");
            }
            return dream;
        }
    }
}
