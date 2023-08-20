using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSDR.SDK.Audio;
using Torii.Resource;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace LSDR.Audio
{
    public class MusicPlayer : MonoSingleton<MusicPlayer>
    {
        public bool IsPlaying => _source && _source.isPlaying;

        protected AudioSource _source;

        protected const string MIXER_PATH = "Mixers/MasterMixer";

        public void Start()
        {
            AudioMixer mixer = ResourceManager.UnityLoad<AudioMixer>(MIXER_PATH);
            AudioMixerGroup group = mixer.FindMatchingGroups("Music").First();

            _source = gameObject.AddComponent<AudioSource>();
            _source.loop = true;
            _source.outputAudioMixerGroup = group;
            _source.spatialBlend = 0;
            _source.playOnAwake = false;
        }

        public void StopSong() => StartCoroutine(stopSong());

        public void PlaySong(SongAsset song) => StartCoroutine(playSong(song));

        protected IEnumerator playSong(SongAsset song)
        {
            if (IsPlaying) yield return stopSong();

            yield return new WaitForSecondsRealtime(0.5f);

            if (song == null || song.IsSilent) yield break;

            _source.volume = 1;
            _source.clip = song.Clip;
            _source.Play();

        }

        protected IEnumerator stopSong()
        {
            float t = 0;
            float fadeTimeSeconds = 1;
            while (t < fadeTimeSeconds)
            {
                _source.volume = 1 - (t / fadeTimeSeconds);
                yield return null;
                t += Time.unscaledDeltaTime;
            }
            _source.Stop();
        }
    }
}
