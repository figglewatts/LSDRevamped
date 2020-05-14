using System;
using System.IO;
using NVorbis;
using Torii.Audio;
using Torii.Resource;
using UnityEngine;

namespace LSDR.IO.ResourceHandlers
{
    /// <summary>
    /// A ResourceHandler to handle loading AudioClips.
    /// </summary>
    public class ToriiAudioClipHandler : IResourceHandler
    {
        public Type HandlerType => typeof(ToriiAudioClip);

        /// <summary>
        /// Load an audio file from disk.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        /// <param name="span">The span ID for how long this resource should be cached.</param>
        public void Load(string path, int span)
        {
            ToriiAudioClip clip = new ToriiAudioClip(path);
            Resource<ToriiAudioClip> resource = new Resource<ToriiAudioClip>(clip, span);
            ResourceManager.RegisterResource(path, resource);
        }
    }
}
