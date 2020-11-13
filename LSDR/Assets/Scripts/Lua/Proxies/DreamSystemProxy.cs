using LSDR.Dream;
using LSDR.Entities.Dream;
using Torii.Audio;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class DreamSystemProxy : AbstractLuaProxy<DreamSystem>
    {
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        public DreamSystemProxy(DreamSystem target) : base(target) { }

        public TextureSet CurrentTextureSet => _target.TextureSetSystem.CurrentTextureSet;

        public void SwitchSong(string songPath) { _target.PlaySong(songPath); }

        public void SkipSong() { _target.SkipSong(); }

        public void PlaySound(ToriiAudioClip clip) { AudioPlayer.Instance.PlayClip(clip, mixerGroup: "sfx"); }

        public void EndDream() { _target.EndDream(); }

        public void SetEnvironment(int idx) { _target.ApplyDreamEnvironment(idx); }

        public void SetTextureSet(TextureSet set) { _target.ApplyTextureSet(set); }

        public void Link(Color fadeCol, string dreamPath, bool playSound)
        {
            _target.Transition(fadeCol, dreamPath, playSound);
        }

        public void LinkToPoint(Color fadeCol, string dreamPath, bool playSound, string spawnPointId)
        {
            _target.Transition(fadeCol, dreamPath, playSound, spawnPointId);
        }

        public string RandomLinkableDream() { return _target.JournalLoader.Current.GetLinkableDream(); }

        public void ModifyUpper(int score) { _target.CurrentSequence.UpperModifier += score; }

        public void ModifyDynamic(int score) { _target.CurrentSequence.DynamicModifier += score; }
    }
}
