using LSDR.SDK.DreamControl;
using LSDR.SDK.Visual;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Video")]
    public class VideoSpecialDay : AbstractSpecialDay
    {
        public VideoClip VideoClip;

        public override void HandleDay(int dayNumber)
        {
            SceneManager.LoadScene("video_dream");
            FadeManager.Managed.FadeOut(Color.black, 1f, () =>
            {
                VideoSpecialDayControl control = FindObjectOfType<VideoSpecialDayControl>();
                control.BeginVideoDay(this);
            }, 1);
        }
    }
}
