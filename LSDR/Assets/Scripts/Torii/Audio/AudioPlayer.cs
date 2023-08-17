using System;
using System.Collections.Generic;
using System.Linq;
using Torii.Exceptions;
using Torii.Resource;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Torii.Audio
{
    public class AudioPlayer : MonoSingleton<AudioPlayer>
    {
        private const string MIXER_PATH = "Mixers/MasterMixer";
        public int MaxChannels = 10;

        private List<AudioSource> _channels;
        public int _channelsAvailable => _channels.Count(c => !c.isPlaying);
        public AudioSource FreeChannel => _channels.First(c => !c.isPlaying);

        public override void Init()
        {
            _channels = new List<AudioSource>();
            addChannel();
        }

        private void setMixerGroup(AudioSource channel, string mixerGroup)
        {
            try
            {
                AudioMixer mixer = ResourceManager.UnityLoad<AudioMixer>(MIXER_PATH);
                AudioMixerGroup group = mixer.FindMatchingGroups(mixerGroup).First();
                channel.outputAudioMixerGroup = group;
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

        public AudioSource PlayClip(AudioClip clip, bool loop = false, string mixerGroup = null)
        {
            AudioSource channel = _channelsAvailable == 0 ? addChannel() : FreeChannel;
            if (channel == null)
            {
                Debug.LogWarning($"Unable to play audio clip '{clip.name}', no channels available");
                return null;
            }

            if (!string.IsNullOrEmpty(mixerGroup))
            {
                setMixerGroup(channel, mixerGroup);
            }

            channel.loop = loop;
            channel.clip = clip;
            channel.Play();

            return channel;
        }

        private AudioSource addChannel()
        {
            if (_channels.Count + 1 > MaxChannels)
            {
                return null;
            }

            AudioSource channel = gameObject.AddComponent<AudioSource>();
            _channels.Add(channel);
            return channel;
        }
    }
}
