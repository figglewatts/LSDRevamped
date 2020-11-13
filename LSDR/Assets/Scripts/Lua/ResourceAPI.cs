using MoonSharp.Interpreter;
using Torii.Audio;
using Torii.Resource;
using Torii.Util;
using UnityEngine;

namespace LSDR.Lua
{
    public class ResourceAPI
    {
        static ResourceAPI()
        {
            UserData.RegisterType<ToriiAudioClip>();
            UserData.RegisterType<Texture2D>();
        }

        public static ToriiAudioClip LoadAudio(string filepath)
        {
            return ResourceManager.Load<ToriiAudioClip>(PathUtil.Combine(Application.streamingAssetsPath, filepath),
                "scene");
        }

        public static Texture2D LoadTexture(string filepath)
        {
            return ResourceManager.Load<Texture2D>(PathUtil.Combine(Application.streamingAssetsPath, filepath),
                "scene");
        }
    }
}
