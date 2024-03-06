using System;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Util;
using LSDR.SDK.Visual;
using LSDR.UI.Title;
using Torii.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace LSDR.Game
{
    [RequireComponent(typeof(VideoPlayer))]
    public class IntroVideoController : MonoBehaviour
    {
        public VideoClip[] IntroVideos;

        protected VideoPlayer _videoPlayer;
        protected bool _over = false;

        public void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void Start()
        {
            ToriiFader.Instance.FadeOut(0.1f);

            var randomVideo = RandUtil.RandomArrayElement(IntroVideos);
            _videoPlayer.clip = randomVideo;
            _videoPlayer.loopPointReached += endIntro;
            _videoPlayer.Play();
        }

        public void Update()
        {
            bool skipInputPressed = Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                                                                Gamepad.current.startButton.wasPressedThisFrame);
            bool skipKeyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            if (!_over && _videoPlayer.isPlaying && (skipInputPressed || skipKeyPressed))
            {
                endIntro(_videoPlayer);
            }
        }

        protected void endIntro(VideoPlayer player)
        {
            _over = true;
            player.Pause();
            ToriiFader.Instance.FadeIn(Color.black, 2f, () =>
            {
                SceneManager.LoadScene("titlescreen");
            });
        }
    }
}
