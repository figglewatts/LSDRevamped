using System;
using System.Linq;
using Torii.Exceptions;
using Torii.Resource;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Torii.Audio
{
    public class Audio : MonoSingleton<Audio>
    {
        private AudioSource _source;

        private const string MIXER_PATH = "Mixers/MasterMixer";

        public override void Init()
        {
            ensureAudioSource();
        }

        public void SetMixerGroup(string mixerGroup)
        {
            try
            {
                AudioMixer mixer = ResourceManager.UnityLoad<AudioMixer>(MIXER_PATH);
                AudioMixerGroup group = mixer.FindMatchingGroups(mixerGroup).First();
                _source.outputAudioMixerGroup = group;
            }
            catch (ToriiResourceLoadException)
            {
                Debug.LogError("Unable to find master audio mixer!");
                throw;
            }
            catch (InvalidOperationException)
            {
                Debug.LogError($"Unable to find audio mixer group '{mixerGroup}'!");
                throw;
            }
        }

        public void PlayClip(AudioClip clip, string mixerGroup = null)
        {
            ensureAudioSource();
            
            if (!string.IsNullOrEmpty(mixerGroup))
            {
                SetMixerGroup(mixerGroup);
            }

            _source.PlayOneShot(clip);
        }

        private void ensureAudioSource()
        {
            _source = gameObject.AddComponent<AudioSource>();
        }
    }
}
