using LSDR.SDK.Data;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK
{
    [CustomEditor(typeof(DreamEnvironment))]
    public class DreamEnvironmentEditor : UnityEditor.Editor
    {
        protected bool _showFog = true;
        protected bool _showSky = true;
        protected RenderSettings _oldRenderSettings;
        protected Material _previewSkyMaterial;

        protected static readonly int _subtractiveFogPropertyId = Shader.PropertyToID("_SubtractiveFog");
        protected static readonly int _fogHeightPropertyId = Shader.PropertyToID("_FogHeight");
        protected static readonly int _fogGradientPropertyId = Shader.PropertyToID("_FogGradient");
        protected static readonly int _skyColorPropertyId = Shader.PropertyToID("_SkyColor");
        protected static readonly int _fogColorPropertyId = Shader.PropertyToID("_FogColor");

        protected class RenderSettings
        {
            public readonly Material Sky;
            public readonly bool Fog;
            public readonly Color FogColor;
            public readonly float FogStart;
            public readonly float FogEnd;
            public readonly FogMode FogMode;
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

        public void OnDestroy() { destroyPreview(); }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _showFog = EditorGUILayout.Foldout(_showFog, "Fog", toggleOnLabelClick: true);
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

            _showSky = EditorGUILayout.Foldout(_showSky, "Sky", toggleOnLabelClick: true);
            if (_showSky)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SkyColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SkyFogColor"));

                EditorGUILayout.Space();

                var sunChance = serializedObject.FindProperty("SunChance");
                EditorGUILayout.PropertyField(sunChance);
                if (sunChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SunColor"));

                    var secondSunChance = serializedObject.FindProperty("SecondSunChance");
                    EditorGUILayout.PropertyField(secondSunChance);
                    if (secondSunChance.floatValue > 0)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("SecondSunColor"));
                    }
                }

                EditorGUILayout.Space();

                var sunBurstChance = serializedObject.FindProperty("SunBurstChance");
                EditorGUILayout.PropertyField(sunBurstChance);
                if (sunBurstChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SunBurstColor"));
                }

                EditorGUILayout.Space();

                var cloudsChance = serializedObject.FindProperty("CloudsChance");
                EditorGUILayout.PropertyField(cloudsChance);
                if (cloudsChance.floatValue > 0)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CloudsColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOfClouds"));
                }

                EditorGUILayout.Space();

                var starsChance = serializedObject.FindProperty("StarsChance");
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
            var dreamEnvironment = (DreamEnvironment)target;

            _oldRenderSettings = new RenderSettings();

            _previewSkyMaterial = new Material(Shader.Find("LSDR/GradientSky"));
            _previewSkyMaterial.SetFloat(_fogHeightPropertyId, dreamEnvironment.FogHeight);
            _previewSkyMaterial.SetFloat(_fogGradientPropertyId, dreamEnvironment.FogGradient);
            _previewSkyMaterial.SetColor(_skyColorPropertyId, dreamEnvironment.SkyColor);
            _previewSkyMaterial.SetColor(_fogColorPropertyId, dreamEnvironment.SkyFogColor);

            UnityEngine.RenderSettings.skybox = _previewSkyMaterial;
            UnityEngine.RenderSettings.fog = true;
            UnityEngine.RenderSettings.fogColor = dreamEnvironment.FogColor;
            UnityEngine.RenderSettings.fogStartDistance = dreamEnvironment.FogStartDistance;
            UnityEngine.RenderSettings.fogEndDistance = dreamEnvironment.FogEndDistance;
            UnityEngine.RenderSettings.fogMode = FogMode.Linear;
            Shader.SetGlobalInt(_subtractiveFogPropertyId, dreamEnvironment.SubtractiveFog ? 1 : 0);
        }

        protected void destroyPreview()
        {
            if (_oldRenderSettings == null) return;

            _oldRenderSettings.Apply();
            _oldRenderSettings = null;
            DestroyImmediate(_previewSkyMaterial);
            _previewSkyMaterial = null;
        }
    }
}
