using System;
using System.IO;
using NVorbis;
using UnityEngine;

namespace Torii.Audio
{
    public class ToriiAudioClip : IDisposable
    {
        public AudioClip Clip { get; private set; }

        private VorbisReader _vorbis;

        public ToriiAudioClip(string filePath)
        {
            loadOgg(filePath);
        }
        
        public void Dispose()
        {
            _vorbis?.Dispose();
        }
        
        public static implicit operator AudioClip(ToriiAudioClip toriiAudioClip) => toriiAudioClip.Clip;

        private void loadOgg(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !Path.GetExtension(filePath).Equals(".ogg"))
            {
                Debug.Log($"Unable to load audio clip from path '{filePath}', only OGG format supported.");
                return;
            }
            
            _vorbis = new VorbisReader(filePath);
            
            AudioClip.PCMReaderCallback onAudioRead = data => { _vorbis.ReadSamples(data, 0, data.Length); };
            AudioClip.PCMSetPositionCallback onAudioSetPosition = newPos => { _vorbis.DecodedPosition = newPos; };

            string fileName = Path.GetFileName(filePath);

            Clip = AudioClip.Create(fileName, (int)_vorbis.TotalSamples, _vorbis.Channels, _vorbis.SampleRate,
                true, onAudioRead, onAudioSetPosition);
        }
    }
}
