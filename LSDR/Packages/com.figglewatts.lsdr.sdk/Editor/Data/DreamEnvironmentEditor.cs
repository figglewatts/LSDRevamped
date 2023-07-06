using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Data
{
    [CustomEditor(typeof(DreamEnvironment))]
    public class DreamEnvironmentEditor : UnityEditor.Editor
    {
        protected static readonly int _subtractiveFogPropertyId = Shader.PropertyToID("_SubtractiveFog");
        protected static readonly int _fogHeightPropertyId = Shader.PropertyToID("_FogHeight");
        protected static readonly int _fogGradientPropertyId = Shader.PropertyToID("_FogGradient");
        protected static readonly int _skyColorPropertyId = Shader.PropertyToID("_SkyColor");
        protected static readonly int _fogColorPropertyId = Shader.PropertyToID("_FogColor");
        protected RenderSettings _oldRenderSettings;
        protected Material _previewSkyMaterial;
        protected bool _showFog = true;
        protected bool _showSky = true;

        public void OnDestroy() { destroyPreview(); }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _showFog = EditorGUILayout.Foldout(_showFog, "Fog", true);
            if (_showFog)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FogColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubtractiveFog"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FogStartDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FogEndDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FogHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FogGradient"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            _showSky = EditorGUILayout.Foldout(_showSky, "Sky", true);
            if (_showSky)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SkyColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SkyFogColor"));

                EditorGUILayout.Space();

                SerializedProperty sunChance = serializedObject.FindProperty("SunChance");
                EditorGUILayout.PropertyField(sunChance);
                if (sunChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SunColor"));

                    SerializedProperty secondSunChance = serializedObject.FindProperty("SecondSunChance");
                    EditorGUILayout.PropertyField(secondSunChance);
                    if (secondSunChance.floatValue > 0)
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SecondSunColor"));
                }

                EditorGUILayout.Space();

                SerializedProperty sunBurstChance = serializedObject.FindProperty("SunBurstChance");
                EditorGUILayout.PropertyField(sunBurstChance);
                if (sunBurstChance.floatValue > 0)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SunBurstColor"));

                EditorGUILayout.Space();

                SerializedProperty cloudsChance = serializedObject.FindProperty("CloudsChance");
                EditorGUILayout.PropertyField(cloudsChance);
                if (cloudsChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CloudsColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOfClouds"));
                }

                EditorGUILayout.Space();

                SerializedProperty starsChance = serializedObject.FindProperty("StarsChance");
                EditorGUILayout.PropertyField(starsChance);
                if (starsChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("StarsColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOfStars"));
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (GUILayout.Button(_oldRenderSettings == null ? "Preview Environment" : "Reset Preview"))
                {
                    if (_oldRenderSettings == null) previewDreamEnvironment();
                    else destroyPreview();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void previewDreamEnvironment()
        {
            DreamEnvironment dreamEnvironment = (DreamEnvironment)target;

            _oldRenderSettings = new RenderSettings();

            dreamEnvironment.Apply();
        }

        protected void destroyPreview()
        {
            if (_oldRenderSettings == null) return;

            _oldRenderSettings.Apply();
            _oldRenderSettings = null;
            DestroyImmediate(_previewSkyMaterial);
            _previewSkyMaterial = null;
        }

        protected class RenderSettings
        {
            public readonly bool Fog;
            public readonly Color FogColor;
            public readonly float FogEnd;
            public readonly FogMode FogMode;
            public readonly float FogStart;
            public readonly Material Sky;
            public readonly bool SubtractiveFog;

            public RenderSettings()
            {
                Sky = UnityEngine.RenderSettings.skybox;
                Fog = UnityEngine.RenderSettings.fog;
                FogColor = UnityEngine.RenderSettings.fogColor;
                FogStart = UnityEngine.RenderSettings.fogStartDistance;
                FogEnd = UnityEngine.RenderSettings.fogEndDistance;
                FogMode = UnityEngine.RenderSettings.fogMode;
                SubtractiveFog = Shader.GetGlobalInt(_subtractiveFogPropertyId) == 1;
            }

            public void Apply()
            {
                UnityEngine.RenderSettings.skybox = Sky;
                UnityEngine.RenderSettings.fog = Fog;
                UnityEngine.RenderSettings.fogColor = FogColor;
                UnityEngine.RenderSettings.fogStartDistance = FogStart;
                UnityEngine.RenderSettings.fogEndDistance = FogEnd;
                UnityEngine.RenderSettings.fogMode = FogMode;
                Shader.SetGlobalInt(_subtractiveFogPropertyId, SubtractiveFog ? 1 : 0);
            }
        }
    }
}
