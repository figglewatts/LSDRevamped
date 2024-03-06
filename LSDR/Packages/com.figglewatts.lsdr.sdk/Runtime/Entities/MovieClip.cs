using LSDR.SDK.DreamControl;
using UnityEngine;
using UnityEngine.Video;

namespace LSDR.SDK.Entities
{
    public class MovieClip : BaseEntity
    {
        public VideoClip Clip;

        public void Play(Color fadeInColor)
        {
            DreamControlManager.Managed.PlayVideo(Clip, fadeInColor);
        }
    }
}
