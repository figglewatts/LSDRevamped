using System;
using System.IO;
using LSDR.IO;
using LSDR.Util;
using Torii.Audio;
using Torii.Resource;
using Torii.Util;
using UnityEngine;

namespace LSDR.Audio
{
    [CreateAssetMenu(menuName="System/MusicSystem")]
    public class MusicSystem : ScriptableObject
    {
        public string CurrentSong { get; private set; }
        public string CurrentArtist { get; private set; }
        
        public AudioSource PlayRandomSongFromDirectory(string dir)
        {
            var clip = getRandomSongFromDirectory(dir);
            return AudioPlayer.Instance.PlayClip(clip, loop: true, mixerGroup: "Music");
        }

        public AudioSource PlayRandomSongFromDirectory(AudioSource source, string dir)
        {
            var clip = getRandomSongFromDirectory(dir);
            source.clip = clip;
            source.Play();
            return source;
        }

        private AudioClip getRandomSongFromDirectory(string dir)
        {
            var files = Directory.GetFiles(dir, "*.ogg", SearchOption.AllDirectories);
            var randomFile = RandUtil.From(files);
            setMetadataFromFilePath(randomFile);
            Debug.Log($"Now playing: {CurrentSong} by {CurrentArtist}");
            return ResourceManager.Load<ToriiAudioClip>(randomFile, "scene");
        }

        private void setMetadataFromFilePath(string filePath)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            if (filename != null)
            {
                var splitFilename = filename.Split(new[] {" - "}, 2, StringSplitOptions.RemoveEmptyEntries);
                CurrentArtist = splitFilename[0];
                CurrentSong = splitFilename[1];
            }
        }
    }
}
