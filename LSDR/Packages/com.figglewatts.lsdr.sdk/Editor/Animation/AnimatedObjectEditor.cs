using LSDR.SDK.Animation;
using LSDR.SDK.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Animation
{
    [CustomEditor(typeof(AnimatedObject))]
    public class AnimatedObjectEditor : UnityEditor.Editor
    {
        protected bool _animationsFoldoutOpen = true;
        protected Animator _animator;

        protected int _currentlyPlayingAnimation = -1;
        protected AnimatedObject _target;

        public void OnEnable()
        {
            _target = (AnimatedObject)target;
            _animator = _target.GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Target GameObject '{_target.gameObject}' did not have Animator component!");
            }
        }

        public void OnDisable() { stopAnimation(); }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AnimationClip[] clips = getClips();

            _animationsFoldoutOpen = EditorGUILayout.Foldout(_animationsFoldoutOpen, "Animations");
            if (_animationsFoldoutOpen)
            {
                EditorGUILayout.BeginVertical();
                {
                    for (int i = 0; i < clips.Length; i++)
                    {
                        drawClipGUI(clips[i], i);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        protected void drawClipGUI(AnimationClip clip, int clipIndex)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(pixels: 10);
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("AnimationClip Icon"),
                    GUILayout.Width(width: 20));
                EditorGUILayout.LabelField(clip.name);

                if (ToggleButton.OnGUI(_currentlyPlayingAnimation == clipIndex,
                        EditorGUIUtility.IconContent("PlayButton"), GUILayout.Width(width: 30)))
                {
                    if (_currentlyPlayingAnimation == clipIndex)
                    {
                        stopAnimation();
                    }
                    else
                    {
                        playAnimation(clipIndex);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void playAnimation(int clipIndex)
        {
            if (_currentlyPlayingAnimation != clipIndex)
            {
                stopAnimation();
            }

            _currentlyPlayingAnimation = clipIndex;
            resetAnimation();
            _animator.hideFlags |= HideFlags.NotEditable;
            EditorApplication.update += updateAnimationPreview;
        }

        protected void stopAnimation()
        {
            if (_currentlyPlayingAnimation == -1) return;

            resetAnimation();
            _animator.hideFlags &= ~HideFlags.NotEditable;
            EditorApplication.update -= updateAnimationPreview;

            _currentlyPlayingAnimation = -1;
        }

        protected void resetAnimation()
        {
            AnimationClip clip = getClip(_currentlyPlayingAnimation);
            clip.SampleAnimation(_target.gameObject, time: 0);
            _animator.Play(clip.name, layer: 0, normalizedTime: 0f);
            _animator.Update(deltaTime: 0);
        }

        protected void updateAnimationPreview()
        {
            if (_currentlyPlayingAnimation == -1) return;

            AnimationClip clip = getClip(_currentlyPlayingAnimation);
            clip.SampleAnimation(_target.gameObject, Time.deltaTime);
            _animator.Update(Time.deltaTime);
        }

        protected AnimationClip getClip(int index) { return getClips()[index]; }

        protected AnimationClip[] getClips() { return _animator.runtimeAnimatorController.animationClips; }
    }
}
