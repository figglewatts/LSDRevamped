using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class DreamAudioProxy : AbstractLuaProxy<DreamAudio>
    {
        [MoonSharpHidden]
        public DreamAudioProxy(DreamAudio target) : base(target) { }

        public void Play() => _target.StartPlaying();
        public void Stop() => _target.StopPlaying();
    }
}
