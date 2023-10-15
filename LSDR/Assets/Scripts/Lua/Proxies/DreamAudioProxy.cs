using LSDR.SDK.Entities;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class DreamAudioProxy : AbstractLuaProxy<DreamAudio>
    {
        [MoonSharpHidden]
        public DreamAudioProxy(DreamAudio target) : base(target) { }

        public float Pitch
        {
            get => _target.Pitch;
            set => _target.Pitch = value;
        }

        public float Volume
        {
            get => _target.Volume;
            set => _target.Volume = value;
        }

        public void Play() => _target.StartPlaying();
        public void Stop() => _target.StopPlaying();
    }
}
