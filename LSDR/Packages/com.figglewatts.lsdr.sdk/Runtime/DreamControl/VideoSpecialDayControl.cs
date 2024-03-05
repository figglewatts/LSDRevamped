using System;
using LSDR.SDK.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace LSDR.SDK.DreamControl
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoSpecialDayControl : MonoBehaviour
    {
        protected VideoPlayer _videoPlayer;
        protected bool _over = false;

        public void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void BeginVideoClip(VideoClip clip)
        {
            _videoPlayer.clip = clip;
            _videoPlayer.loopPointReached += endVideoDay;
            _videoPlayer.Play();
        }

        public void BeginVideoDay(VideoSpecialDay day)
        {
            _videoPlayer.clip = day.VideoClip;
            _videoPlayer.loopPointReached += endVideoDay;
            _videoPlayer.Play();
        }

        public void Update()
        {
            bool skipInputPressed = Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                                                                Gamepad.current.startButton.wasPressedThisFrame);
            bool skipKeyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            if (!_over && _videoPlayer.isPlaying && (skipInputPressed || skipKeyPressed))
            {
                endVideoDay(_videoPlayer);
            }
        }

        protected void endVideoDay(VideoPlayer player)
        {
            _over = true;
            DreamControlManager.Managed.VideoFinished();
            DreamControlManager.Managed.EndDream();
            player.Pause();
        }
    }
}
