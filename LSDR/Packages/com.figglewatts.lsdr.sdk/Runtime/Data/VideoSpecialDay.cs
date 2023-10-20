using UnityEngine;
using UnityEngine.Video;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Video")]
    public class VideoSpecialDay : AbstractSpecialDay
    {
        public VideoClip VideoClip;

        public override void HandleDay(int dayNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
