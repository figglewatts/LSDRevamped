using System;
using System.Collections;
using LSDR.SDK.Entities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Entities
{
    [CustomEditor(typeof(DreamAudio))]
    public class DreamAudioEditor : UnityEditor.Editor
    {
        protected DreamAudio _this;
        [SerializeField] protected AudioSource _source;
        protected EditorCoroutine _playAudioCoroutine;

        public void OnEnable()
        {
            _this = (DreamAudio)target;
        }

        public void OnDisable()
        {
            stopPreviewing();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                var buttonText = _playAudioCoroutine == null ? "Preview" : "Stop";
                if (GUILayout.Button(buttonText, GUILayout.Width(60)))
                {
                    if (_playAudioCoroutine == null) startPreviewing();
                    else stopPreviewing();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void startPreviewing()
        {
            ensureAudioSource();
            stopPreviewing();
            _playAudioCoroutine = EditorCoroutineUtility.StartCoroutine(editorPlayAudio(), this);
        }

        protected void stopPreviewing()
        {
            if (_playAudioCoroutine != null) EditorCoroutineUtility.StopCoroutine(_playAudioCoroutine);
            _playAudioCoroutine = null;
            if (_source != null) _source.Stop();
        }

        protected IEnumerator editorPlayAudio()
        {
            while (true)
            {
                if (_this.DelayBeforePlayingSeconds > 0)
                    yield return new EditorWaitForSeconds(_this.DelayBeforePlayingSeconds);

                // play the audio
                _source.Play();

                // stop the audio if that setting is enabled
                if (_this.PlayClipForSeconds > 0)
                {
                    yield return new EditorWaitForSeconds(_this.PlayClipForSeconds);
                    _source.Stop();
                }
                else if (_this.Loop)
                {
                    // handle looping
                    yield return new EditorWaitForSeconds(_this.Clip.length * _this.Pitch);
                }

                // handle waiting between audio plays
                if (_this.DelayBetweenPlaysSeconds <= 0 || _this.Loop)
                    yield return null; // ensure we at least yield once
                else yield return new EditorWaitForSeconds(_this.DelayBetweenPlaysSeconds);
            }
        }

        protected void ensureAudioSource()
        {
            if (_source == null)
            {
                GameObject sourceObject = new GameObject("__audioSourceEditor");
                sourceObject.hideFlags = HideFlags.HideAndDontSave;
                _source = sourceObject.AddComponent<AudioSource>();
            }
            _source.spatialBlend = 0; // 2d
            _source.loop = false;
            _source.playOnAwake = false;
            _source.pitch = _this.Pitch;
            _source.volume = _this.Volume;
            _source.clip = _this.Clip;
        }
    }
}
