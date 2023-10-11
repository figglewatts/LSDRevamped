using System;
using UnityEngine;

namespace LSDR.SDK.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimatedObject : MonoBehaviour
    {
        public AnimationClip[] Clips;

        protected Animator _animator;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Play(int index)
        {
            if (index >= Clips.Length)
            {
                Debug.LogWarning($"Unable to play animation {index} on object '{gameObject.name}', " +
                                 $"object only has {Clips.Length} animations");
                return;
            }

            _animator.enabled = true;
            _animator.Play(Clips[index].name);
        }

        public void Resume() { _animator.enabled = true; }

        public void Stop() { _animator.enabled = false; }
    }
}
