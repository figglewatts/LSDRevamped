using UnityEngine;

namespace LSDR.SDK.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatedObject : MonoBehaviour
    {
        public AnimationClip[] Clips;
    }
}
