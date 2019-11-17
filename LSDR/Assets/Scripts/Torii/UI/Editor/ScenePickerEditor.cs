using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Torii.UI
{
    [CustomPropertyDrawer(typeof(ScenePicker))]
    public class ScenePickerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneName = property.FindPropertyRelative("ScenePath");
            
            SceneAsset sceneObject = getScene(sceneName.stringValue);

            Object scene = EditorGUI.ObjectField(position, label, sceneObject, typeof(SceneAsset), true);

            if (scene == null)
            {
                sceneName.stringValue = "";
            }
            else if (scene.name != sceneName.stringValue)
            {
                SceneAsset sceneObj = getScene(scene.name);

                if (sceneObj != null)
                {
                    sceneName.stringValue = scene.name;
                }
            }
        }

        private SceneAsset getScene(string sceneName)
        {
            foreach (EditorBuildSettingsScene editorScene in EditorBuildSettings.scenes)
            {
                if (editorScene.path.IndexOf(sceneName, StringComparison.Ordinal) != -1)
                {
                    return AssetDatabase.LoadAssetAtPath<SceneAsset>(editorScene.path);
                }
            }
            
            Debug.LogWarning($"Scene '{sceneName}' not added to build -- add it to the build to pick it.");
            return null;
        }
    }
}
