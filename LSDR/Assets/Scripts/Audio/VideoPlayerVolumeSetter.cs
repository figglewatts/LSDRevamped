using System;
using System.Collections;
using LSDR.Game;
using UnityEngine;
using UnityEngine.Video;

namespace LSDR.Audio
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerVolumeSetter : MonoBehaviour
    {
        public SettingsSystem SettingsSystem;

        public VideoPlayer VideoPlayer;

        public void Start()
        {
            VideoPlayer = GetComponent<VideoPlayer>();
            VideoPlayer.SetDirectAudioVolume(0, SettingsSystem.Settings.MusicVolume);
        }
    }
}
