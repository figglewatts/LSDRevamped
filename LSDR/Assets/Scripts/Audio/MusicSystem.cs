using System;
using System.IO;
using System.Linq;
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

        private string _currentSongFilename;

        public void Awake()
        {
            CurrentArtist = "No artist";
            CurrentSong = "No song";
        }

        /// <summary>
        /// Play a random song from the given directory and return the AudioSource playing the song.
        /// </summary>
        /// <param name="dir">The directory to use.</param>
        /// <returns>The AudioSource that is playing the song. Null if an error occurred.</returns>
        public AudioSource PlayRandomSongFromDirectory(string dir)
        {
            var clip = getRandomSongFromDirectory(dir);
            if (clip == null)
            {
                return null;
            }
            return AudioPlayer.Instance.PlayClip(clip, loop: true, mixerGroup: "Music");
        }

        /// <summary>
        /// Play a random song from the given directory using a given AudioSource.
        /// </summary>
        /// <param name="dir">The directory to use.</param>
        /// <returns>The AudioSource that is playing the song. Null if an error occurred.</returns>
        public AudioSource PlayRandomSongFromDirectory(AudioSource source, string dir)
        {
            var clip = getRandomSongFromDirectory(dir);
            if (clip == null)
            {
                return null;
            }
            source.clip = clip;
            source.Play();
            return source;
        }

        private AudioClip getRandomSongFromDirectory(string dir)
        {
            var files = Directory.GetFiles(dir, "*.ogg", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Debug.LogWarning($"Music directory '{dir}' did not contain any OGG files, cannot play music.");
                CurrentArtist = "No artist";
                CurrentSong = "No song";
                return null;
            }

            // handle case where directory only contains 1 file - we'd want to just play it again instead of
            // excluding it and causing no song to play next
            var filesToChooseFrom = files.Length == 1 ? files : files.Where(f => !f.Equals(_currentSongFilename));
            
            var randomFile = RandUtil.RandomListElement(filesToChooseFrom);
            _currentSongFilename = randomFile;
            
            setMetadataFromFilePath(randomFile);
            Debug.Log($"Now playing: {CurrentSong} by {CurrentArtist}");
            
            return ResourceManager.Load<ToriiAudioClip>(randomFile, "scene");
        }

        private void setMetadataFromFilePath(string filePath)
        {
            if (filePath == null) return;
            
            var filename = Path.GetFileNameWithoutExtension(filePath);
            if (filename.Contains(" - "))
            {
                var splitFilename = filename.Split(new[] {" - "}, 2, StringSplitOptions.RemoveEmptyEntries);
                CurrentArtist = splitFilename[0];
                CurrentSong = splitFilename[1];
            }
            else
            {
                CurrentArtist = "Unknown artist";
                CurrentSong = filename;
            }
        }
    }
}