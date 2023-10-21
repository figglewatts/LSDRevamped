using System;
using LSDR.Game;
using UnityEngine;
using UnityEngine.Video;

namespace LSDR.Audio
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerVolumeSetter : MonoBehaviour
    {
        public SettingsSystem SettingsSystem;

        protected VideoPlayer _videoPlayer;

        public void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void Start()
        {
            _videoPlayer.SetDirectAudioVolume(0, SettingsSystem.Settings.MusicVolume);
        }
    }
}
