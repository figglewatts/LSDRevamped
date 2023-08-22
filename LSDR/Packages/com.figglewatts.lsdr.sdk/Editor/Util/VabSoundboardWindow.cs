using System;
using System.Linq;
using LSDR.SDK.Assets;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    public class VabSoundboardWindow : EditorWindow
    {
        public VABAsset VAB;

        protected SerializedObject _thisObj;
        protected float _pitch = 1.0f;
        protected AudioSource _source;
        protected int[] _excludes;
        protected string _stringExcludes;
        protected Vector2 _scrollPos;

        protected const string _excludesEditorPref = "LSDR_VABSoundboard_Excludes";


        [MenuItem("LSDR SDK/VAB Soundboard")]
        public static void ShowWindow()
        {
            GetWindow<VabSoundboardWindow>("VAB Soundboard");
        }

        public void OnEnable()
        {
            _thisObj = new SerializedObject(this);
            loadExcludes();
        }

        public void OnGUI()
        {
            _thisObj.Update();

            EditorGUILayout.PropertyField(_thisObj.FindProperty("VAB"));

            if (VAB == null)
            {
                EditorGUILayout.HelpBox("You need to select a VAB to use this", MessageType.Warning, wide: true);
                _thisObj.ApplyModifiedProperties();
                return;
            }

            EditorGUI.BeginChangeCheck();
            {
                _stringExcludes = EditorGUILayout.TextField("Exclude sample numbers", _stringExcludes);
            }
            if (EditorGUI.EndChangeCheck())
            {
                saveExcludes();
            }

            _pitch = EditorGUILayout.FloatField("Pitch", _pitch);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                for (int i = 0; i < VAB.Samples.Count; i++)
                {
                    if (_excludes != null && _excludes.Contains(i)) continue;

                    AudioClip sample = VAB.Samples[i];
                    if (GUILayout.Button($"Play {sample.name}"))
                    {
                        playClip(sample);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            _thisObj.ApplyModifiedProperties();
        }

        public void OnDestroy()
        {
            if (_source)
            {
                DestroyImmediate(_source.gameObject);
            }
        }

        protected void playClip(AudioClip clip)
        {
            ensureAudioSource();
            _source.clip = clip;
            _source.pitch = _pitch;
            _source.Play();
        }

        protected void saveExcludes()
        {
            EditorPrefs.SetString(_excludesEditorPref, _stringExcludes);
            loadExcludes();
        }

        protected void loadExcludes()
        {
            _stringExcludes = EditorPrefs.GetString(_excludesEditorPref, string.Empty);
            if (_stringExcludes.Equals(string.Empty, StringComparison.InvariantCulture))
            {
                _excludes = Array.Empty<int>();
                return;
            }
            _excludes = _stringExcludes.Split(',').Select(Int32.Parse).ToArray();
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
        }
    }
}
