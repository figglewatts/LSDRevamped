using System;
using System.IO;
using NVorbis;
using Torii.Resource;
using UnityEngine;

namespace LSDR.IO.ResourceHandlers
{
    /// <summary>
    /// A ResourceHandler to handle loading AudioClips.
    /// </summary>
    public class AudioClipHandler : IResourceHandler
    {
        public Type HandlerType => typeof(AudioClip);

        /// <summary>
        /// Load an audio file from disk.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        /// <param name="span">The span ID for how long this resource should be cached.</param>
        public void Load(string path, int span)
        {
            using (var vorbis = new VorbisReader(path))
            {
                AudioClip clip = AudioClip.Create(path, (int)vorbis.TotalSamples, vorbis.Channels, vorbis.SampleRate,
                    false);
                var readBuffer = new float[vorbis.Channels * vorbis.SampleRate / 5]; // 200ms

                int count;
                int total = 0;
                while ((count = vorbis.ReadSamples(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    total += count;
                    clip.SetData(readBuffer, total / vorbis.Channels - 1);
                }
                
                Resource<AudioClip> resource = new Resource<AudioClip>(clip, span);
                ResourceManager.RegisterResource(path, resource);
            }
        }
    }
}
